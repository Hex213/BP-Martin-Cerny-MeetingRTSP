// PHZ
// 2018-5-16

#if defined(WIN32) || defined(_WIN32)
#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif
#endif

#include "RtspMessage.h"

#include <iostream>
#include "NamedPipe.h"

#include "AesGcm.h"
#include "media.h"
#include "Parser.h"

#include "Global.h"

using namespace std;
using namespace xop;

extern NamedPipe pipe;
extern int clientRTP_t0;
extern int clientRTCP_t0;
extern int serverRTP_t0;
extern int serverRTCP_t0;
extern int clientRTP_t1;
extern int clientRTCP_t1;
extern int serverRTP_t1;
extern int serverRTCP_t1;
int real_rtp_client_portT0 = -1;
int real_rtcp_client_portT0 = -1;
int real_rtp_client_portT1 = -1;
int real_rtcp_client_portT1 = -1;
bool oneSet = true;

int ReturnInt(string s)
{
	if (s.length() != 4)
	{
		throw new std::invalid_argument("string must have 4 bytes");
	}
	char* t = const_cast<char*>(s.c_str());
	int a;
	memcpy(&a, t, sizeof(int));
	return a;
}

void RewriteHCPORT(char* start, size_t clear)
{
	//std::cout << "\nStart:" << (void*)start << "-End:" << (void*)(start + clear) << std::endl;
	for (int i = 0; i <= clear; i++)
	{
		start[i] = ' ';
	}
}

int extract_port(string& data, bool forceOffOutput = false)
{
	if (_DEBUG && !forceOffOutput) std::cout << "L0:" << data.length() << std::endl;
	auto start = data.find("-");
	if (start != string::npos)
	{
		if (_DEBUG && !forceOffOutput) std::cout << "-S:" << start << std::endl;
		data = start != 0 ? data.erase(0, start + 1) : data.erase(0, 1);
		if (_DEBUG && !forceOffOutput) std::cout << "-L1:" << data.length() << std::endl;
		auto out = Parser::GetIntFromBytes(const_cast<char*>(data.c_str()));
		data = data.erase(0, 4);
		if (_DEBUG && !forceOffOutput) std::cout << "-L2:" << data.length() << std::endl;
		if (_DEBUG && !forceOffOutput) std::cout << "ret:" << out << std::endl;
		return out;
	} else
	{
		throw /*new*/ exception("Cannot find \"-\"!");
	}
	
	return INT_MIN;
}

bool RtspRequest::ParseRequest(BufferReader *buffer)
{
	if(buffer->Peek()[0] == '$') {
		method_ = RTCP;
		return true;
	}

#if USE_PROXY
	auto f = buffer->Peek();
	auto first = buffer->FindFirstCrlf();
	std::string s = std::string(f, first - f);

	if(s.find("HCPORT") != std::string::npos && oneSet)
	{
		auto remsize = s.length();
		if constexpr (_DEBUG) std::cout << "HCPORT\n";
		s.erase(0, 6);
		char hcport[] = "HCPORT ";
		const int size = 7 + 4 + 4 + 4 + 4;
		int dynSize = size, index = 0;
		char portsReq[size];
		portsReq[15] = '\0';
		memcpy_s(portsReq, size, hcport, 7); index += 7;
		dynSize -= 7;
		//if(s[0] != '-')
		//{
		//}
		//s.erase(0, 1);
		real_rtp_client_portT0 = extract_port(s, true);//s.substr(0, 4);
		real_rtcp_client_portT0 = extract_port(s, true);
		real_rtp_client_portT1 = extract_port(s, true);
		real_rtcp_client_portT1 = extract_port(s, true);
		//char* pEnd = tmp.c_str();
		//real_rtp_client_portT0 = ReturnInt(tmp);
		auto t0rtp = Parser::GetBytesFromInt(real_rtp_client_portT0);
		auto t0rtcp = Parser::GetBytesFromInt(real_rtcp_client_portT0);
		auto t1rtp = Parser::GetBytesFromInt(real_rtp_client_portT1);
		auto t1rtcp = Parser::GetBytesFromInt(real_rtcp_client_portT1);

		memcpy_s(portsReq + index, dynSize, t0rtp, 4); dynSize -= 4; index += 4;
		memcpy_s(portsReq + index, dynSize, t0rtcp, 4); dynSize -= 4; index += 4;
		memcpy_s(portsReq + index, dynSize, t1rtp, 4); dynSize -= 4; index += 4;
		memcpy_s(portsReq + index, dynSize, t1rtcp, 4); dynSize -= 4; index += 4;

		delete[] t0rtp;
		delete[] t0rtcp;
		delete[] t1rtp;
		delete[] t1rtcp;
		
		std::cout << portsReq << "::rtpT0=" << real_rtp_client_portT0 << "::rtcpT0=" << real_rtcp_client_portT0
			<< "::rtpT1=" << real_rtp_client_portT1 << "::rtcpT1=" << real_rtcp_client_portT1 << std::endl;
		RewriteHCPORT(f, remsize + 1);
		
		pipe.Write(portsReq, size);
		oneSet = false;
	}
#endif
	
    bool ret = true;
	while(true) {
		if(state_ == kParseRequestLine) {
			const char* firstCrlf = buffer->FindFirstCrlf();
			if(firstCrlf != nullptr)
			{
				ret = ParseRequestLine(buffer->Peek(), firstCrlf);
				buffer->RetrieveUntil(firstCrlf + 2);
			}

			if (state_ == kParseHeadersLine) {
				continue;
			}             
			else {
				break;
			}           
		}
		else if(state_ == kParseHeadersLine) {
			const char* lastCrlf = buffer->FindLastCrlf();
			if(lastCrlf != nullptr) {
				ret = ParseHeadersLine(buffer->Peek(), lastCrlf);
				buffer->RetrieveUntil(lastCrlf + 2);
			}
			break;
		}
		else if(state_ == kGotAll) {
			buffer->RetrieveAll();
			return true;
		}
	}

	return ret;
}

