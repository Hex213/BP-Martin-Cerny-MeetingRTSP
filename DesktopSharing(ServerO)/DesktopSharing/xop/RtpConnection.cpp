// PHZ
// 2018-9-30

#include "RtpConnection.h"

#include "Global.h"
#include "RtspConnection.h"
#include "net/SocketUtil.h"

using namespace std;
using namespace xop;

RtpConnection::RtpConnection(std::weak_ptr<TcpConnection> rtsp_connection)
    : rtsp_connection_(rtsp_connection)
{
	std::random_device rd;

	for(int chn=0; chn<MAX_MEDIA_CHANNEL; chn++) {
		rtpfd_[chn] = 0;
		rtcpfd_[chn] = 0;
		memset(&media_channel_info_[chn], 0, sizeof(media_channel_info_[chn]));
		media_channel_info_[chn].rtp_header.version = RTP_VERSION;
		media_channel_info_[chn].packet_seq = rd()&0xffff;
		media_channel_info_[chn].rtp_header.seq = 0; //htons(1);
		media_channel_info_[chn].rtp_header.ts = htonl(rd());
		media_channel_info_[chn].rtp_header.ssrc = htonl(rd());
	}
}

RtpConnection::~RtpConnection()
{
	for(int chn=0; chn<MAX_MEDIA_CHANNEL; chn++) {
		if(rtpfd_[chn] > 0) {
			SocketUtil::Close(rtpfd_[chn]);
		}

		if(rtcpfd_[chn] > 0) {
			SocketUtil::Close(rtcpfd_[chn]);
		}
	}
}

void SendPorts(unsigned int rtp, unsigned int rtcp)
{
	char ToSend[11];
	ToSend[0] = 'S', ToSend[1] = 'P', ToSend[10] = '\0';
	unsigned char bytes[4];
	bytes[0] = (rtp >> 24) & 0xFF;
	bytes[1] = (rtp >> 16) & 0xFF;
	bytes[2] = (rtp >> 8) & 0xFF;
	bytes[3] = rtp & 0xFF;
	memcpy_s(ToSend + 2, 8, bytes, 4);
	bytes[0] = (rtcp >> 24) & 0xFF;
	bytes[1] = (rtcp >> 16) & 0xFF;
	bytes[2] = (rtcp >> 8) & 0xFF;
	bytes[3] = rtcp & 0xFF;
	memcpy_s(ToSend + 6, 4, bytes, 4);
	std::cout << "::" << rtp << ":" << rtcp << std::endl;
	for (int i = 0; i < 11; i++)
	{
		std::cout << +ToSend[i] << "-";
	}std::cout << std::endl;
	pipe.Write(ToSend, 10);
}

int RtpConnection::GetId() const
{
	auto conn = rtsp_connection_.lock();
	if (!conn) {
		return -1;
	}
	RtspConnection *rtspConn = (RtspConnection *)conn.get();
	return rtspConn->GetId();
}

bool RtpConnection::SetupRtpOverTcp(MediaChannelId channel_id, uint16_t rtp_channel, uint16_t rtcp_channel)
{
	auto conn = rtsp_connection_.lock();
	if (!conn) {
		return false;
	}

	//SendPorts(rtp_channel, rtcp_channel);

	media_channel_info_[channel_id].rtp_channel = rtp_channel;
	media_channel_info_[channel_id].rtcp_channel = rtcp_channel;
	rtpfd_[channel_id] = conn->GetSocket();
	rtcpfd_[channel_id] = conn->GetSocket();
	media_channel_info_[channel_id].is_setup = true;
	transport_mode_ = RTP_OVER_TCP;

	return true;
}

bool allowNext = false;

