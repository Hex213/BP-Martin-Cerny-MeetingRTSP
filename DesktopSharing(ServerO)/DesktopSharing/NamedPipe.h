#pragma once
#include <functional>
#include <process.h>
#include <stdio.h>
#include <string>
#include <thread>
#include <Windows.h>

void RecvFunc();
DWORD WINAPI ThreadProc();
extern HANDLE hPipe1, hPipe2;
extern BOOL Finished;

static unsigned long __stdcall NET_RvThr(void* pParam)
{
	RecvFunc();
	return 0;
};

class NamedPipe
{
private:
	char buf[100];
	LPTSTR lpszPipename1;
	LPTSTR lpszPipename2;
	DWORD cbWritten;
	DWORD threadId;
	HANDLE hThread = NULL;

public:
	NamedPipe() {}

	NamedPipe(std::string name, bool waitForConnect)
	{
		Init(name, waitForConnect);
	}

	void Init(std::string name, bool waitForConnect);
	void Write(char* buff, const size_t count);
	void WaitForConfirm();
	void StartListening();
	void Close();
};