char* ChangeAddress(char *url, size_t size)
{
	if (url == nullptr) return nullptr;
	std::string surl = std::string(url);

	auto posRtsp = surl.find("rtsp://");
	auto posPort = surl.find(":", posRtsp + strlen("rtsp://"));
	auto posEndPort = surl.find("/", posPort);
	surl.replace(posRtsp + strlen("rtsp://"), posPort - (posRtsp + strlen("rtsp://")), "127.0.0.1");
	surl.replace(posPort + 1, posEndPort - posPort - 1, "8554");

	strcpy_s(url, 512, surl.c_str());
}

bool RtspRequest::ParseRequestLine(const char* begin, const char* end)
{
	string message(begin, end);
	char method[64] = {0};
	char url[512] = {0};
	char version[64] = {0};

	//std::cout << (void*)begin << std::endl << (void*)end << " Count:" << end-begin << std::endl;
	//std::cout << "Start\n" << message;
	for (auto ch : message)
	{
		std::cout << ch;
	}std::cout << std::endl;
	
	do
	{
		if (message[0] == ' ' && message.length() > 1)
		{
			message.erase(0, 1);
		}
		else
		{
			break;
		}
	}
	while (true);

	//std::cout << "End\n" << message;
	
	if(sscanf(message.c_str(), "%s %s %s", method, url, version) != 3) {
		//std::cout << "FALSE";
		return true; 
	}

#if USE_PROXY
	ChangeAddress(url, 512);
#endif

	string method_str(method);
	if(method_str == "OPTIONS") {
		method_ = OPTIONS;
	}
	else if(method_str == "DESCRIBE") {
		method_ = DESCRIBE;
	}
	else if(method_str == "SETUP") {
		method_ = SETUP;
	}
	else if(method_str == "PLAY") {
		method_ = PLAY;
	}
	else if(method_str == "TEARDOWN") {
		method_ = TEARDOWN;
	}
	else if(method_str == "GET_PARAMETER") {
		method_ = GET_PARAMETER;
	}
	else {
		method_ = NONE;
		return false;
	}

	if(strncmp(url, "rtsp://", 7) != 0) {
		return false;
	}

	// parse url
	uint16_t port = 0;
	char ip[64] = {0};
	char suffix[64] = {0};

	if(sscanf(url+7, "%[^:]:%hu/%s", ip, &port, suffix) == 3) {

	}
	else if(sscanf(url+7, "%[^/]/%s", ip, suffix) == 2) {
		port = 554;
	}
	else {
		std::cout << "\nurl=" << url
			<< "\nip=" << ip
			<< "\nport=" << port << std::endl;
		return false;
	}
	std::cout << "\nurl=" << url
		<< "\nip=" << ip
		<< "\nport=" << port << std::endl;
	request_line_param_.emplace("url", make_pair(string(url), 0));
	request_line_param_.emplace("url_ip", make_pair(string(ip), 0));
	request_line_param_.emplace("url_port", make_pair("", (uint32_t)port));
	request_line_param_.emplace("url_suffix", make_pair(string(suffix), 0));
	request_line_param_.emplace("version", make_pair(string(version), 0));
	request_line_param_.emplace("method", make_pair(move(method_str), 0));

	state_ = kParseHeadersLine;
	return true;
}

