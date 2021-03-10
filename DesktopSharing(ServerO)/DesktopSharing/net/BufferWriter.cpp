// PHZ
// 2018-5-15

#include "BufferWriter.h"
#include "Socket.h"
#include "SocketUtil.h"
#include "Global.h"

#include <fstream>

using namespace xop;

void xop::WriteUint32BE(char* p, uint32_t value)
{
	p[0] = value >> 24;
	p[1] = value >> 16;
	p[2] = value >> 8;
	p[3] = value & 0xff;
}

void xop::WriteUint32LE(char* p, uint32_t value)
{
	p[0] = value & 0xff;
	p[1] = value >> 8;
	p[2] = value >> 16;
	p[3] = value >> 24;
}

void xop::WriteUint24BE(char* p, uint32_t value)
{
	p[0] = value >> 16;
	p[1] = value >> 8;
	p[2] = value & 0xff;
}

void xop::WriteUint24LE(char* p, uint32_t value)
{
	p[0] = value & 0xff;
	p[1] = value >> 8;
	p[2] = value >> 16;
}

void xop::WriteUint16BE(char* p, uint16_t value)
{
	p[0] = value >> 8;
	p[1] = value & 0xff;
}

void xop::WriteUint16LE(char* p, uint16_t value)
{
	p[0] = value & 0xff;
	p[1] = value >> 8;
}

BufferWriter::BufferWriter(int capacity) 
	: max_queue_length_(capacity)
	, buffer_(new std::queue<Packet>), bufferEncryp_(new std::queue<HexPacket>)
{
	
}	

//tcp stream control
bool BufferWriter::Append(std::shared_ptr<char> data, uint32_t size, uint32_t index)
{
	if (size <= index) {
		return false;
	}
   
	if ((int)buffer_->size() >= max_queue_length_) {
		return false;
	}
     
	Packet pkt = {data, size, index};
#if ENCRYPT_PKT
	HexPacket hpkt(data, size, index, Packet_type::Encrypt);
	if(hpkt.EncryptPacket())
	{
		bufferEncryp_->emplace(std::move(hpkt));
	}
#endif

	buffer_->emplace(std::move(pkt));
	
	return true;
}

//TCP data
bool BufferWriter::Append(const char* data, uint32_t size, uint32_t index)
{
	if (size <= index) {
		return false;
	}
     
	if ((int)buffer_->size() >= max_queue_length_) {
		return false;
	}
     
	Packet pkt;
	pkt.data.reset(new char[size+512]);
	memcpy(pkt.data.get(), data, size);
	pkt.size = size;
	pkt.writeIndex = index;

#if ENCRYPT_PKT
	HexPacket hpkt(pkt.data, size, index, Packet_type::Encrypt);
	if (hpkt.EncryptPacket())
	{
		bufferEncryp_->emplace(std::move(hpkt));
	}
#endif

	buffer_->emplace(std::move(pkt));
	
	return true;
}

#include "Global.h"
#include "AesGcm.h"

std::string getKey(void);
std::string getIV(void);

//send tcp data
int BufferWriter::Send(SOCKET sockfd, int timeout)
{
	if (timeout > 0) {
		SocketUtil::SetBlock(sockfd, timeout); 
	}
      
	int ret = 0;
	int count = 1;

	do
	{
		if (buffer_->empty()) {
			return 0;
		}
#if ENCRYPT_PKT
		if (bufferEncryp_->empty()) {
			return 0;
		}
		HexPacket& hpkt = bufferEncryp_->front();
#endif

		count -= 1;
		Packet& pkt = buffer_->front();
		
		
#if ENCRYPT_PKT == 0
		//Send data
#if NETWORK_OUTPUT
		std::cout << "Send(" << (pkt.size - pkt.writeIndex) << "): " << pkt.data.get() << std::endl;
#endif
		ret = ::send(sockfd, pkt.data.get() + pkt.writeIndex, pkt.size - pkt.writeIndex, 0);
#else
		size_t send_data_bytes = 0;
		auto* send_data = hpkt.GetDataToSend(send_data_bytes);
#if NETWORK_OUTPUT
		std::cout << "Send(" << (send_data_bytes - hpkt.get_offset()) << ")/("<<pkt.size<<"): " << (ENCRYPT_USEBASE64 == 1 ? send_data+4+4 : "(only base64 show data)") << std::endl;
#endif
		ret = ::send(sockfd, send_data, send_data_bytes, 0);
		delete[] send_data;
#endif

		//compute if is connection live
		if (ret > 0) {
			//check if is sended all things
			pkt.writeIndex += ret;
#if ENCRYPT_PKT
			if(ret <= 8)
			{
				throw std::exception("Network error!");
			}
			hpkt.set_offset(hpkt.get_offset() + ret - 4 - sizeof(uint32_t));
			if (hpkt.get_size() == hpkt.get_offset()) {
				count += 1;
				buffer_->pop();
				bufferEncryp_->pop();
			}
#else
			if (pkt.size == pkt.writeIndex) {
				count += 1;
				buffer_->pop();
			}
#endif
		}
		else if (ret < 0) {
#if defined(__linux) || defined(__linux__)
		if (errno == EINTR || errno == EAGAIN) 
#elif defined(WIN32) || defined(_WIN32)
			int error = WSAGetLastError();
			if (error == WSAEWOULDBLOCK || error == WSAEINPROGRESS || error == 0)
#endif
			{
				ret = 0;
			}
		}
	} while (count > 0);

	if (timeout > 0) {
		SocketUtil::SetNonBlock(sockfd);
	}
    
	return ret;
}