bool RtpConnection::SetupRtpOverUdp(MediaChannelId channel_id, uint16_t rtp_port, uint16_t rtcp_port)
{
	auto conn = rtsp_connection_.lock();
	if (!conn) {
		return false;
	}

	if(SocketUtil::GetPeerAddr(conn->GetSocket(), &peer_addr_) < 0) {
		return false;
	}
	
	media_channel_info_[channel_id].rtp_port = rtp_port;
	media_channel_info_[channel_id].rtcp_port = rtcp_port;


	std::random_device rd;
	for (int n = 0; n <= 10; n++) {
		if (n == 10) {
			return false;
		}
        
		local_rtp_port_[channel_id] = rd() & 0xfffe;
		local_rtcp_port_[channel_id] =local_rtp_port_[channel_id] + 1;

		rtpfd_[channel_id] = ::socket(AF_INET, SOCK_DGRAM, 0);
		if(!SocketUtil::Bind(rtpfd_[channel_id], "0.0.0.0",  local_rtp_port_[channel_id])) {
			SocketUtil::Close(rtpfd_[channel_id]);
			continue;
		}

		rtcpfd_[channel_id] = ::socket(AF_INET, SOCK_DGRAM, 0);
		if(!SocketUtil::Bind(rtcpfd_[channel_id], "0.0.0.0", local_rtcp_port_[channel_id])) {
			SocketUtil::Close(rtpfd_[channel_id]);
			SocketUtil::Close(rtcpfd_[channel_id]);
			continue;
		}

		break;
	}

	SocketUtil::SetSendBufSize(rtpfd_[channel_id], 50*1024);

	peer_rtp_addr_[channel_id].sin_family = AF_INET;
	peer_rtp_addr_[channel_id].sin_addr.s_addr = peer_addr_.sin_addr.s_addr;
	peer_rtp_addr_[channel_id].sin_port = htons(media_channel_info_[channel_id].rtp_port);

	peer_rtcp_sddr_[channel_id].sin_family = AF_INET;
	peer_rtcp_sddr_[channel_id].sin_addr.s_addr = peer_addr_.sin_addr.s_addr;
	peer_rtcp_sddr_[channel_id].sin_port = htons(media_channel_info_[channel_id].rtcp_port);

	media_channel_info_[channel_id].is_setup = true;
	transport_mode_ = RTP_OVER_UDP;

	return true;
}

bool RtpConnection::SetupRtpOverMulticast(MediaChannelId channel_id, std::string ip, uint16_t port)
{
    std::random_device rd;
    for (int n = 0; n <= 10; n++) {
		if (n == 10) {
			return false;
		}
       
		local_rtp_port_[channel_id] = rd() & 0xfffe;
		rtpfd_[channel_id] = ::socket(AF_INET, SOCK_DGRAM, 0);
		if (!SocketUtil::Bind(rtpfd_[channel_id], "0.0.0.0", local_rtp_port_[channel_id])) {
			SocketUtil::Close(rtpfd_[channel_id]);
			continue;
		}

		break;
    }

	media_channel_info_[channel_id].rtp_port = port;

	peer_rtp_addr_[channel_id].sin_family = AF_INET;
	peer_rtp_addr_[channel_id].sin_addr.s_addr = inet_addr(ip.c_str());
	peer_rtp_addr_[channel_id].sin_port = htons(port);

	media_channel_info_[channel_id].is_setup = true;
	transport_mode_ = RTP_OVER_MULTICAST;
	is_multicast_ = true;
	return true;
}

void RtpConnection::Play()
{
	for(int chn=0; chn<MAX_MEDIA_CHANNEL; chn++) {
		if (media_channel_info_[chn].is_setup) {
			media_channel_info_[chn].is_play = true;
		}
	}
}

void RtpConnection::Record()
{
	for (int chn=0; chn<MAX_MEDIA_CHANNEL; chn++) {
		if (media_channel_info_[chn].is_setup) {
			media_channel_info_[chn].is_record = true;
			media_channel_info_[chn].is_play = true;
		}
	}
}

void RtpConnection::Teardown()
{
	if(!is_closed_) {
		is_closed_ = true;
		for(int chn=0; chn<MAX_MEDIA_CHANNEL; chn++) {
			media_channel_info_[chn].is_play = false;
			media_channel_info_[chn].is_record = false;
		}
	}
}

string RtpConnection::GetMulticastIp(MediaChannelId channel_id) const
{
	return std::string(inet_ntoa(peer_rtp_addr_[channel_id].sin_addr));
}

string RtpConnection::GetRtpInfo(const std::string& rtsp_url)
{
	char buf[2048] = { 0 };
	snprintf(buf, 1024, "RTP-Info: ");

	int num_channel = 0;

	auto time_point = chrono::time_point_cast<chrono::milliseconds>(chrono::steady_clock::now());
	auto ts = time_point.time_since_epoch().count();
	for (int chn = 0; chn<MAX_MEDIA_CHANNEL; chn++) {
		uint32_t rtpTime = (uint32_t)(ts*media_channel_info_[chn].clock_rate / 1000);
		if (media_channel_info_[chn].is_setup) {
			if (num_channel != 0) {
				snprintf(buf + strlen(buf), sizeof(buf) - strlen(buf), ",");
			}			

			snprintf(buf + strlen(buf), sizeof(buf) - strlen(buf),
					"url=%s/track%d;seq=0;rtptime=%u",
					rtsp_url.c_str(), chn, rtpTime);
			num_channel++;
		}
	}

	return std::string(buf);
}