bool RtspRequest::ParseHeadersLine(const char* begin, const char* end)
{
	string message(begin, end);
	//std::cout << "Parsing Header: " << message << "\n";
	if(!ParseCSeq(message)) {
		if (header_line_param_.find("cseq") == header_line_param_.end()) {
			return false;
		} 
	}

	if (method_ == DESCRIBE || method_ == SETUP || method_ == PLAY) {
		ParseAuthorization(message);
	}

	if(method_ == OPTIONS) {
		state_ = kGotAll;
		return true;
	}

	if(method_ == DESCRIBE) {
		if(ParseAccept(message)) {
			state_ = kGotAll;
		}
		return true;
	}

	if(method_ == SETUP) {
		if(ParseTransport(message)) {
			ParseMediaChannel(message);
			state_ = kGotAll;
		}

		return true;
	}

	if(method_ == PLAY) {
		if(ParseSessionId(message)) {
			state_ = kGotAll;
		}
		return true;
	}

	if(method_ == TEARDOWN) {
		state_ = kGotAll;
		return true;
	}

	if(method_ == GET_PARAMETER) {
		state_ = kGotAll;
		return true;
	}

    return true;
}

bool RtspRequest::ParseCSeq(std::string& message)
{
	const std::size_t pos = message.find("CSeq");
	if (pos != std::string::npos) {
		uint32_t cseq = 0;
		sscanf(message.c_str()+pos, "%*[^:]: %u", &cseq);
		header_line_param_.emplace("cseq", make_pair("", cseq));
		return true;
	}

    return false;
}

bool RtspRequest::ParseAccept(std::string& message)
{
	if ((message.rfind("Accept")==std::string::npos)
		|| (message.rfind("sdp")==std::string::npos)) {
		return false;
	}

	return true;
}

bool fi = false;

bool RtspRequest::ParseTransport(std::string& message)
{
	//std::cout << "Parsing: " << message << "\n";
	std::size_t pos = message.find("Transport");
	if(pos != std::string::npos) {
		if((pos=message.find("RTP/AVP/TCP")) != std::string::npos) {
			transport_ = RTP_OVER_TCP;
			uint16_t rtpChannel = 0, rtcpChannel = 0;
			if (sscanf(message.c_str() + pos, "%*[^;];%*[^;];%*[^=]=%hu-%hu", &rtpChannel, &rtcpChannel) != 2) {
				return false;
			}
			header_line_param_.emplace("rtp_channel", make_pair("", rtpChannel));
			header_line_param_.emplace("rtcp_channel", make_pair("", rtcpChannel));
		}
		else if((pos=message.find("RTP/AVP")) != std::string::npos) {
			uint16_t rtp_port = 0, rtcpPort = 0;
			if(((message.find("unicast", pos)) != std::string::npos)) {
				transport_ = RTP_OVER_UDP;
				if(sscanf(message.c_str()+pos, "%*[^;];%*[^;];%*[^=]=%hu-%hu",
						&rtp_port, &rtcpPort) != 2)
				{
					return false;
				}
#if USE_PROXY
				if (!fi)
				{
					rtp_port = clientRTP_t0;
					rtcpPort = clientRTCP_t0;
					fi = !fi;
				}
				else
				{
					rtp_port = clientRTP_t1;
					rtcpPort = clientRTCP_t1;
				}
				std::cout << "New Client Ports: " << rtp_port << "-" << rtcpPort << std::endl;
#endif	
			}
			else if((message.find("multicast", pos)) != std::string::npos) {
				transport_ = RTP_OVER_MULTICAST;
			}
			else {
				return false;
			}

			header_line_param_.emplace("rtp_port", make_pair("", rtp_port));
			header_line_param_.emplace("rtcp_port", make_pair("", rtcpPort));
		}
		else {
			return false;
		}

		return true;
	}

    return false;
}

