#include "Parser.h"

#include <iostream>
#include <algorithm>

#include "Global.h"

int clientRTP_t0;
int clientRTCP_t0;
int serverRTP_t0;
int serverRTCP_t0;
int clientRTP_t1;
int clientRTCP_t1;
int serverRTP_t1;
int serverRTCP_t1;
bool confirmed = false;
bool firstSetup = false;

int Parser::findInPtr(char* data, size_t data_size, char *pattern, size_t pattern_size, size_t offset)
{
	if (pattern_size > data_size - offset)
	{
		return -1;
	}

	int c = data_size - offset - pattern_size + 1;
	int j;
	
	for (int i = offset; i < c; i++)
	{
		if (data[i] != pattern[0]) continue;
		for (j = pattern_size - 1; j >= 1 && data[i + j] == pattern[j]; j--);
		if (j == 0) return i;
	}
	return -1;
}
unsigned char* Parser::GetBytesFromInt(int integer, bool isLittleEndian)
{
	unsigned char* bytes = new unsigned char[4];
	unsigned long n = integer;

	bytes[0] = (n >> 24) & 0xFF;
	bytes[1] = (n >> 16) & 0xFF;
	bytes[2] = (n >> 8) & 0xFF;
	bytes[3] = n & 0xFF;

	if(isLittleEndian)
	{
		
	}
	
	return bytes;
}

int Parser::GetIntFromBytes(char *bytes, bool isLittleEndian)
{
	if (isLittleEndian)
	{
		ReverseBytes(bytes, 4);
	}
	return int((unsigned char)(bytes[0]) << 24 |
		(unsigned char)(bytes[1]) << 16 |
		(unsigned char)(bytes[2]) << 8 |
		(unsigned char)(bytes[3]));
}

//Co ma byt server a co klient
bool Parser::ParsePorts(char* data, size_t size)
{
	char cl0[] = "CL0=", cl1[] = "CL1=", sr0[] = "SR0=", sr1[] = "SR1=";

	std::cout << "\nParsing ports\n";
	
	auto cl0Start = findInPtr(data, size, cl0, 4); //server
	auto sr0Start = findInPtr(data, size, sr0, 4); //client
	auto cl1Start = findInPtr(data, size, cl1, 4); //server
	auto sr1Start = findInPtr(data, size, sr1, 4); //client

	std::cout << "cl0: " << cl0Start << " sr0:" << sr0Start << " cl1: " << cl1Start << " sr1:" << sr1Start << std::endl;
	
	if (cl0Start == -1 || sr0Start == -1 || cl1Start == -1 || sr1Start == -1)
	{
		return false;
	}

	bool littleEnd = true;
	char* tmp = data + cl0Start + 4;
	clientRTP_t0 = GetIntFromBytes(tmp, littleEnd); tmp += 4;
	clientRTCP_t0 = GetIntFromBytes(tmp, littleEnd);
	tmp = data + sr0Start + 4;
	serverRTP_t0 = GetIntFromBytes(tmp, littleEnd); tmp += 4;
	serverRTCP_t0 = GetIntFromBytes(tmp, littleEnd);
	tmp = data + cl1Start + 4;
	clientRTP_t1 = GetIntFromBytes(tmp, littleEnd); tmp += 4;
	clientRTCP_t1 = GetIntFromBytes(tmp, littleEnd);
	tmp = data + sr1Start + 4;
	serverRTP_t1 = GetIntFromBytes(tmp, littleEnd); tmp += 4;
	serverRTCP_t1 = GetIntFromBytes(tmp, littleEnd);

	//todo: check if is not nezmysel
	std::cout << "c0RTP=" << clientRTP_t0 << ", c0RTCP=" << clientRTCP_t0 << ", s0RTP=" << serverRTP_t0 << ", s0RTCP=" << serverRTCP_t0 << std::endl
		<< "c1RTP=" << clientRTP_t1 << ", c1RTCP=" << clientRTCP_t1 << ", s1RTP=" << serverRTP_t1 << ", s1RTCP=" << serverRTCP_t1 << std::endl;
	return true;
}

bool Parser::ParseConn(char* data, size_t size)
{
	char conf[] = "CONN";
	return findInPtr(data, size, conf, 4) != -1;
}

bool Parser::ParseProxy(char* data, size_t size)
{
	char conf[] = "PROXY";
	return findInPtr(data, size, conf, 5) != -1;
}

bool Parser::ParseOK(char* data, size_t size)
{
	char conf[] = "OK";
	return findInPtr(data, size, conf, 2) != -1;
}

bool Parser::ParseNON(char* data, size_t size)
{
	char conf[] = "NON";
	return findInPtr(data, size, conf, 3) != -1;
}

void Parser::ParseData(char* data, size_t size)
{
	std::cout << "Parsing..." << size << std::endl;

	std::cout << "\nType:\n";
	if(ParsePorts(data, size))
	{
		std::cout << "Ports\n";
	} else if(ParseProxy(data, size))
	{
		std::cout << "Proxy-";
		if(ParseConn(data, size))
		{
			std::cout << "Connection-";
		}
		if(ParseOK(data, size))
		{
			std::cout << "Ok\n";
			confirmed = true;
		}
		else if(ParseNON(data, size))
		{
			std::cout << "Non\n";
		}
		else
		{
			std::cout << "Err\n";
		}
	}
	else
	{
		std::cout << "Unknown\n";
	}
}