void RtpConnection::SetFrameType(uint8_t frame_type)
{
	frame_type_ = frame_type;
	if(!has_key_frame_ && (frame_type == 0 || frame_type == VIDEO_FRAME_I)) {
		has_key_frame_ = true;
	}
}

void RtpConnection::SetRtpHeader(MediaChannelId channel_id, RtpPacket pkt)
{
	if((media_channel_info_[channel_id].is_play || media_channel_info_[channel_id].is_record) && has_key_frame_) {
		media_channel_info_[channel_id].rtp_header.marker = pkt.last;
		media_channel_info_[channel_id].rtp_header.ts = htonl(pkt.timestamp);
		media_channel_info_[channel_id].rtp_header.seq = htons(media_channel_info_[channel_id].packet_seq++);
		memcpy(pkt.data.get()+4, &media_channel_info_[channel_id].rtp_header, RTP_HEADER_SIZE);
	}
}

#if ENCRYPT_PKT
extern std::string key;
extern std::string iv;
#endif

int RtpConnection::SendRtpPacket(MediaChannelId channel_id, RtpPacket pkt)
{
	if (is_closed_) {
		return -1;
	}
   
	auto conn = rtsp_connection_.lock();
	if (!conn) {
		return -1;
	}
	RtspConnection *rtsp_conn = (RtspConnection *)conn.get();
	bool ret = rtsp_conn->task_scheduler_->AddTriggerEvent([this, channel_id, pkt] {
		this->SetFrameType(pkt.type);
		this->SetRtpHeader(channel_id, pkt);
		if((media_channel_info_[channel_id].is_play || media_channel_info_[channel_id].is_record) && has_key_frame_ ) {            
			if(transport_mode_ == RTP_OVER_TCP) {
				SendRtpOverTcp(channel_id, pkt);
			}
			else {
				SendRtpOverUdp(channel_id, pkt);
			}
                   
			//media_channel_info_[channel_id].octetCount  += pkt.size;
			//media_channel_info_[channel_id].packetCount += 1;
		}
	});

	return ret ? 0 : -1;
}

//#include "Global.h"

int RtpConnection::SendRtpOverTcp(MediaChannelId channel_id, RtpPacket pkt)
{
	auto conn = rtsp_connection_.lock();
	if (!conn) {
		return -1;
	}

	//Format paket
	uint8_t* rtpPktPtr = pkt.data.get();
	rtpPktPtr[0] = '$';
	rtpPktPtr[1] = (char)media_channel_info_[channel_id].rtp_channel;
	rtpPktPtr[2] = (char)(((pkt.size-4)&0xFF00)>>8);
	rtpPktPtr[3] = (char)((pkt.size -4)&0xFF);

#if NETWORK_OUTPUT
	std::cout << "SendTCP("<< pkt.size << ")" << std::endl;
#endif
	
	conn->Send((char*)rtpPktPtr, pkt.size);
	return pkt.size;
}

int RtpConnection::SendRtpOverUdp(MediaChannelId channel_id, RtpPacket pkt)
{
	//media_channel_info_[channel_id].octetCount  += pktSize;
	//media_channel_info_[channel_id].packetCount += 1;
	int ret = -1;
	
	//Encryption for data(udp)
#if ENCRYPT_PKT
	HexPacket* hpkt = new HexPacket(reinterpret_cast<char*>(pkt.data.get() + 4), pkt.size - 4, 0, Packet_type::Encrypt);
	hpkt->EncryptPacket();
	size_t send_data_bytes = 0;
	const auto* send_data = hpkt->GetDataToSend(send_data_bytes);

#if NETWORK_OUTPUT
	std::cout << "SendUDP(" << send_data_bytes << ")/clear(" << pkt.size - 4 << ")" << std::endl;
#endif
	ret = sendto(rtpfd_[channel_id], send_data, send_data_bytes, 0,
		(struct sockaddr*)&(peer_rtp_addr_[channel_id]),
		sizeof(struct sockaddr_in));
	
	delete[] send_data;
	free(hpkt);
#else
#if NETWORK_OUTPUT
	std::cout << "SendUDP(" << pkt.size - 4 << ")" << std::endl;
#endif
	ret = sendto(rtpfd_[channel_id], (const char*)pkt.data.get()+4, pkt.size-4, 0, 
					(struct sockaddr *)&(peer_rtp_addr_[channel_id]),
					sizeof(struct sockaddr_in));
#endif

	if(ret < 0) {        
		Teardown();
		return -1;
	}
	
	return ret;
}