bool RtspRequest::ParseSessionId(std::string& message)
{
	const std::size_t pos = message.find("Session");
	if (pos != std::string::npos) {
		uint32_t session_id = 0;
		if (sscanf(message.c_str() + pos, "%*[^:]: %u", &session_id) != 1) {
			return false;
		}        
		return true;
	}

	return false;
}

bool RtspRequest::ParseMediaChannel(std::string& message)
{
	channel_id_ = channel_0;

	const auto iter = request_line_param_.find("url");
	if(iter != request_line_param_.end()) {
		const std::size_t pos = iter->second.first.find("track1");
		if (pos != std::string::npos) {
			channel_id_ = channel_1;
		}       
	}

	return true;
}

bool RtspRequest::ParseAuthorization(std::string& message)
{	
	std::size_t pos = message.find("Authorization");
	if (pos != std::string::npos) {
		if ((pos = message.find("response=")) != std::string::npos) {
			auth_response_ = message.substr(pos + 10, 32);
			if (auth_response_.size() == 32) {
				return true;
			}
		}
	}

	auth_response_.clear();
	return false;
}

uint32_t RtspRequest::GetCSeq() const
{
	uint32_t cseq = 0;
	const auto iter = header_line_param_.find("cseq");
	if(iter != header_line_param_.end()) {
		cseq = iter->second.second;
	}

	return cseq;
}

std::string RtspRequest::GetIp() const
{
	const auto iter = request_line_param_.find("url_ip");
	if(iter != request_line_param_.end()) {
		return iter->second.first;
	}

	return "";
}

std::string RtspRequest::GetRtspUrl() const
{
	const auto iter = request_line_param_.find("url");
	if(iter != request_line_param_.end()) {
		return iter->second.first;
	}

	return "";
}

std::string RtspRequest::GetRtspUrlSuffix() const
{
	const auto iter = request_line_param_.find("url_suffix");
	if(iter != request_line_param_.end()) {
		return iter->second.first;
	}

	return "";
}

std::string RtspRequest::GetAuthResponse() const
{
	return auth_response_;
}

uint8_t RtspRequest::GetRtpChannel() const
{
	const auto iter = header_line_param_.find("rtp_channel");
	if(iter != header_line_param_.end()) {
		return iter->second.second;
	}

	return 0;
}

uint8_t RtspRequest::GetRtcpChannel() const
{
	const auto iter = header_line_param_.find("rtcp_channel");
	if(iter != header_line_param_.end()) {
		return iter->second.second;
	}

	return 0;
}

uint16_t RtspRequest::GetRtpPort() const
{
	const auto iter = header_line_param_.find("rtp_port");
	if(iter != header_line_param_.end()) {
		return iter->second.second;
	}

	return 0;
}

uint16_t RtspRequest::GetRtcpPort() const
{
	const auto iter = header_line_param_.find("rtcp_port");
	if(iter != header_line_param_.end()) {
		return iter->second.second;
	}

	return 0;
}

#include "Global.h"//ENCRYPTION------------------------------------------------------------

int returnValue(const char* buf, int buf_size, char* res)
{
#if ENCRYPT_WHENBUILD
	return encryptAndClear(buf, buf_size, res);
#else
	return encryptAndClearWO(buf, buf_size, res);
#endif
}

int RtspRequest::BuildOptionRes(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);

	snprintf((char*)ptr, buf_size, "RTSP/1.0 200 OK\r\n"
		"CSeq: %u\r\n"
		"Public: OPTIONS, DESCRIBE, SETUP, TEARDOWN, PLAY\r\n"
		"\r\n",
		this->GetCSeq());

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildDescribeRes(const char* buf, int buf_size, const char* sdp)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %u\r\n"
			"Content-Length: %d\r\n"
			"Content-Type: application/sdp\r\n"
			"\r\n"
			"%s",
			this->GetCSeq(), 
			(int)strlen(sdp), 
			sdp);
	
	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildSetupMulticastRes(const char* buf, int buf_size, const char* multicast_ip, uint16_t port, uint32_t session_id)
{	
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %u\r\n"
			"Transport: RTP/AVP;multicast;destination=%s;source=%s;port=%u-0;ttl=255\r\n"
			"Session: %u\r\n"
			"\r\n",
			this->GetCSeq(),
			multicast_ip,
			this->GetIp().c_str(),
			port,
			session_id);

	return returnValue(buf, buf_size, res);
}

