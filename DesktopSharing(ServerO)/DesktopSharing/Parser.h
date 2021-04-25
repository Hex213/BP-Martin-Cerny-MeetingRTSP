#pragma once
static class Parser
{
public:
	static int findInPtr(char* data, size_t data_size, char* pattern, size_t pattern_size, size_t offset = 0);
	static void ParseData(char* data, size_t size);
	static unsigned char* GetBytesFromInt(int integer, bool isLittleEndian = true);
	static int GetIntFromBytes(char* bytes, bool isLittleEndian = true);
private:
	static bool ParseProxy(char* data, size_t size);
	static bool ParseOK(char* data, size_t size);
	static bool ParseNON(char* data, size_t size);
	static bool ParsePorts(char* data, size_t size);
	static bool ParseConn(char* data, size_t size);
};

