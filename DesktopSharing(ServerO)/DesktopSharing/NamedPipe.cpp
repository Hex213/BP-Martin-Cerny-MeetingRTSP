#include "NamedPipe.h"

#include <iostream>

extern bool confirmed;

void NamedPipe::Init(std::string name, bool waitForConnect)
{
	std::string param = "\\\\.\\pipe\\" + name.substr() + "in";
	std::cout << "pipein: " << param << std::endl;
	lpszPipename1 = const_cast<LPSTR>(param.c_str());
	param.clear();
	param = "\\\\.\\pipe\\" + name.substr() + "out";
	std::cout << "pipeout: " << param << std::endl;
	lpszPipename2 = const_cast<LPSTR>(param.c_str());

	DWORD cbWritten;
	DWORD dwBytesToWrite = (DWORD)strlen(buf);
	
	BOOL Write_St = TRUE;

	Finished = FALSE;

	int count = 0;
	
	std::cout << "Create pipe in...";// << lpszPipename1 << " write\n";
	do
	{
		hPipe1 = CreateFile(lpszPipename1, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
		if((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE)) {
			param = "\\\\.\\pipe\\" + name.substr() + "in";
			lpszPipename1 = const_cast<LPSTR>(param.c_str());
			printf("Could not open the pipe  - (error %d)\n", GetLastError());
			Sleep(1000);
			count++;
		}
		if(count > 10)
		{
			exit(1);
		}
	}
	while (waitForConnect == true && (hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE));
	count = 0;
	std::cout << "OK\nCreate pipe out...";
	do
	{
		hPipe2 = CreateFile(lpszPipename2, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
		if ((hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE)) {
			param = "\\\\.\\pipe\\" + name.substr() + "out";
			lpszPipename2 = const_cast<LPSTR>(param.c_str());
			printf("Could not open the pipe  - (error %d)\n", GetLastError());
			Sleep(1000);
			count++;
		}
		if (count > 10)
		{
			exit(1);
		}
	}
	while (waitForConnect == true && (hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE));
	std::cout << "OK";
	if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE) || (hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE))
	{
		printf("Could not open the pipe  - (error %d)\n", GetLastError());
	}
}

void NamedPipe::Write(char* buff, const size_t count)
{
	std::cout << "\nP(w): ";
	memcpy_s(buf, 100, buff, count);
	WriteFile(hPipe1, buf, count, &cbWritten, NULL);
	std::cout << buf << "(" << count << ")" << std::endl;
	memset(buf, 0xCC, 100);
}

void NamedPipe::WaitForConfirm()
{
	while(!confirmed)
	{
		Sleep(10);
	}
	
	confirmed = !confirmed;
}

void NamedPipe::StartListening()
{
	std::cout << "Starting listening...";
	hThread = CreateThread(NULL, 0, &NET_RvThr, 0, 0, NULL);
	std::cout << "OK\n";
}

void NamedPipe::Close()
{
	CloseHandle(hPipe1);
	CloseHandle(hPipe2);
	Finished = TRUE;
}
//
//unsigned long __stdcall NamedPipe::NET_RvThr(void* pParam)
//{
//	RecvFunc();
//};