bool track1 = false;

int RtspRequest::BuildSetupUdpRes(const char* buf, int buf_size, uint16_t rtp_chn, uint16_t rtcp_chn, uint32_t session_id)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	//if(_DEBUG) std::cout << "PORTY PRE SERVER: " << rtp_chn << " - " << serverRTP_t0 << std::endl << rtcp_chn << " - " << serverRTCP_t0 << std::endl;
	int rtp, rtcp;
	
#if USE_PROXY
	if (!track1)
	{
		rtp_chn = serverRTP_t0;
		rtcp_chn = serverRTCP_t0;
		rtp = real_rtp_client_portT0;
		rtcp = real_rtcp_client_portT0;
		track1 = !track1;
	}else
	{
		rtp_chn = serverRTP_t1;
		rtcp_chn = serverRTCP_t1;
		rtp = real_rtp_client_portT1;
		rtcp = real_rtcp_client_portT1;
	}
	std::cout << "SUMMARY:" <<
		"\nrtp_ch: " << rtp_chn <<
		"\nrtcp_ch: " << rtcp_chn <<
		"\nrtp_client: " << rtp <<
		"\nrtcp_client: " << rtcp << std::endl;
#else
	rtp = this->GetRtpPort();
	rtcp = this->GetRtcpPort();
#endif
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %u\r\n"
			"Transport: RTP/AVP;unicast;client_port=%hu-%hu;server_port=%hu-%hu\r\n"
			"Session: %u\r\n"
			"\r\n",
			this->GetCSeq(),
			rtp,
			rtcp,
			rtp_chn, 
			rtcp_chn,
			session_id);

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildSetupTcpRes(const char* buf, int buf_size, uint16_t rtp_chn, uint16_t rtcp_chn, uint32_t session_id)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %u\r\n"
			"Transport: RTP/AVP/TCP;unicast;interleaved=%d-%d\r\n"
			"Session: %u\r\n"
			"\r\n",
			this->GetCSeq(),
			rtp_chn, rtcp_chn,
			session_id);

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildPlayRes(const char* buf, int buf_size, const char* rtpInfo, uint32_t session_id)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %d\r\n"
			"Range: npt=0.000-\r\n"
			"Session: %u; timeout=60\r\n",
			this->GetCSeq(),
			session_id);

	if (rtpInfo != nullptr) {
		snprintf((char*)ptr + strlen(ptr), buf_size - strlen(ptr), "%s\r\n", rtpInfo);
	}

	snprintf((char*)ptr + strlen(ptr), buf_size - strlen(ptr), "\r\n");
	
	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildTeardownRes(const char* buf, int buf_size, uint32_t session_id)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %d\r\n"
			"Session: %u\r\n"
			"\r\n",
			this->GetCSeq(),
			session_id);

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildGetParamterRes(const char* buf, int buf_size, uint32_t session_id)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 200 OK\r\n"
			"CSeq: %d\r\n"
			"Session: %u\r\n"
			"\r\n",
			this->GetCSeq(),
			session_id);

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildNotFoundRes(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 404 Stream Not Found\r\n"
			"CSeq: %u\r\n"
			"\r\n",
			this->GetCSeq());

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildServerErrorRes(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 500 Internal Server Error\r\n"
			"CSeq: %u\r\n"
			"\r\n",
			this->GetCSeq());

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildUnsupportedRes(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 461 Unsupported transport\r\n"
			"CSeq: %d\r\n"
			"\r\n",
			this->GetCSeq());

	return returnValue(buf, buf_size, res);
}

