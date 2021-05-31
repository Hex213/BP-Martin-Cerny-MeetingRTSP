#include "TcpConnection.h"
#include "SocketUtil.h"

using namespace xop;

TcpConnection::TcpConnection(TaskScheduler *task_scheduler, SOCKET sockfd)
	: task_scheduler_(task_scheduler)
	, read_buffer_(new BufferReader)
	, write_buffer_(new BufferWriter(500))
	, channel_(new Channel(sockfd))
{
	is_closed_ = false;

	channel_->SetReadCallback([this]() { this->HandleRead(); });
	channel_->SetWriteCallback([this]() { this->HandleWrite(); });
	channel_->SetCloseCallback([this]() { this->HandleClose(); });
	channel_->SetErrorCallback([this]() { this->HandleError(); });

	SocketUtil::SetNonBlock(sockfd);
	SocketUtil::SetSendBufSize(sockfd, 100 * 1024);
	SocketUtil::SetKeepAlive(sockfd);

	channel_->EnableReading();
	task_scheduler_->UpdateChannel(channel_);
}

TcpConnection::~TcpConnection()
{
	SOCKET fd = channel_->GetSocket();
	if (fd > 0) {
		SocketUtil::Close(fd);
	}
}

//unused
void TcpConnection::Send(std::shared_ptr<char> data, uint32_t size)
{
	if (!is_closed_) {
		mutex_.lock();
		write_buffer_->Append(data, size);
		mutex_.unlock();

		this->HandleWrite();
	}
}

//send data
void TcpConnection::Send(const char *data, uint32_t size)
{
	if (!is_closed_) {
		mutex_.lock();
		write_buffer_->Append(data, size);
		mutex_.unlock();

		this->HandleWrite();
	}
}

void TcpConnection::Disconnect()
{
	std::lock_guard<std::mutex> lock(mutex_);
	auto conn = shared_from_this();
	task_scheduler_->AddTriggerEvent([conn]() {
		conn->Close();
	});
}

#include "Global.h"

void TcpConnection::HandleRead()
{
	{
		std::lock_guard<std::mutex> lock(mutex_);
		int offset = -1;
		
		if (is_closed_) {
			std::cout << "CLOSED!!!!";
			return;
		}

		
		int ret = read_buffer_->Read(channel_->GetSocket(), offset);
		if (ret <= 0) {
			this->Close();
			return;
		}

#if NETWORK_OUTPUT
		std::cout << "Recv(" << ret << ")" << std::endl;
		//for (int i = 0; i < ret; i++)
		//{
		//	std::cout << +((unsigned char)(read_buffer_->beginWrite() - ret)[i]) << "-";
		//}std::cout << std::endl;
#endif
#if ENCRYPT_PKT
		Sleep(1000);
		size_t r = ret;
		int out_size = 0, off = 0;
		auto pkts = read_buffer_->ParsePakets(const_cast<char*>(read_buffer_->BeginWrite() - r), r);
		while(!pkts.empty())
		{
			auto pkt = pkts.front();
			pkts.pop_front();
			//std::cout << "\nDecrypting packet: " << static_cast<void*>(pkt.data) << ", size = " << pkt.size << " ";
			auto* output = decrypt(pkt.data, pkt.size, out_size);
			if (output == nullptr || out_size == 0)
			{
				std::cout << "Decryption failed - " << GetLastCryptoError();
				break;
			}
			else
			{
				auto *p = read_buffer_->Peek();
				std::cout << "PEEK=" << static_cast<void*>(p) << ", off=" << off << "\n";
				std::cout << "\nBEGIN\n" << std::string(output, out_size) << "\nEND\n";
				memcpy(p + off, output, out_size);
				off += out_size;
				free(output);
			}

			free(pkt.data);
		}

		ret = off;
		pkts.clear();
		/*int out_size = 0, tmp = 0, rtmp = ret;
		while(out_size < ret)
		{
			auto output = decrypt(read_buffer_->beginWrite() - rtmp + out_size, rtmp - out_size, tmp);
			if (output == nullptr || tmp == 0)
			{
				std::cout << "Decryption failed - " << GetLastCryptoError();
				break;
			}
			else
			{
				memcpy(read_buffer_->Peek(), output, tmp);
				out_size += tmp;
				free(output);
			}
		}*/
#else
		
#endif
	}

	if (read_cb_) {
		bool ret = read_cb_(shared_from_this(), *read_buffer_);
		if (false == ret) {
			std::lock_guard<std::mutex> lock(mutex_);
			this->Close();
		}
	}
}

void TcpConnection::HandleWrite()
{
	if (is_closed_) {
		return;
	}
	
	//std::lock_guard<std::mutex> lock(mutex_);
	if (!mutex_.try_lock()) {
		return;
	}

	int ret = 0;
	bool empty = false;
	do
	{
		ret = write_buffer_->Send(channel_->GetSocket());
		if (ret < 0) {
			this->Close();
			mutex_.unlock();
			return;
		}
		empty = write_buffer_->IsEmpty();
	} while (0);

	if (empty) {
		if (channel_->IsWriting()) {
			channel_->DisableWriting();
			task_scheduler_->UpdateChannel(channel_);
		}
	}
	else if(!channel_->IsWriting()) {
		channel_->EnableWriting();
		task_scheduler_->UpdateChannel(channel_);
	}

	mutex_.unlock();
}

void TcpConnection::Close()
{
	if (!is_closed_) {
		is_closed_ = true;
		task_scheduler_->RemoveChannel(channel_);

		if (close_cb_) {
			close_cb_(shared_from_this());
		}			

		if (disconnect_cb_) {
			disconnect_cb_(shared_from_this());
		}	
	}
}

void TcpConnection::HandleClose()
{
	std::lock_guard<std::mutex> lock(mutex_);
	this->Close();
}

void TcpConnection::HandleError()
{
	std::lock_guard<std::mutex> lock(mutex_);
	this->Close();
}