int RtspRequest::BuildUnauthorizedRes(const char* buf, int buf_size, const char* realm, const char* nonce)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size,
			"RTSP/1.0 401 Unauthorized\r\n"
			"CSeq: %d\r\n"
			"WWW-Authenticate: Digest realm=\"%s\", nonce=\"%s\"\r\n"
			"\r\n",
			this->GetCSeq(),
			realm,
			nonce);

	return returnValue(buf, buf_size, res);
}

bool RtspResponse::ParseResponse(xop::BufferReader *buffer)
{
	if (strstr(buffer->Peek(), "\r\n\r\n") != NULL) {
		if (strstr(buffer->Peek(), "OK") == NULL) {
			return false;
		}

		char* ptr = strstr(buffer->Peek(), "Session");
		if (ptr != NULL) {
			char session_id[50] = {0};
			if (sscanf(ptr, "%*[^:]: %s", session_id) == 1)
				session_ = session_id;
		}

		cseq_++;
		buffer->RetrieveUntil("\r\n\r\n");
	}

	return true;
}

int RtspResponse::BuildOptionReq(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size, //change buf <-> res
			"OPTIONS %s RTSP/1.0\r\n"
			"CSeq: %u\r\n"
			"User-Agent: %s\r\n"
			"\r\n",
			rtsp_url_.c_str(),
			this->GetCSeq() + 1,
			user_agent_.c_str());

	method_ = OPTIONS;
	return returnValue(buf, buf_size, res);
}

int RtspResponse::BuildAnnounceReq(const char* buf, int buf_size, const char *sdp)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size, //change buf <-> res
			"ANNOUNCE %s RTSP/1.0\r\n"
			"Content-Type: application/sdp\r\n"
			"CSeq: %u\r\n"
			"User-Agent: %s\r\n"
			"Session: %s\r\n"
			"Content-Length: %d\r\n"
			"\r\n"
			"%s",
			rtsp_url_.c_str(),
			this->GetCSeq() + 1, 
			user_agent_.c_str(),
			this->GetSession().c_str(),
			(int)strlen(sdp),
			sdp);

	method_ = ANNOUNCE;
	return returnValue(buf, buf_size, res);
}

int RtspResponse::BuildDescribeReq(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size, //change buf <-> res
			"DESCRIBE %s RTSP/1.0\r\n"
			"CSeq: %u\r\n"
			"Accept: application/sdp\r\n"
			"User-Agent: %s\r\n"
			"\r\n",
			rtsp_url_.c_str(),
			this->GetCSeq() + 1,
			user_agent_.c_str());

	method_ = DESCRIBE;
	return returnValue(buf, buf_size, res);
}

int RtspResponse::BuildSetupTcpReq(const char* buf, int buf_size, int trackId)
{
	int interleaved[2] = { 0, 1 };
	if (trackId == 1) {
		interleaved[0] = 2;
		interleaved[1] = 3;
	}

	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size, //change buf <-> res
			"SETUP %s/track%d RTSP/1.0\r\n"
			"Transport: RTP/AVP/TCP;unicast;mode=record;interleaved=%d-%d\r\n"
			"CSeq: %u\r\n"
			"User-Agent: %s\r\n"
			"Session: %s\r\n"
			"\r\n",
			rtsp_url_.c_str(), 
			trackId,
			interleaved[0],
			interleaved[1],
			this->GetCSeq() + 1,
			user_agent_.c_str(), 
			this->GetSession().c_str());

	method_ = SETUP;
	return returnValue(buf, buf_size, res);
}

int RtspResponse::BuildRecordReq(const char* buf, int buf_size)
{
	//create encryption buffer if is needed
	auto* res = ENCRYPT_WHENBUILD == 1 ? new char[buf_size] : nullptr;
	//create universal ptr (enrypt/clear)
	const auto* ptr = ENCRYPT_WHENBUILD == 1 ? res : buf;
	//Clear buffers
	clear_buffers(buf, buf_size, res);
	
	snprintf((char*)ptr, buf_size, //change buf <-> res
			"RECORD %s RTSP/1.0\r\n"
			"Range: npt=0.000-\r\n"
			"CSeq: %u\r\n"
			"User-Agent: %s\r\n"
			"Session: %s\r\n"
			"\r\n",
			rtsp_url_.c_str(), 
			this->GetCSeq() + 1,
			user_agent_.c_str(), 
			this->GetSession().c_str());

	method_ = RECORD;
	return returnValue(buf, buf_size, res);
}
