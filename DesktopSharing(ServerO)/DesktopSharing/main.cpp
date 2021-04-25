#include <charconv>

#include "ScreenLive.h"
#include "MainWindow.h"
#include <cstdlib>
#include "NamedPipe.h"

#include "AesGcm.h"
#include "csrc/getopt.h"

#define TESTING 0

#define RTSP_PUSHER_TEST "rtsp://127.0.0.1:554/test" 
#define RTMP_PUSHER_TEST "rtmp://127.0.0.1:1935/live/02"  

//DWORD WINAPI ThreadProc();
//HANDLE hPipe1, hPipe2;
//BOOL Finished;

//unsigned long __stdcall NET_RvThr(void* pParam) {
//	BOOL fSuccess;
//	char chBuf[100];
//	DWORD dwBytesToWrite = (DWORD)strlen(chBuf);
//	DWORD cbRead;
//	int i;
//
//	while (1)
//	{
//		Sleep(1);
//		fSuccess = ReadFile(hPipe2, chBuf, dwBytesToWrite, &cbRead, NULL);
//		if (fSuccess)
//		{
//			// cbread = kolko, chbuf = co
//		}
//		if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
//		{
//			printf("Can't Read\n");
//			if (Finished)
//				break;
//		}
//	}
//
//	return Finished;
//}

#include <fstream>

#include "Global.h"
#include "Parser.h"

std::string key;
std::string iv;

void RecvFunc()
{
	BOOL fSuccess;
	char chBuf[100];
	DWORD dwBytesToWrite = (DWORD)strlen(chBuf);
	DWORD cbRead;
	int i;

	int err = 0, errlimit = 150;
	
	while (true)
	{
		fSuccess = ReadFile(hPipe2, chBuf, dwBytesToWrite, &cbRead, NULL);
		if (fSuccess)
		{
			Parser::ParseData(chBuf, cbRead);
		}
		if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
		{
			err++;
			printf("\rCan't Read");
			if (Finished)
				break;
			if(err > errlimit)
			{
				pipe.Close();
				exit(1);
			}
		}
	}
}

int main(int argc, char **argv)
{
	std::fstream file;
	file.open("out.txt", std::ios::out);
	std::fprintf(stdout, "%s", "Start program\n");
	std::cout << "Start program\n";
	for(int i = 0; i < argc; i++)
	{
		std::cout << argv[i] << std::endl;
	}
#if ENCRYPT_PKT > 1 || ENCRYPT_PKT < 0 || ENCRYPT_WHENBUILD > 1 || ENCRYPT_WHENBUILD < 0 || ENCRYPT_USEBASE64 > 1 || ENCRYPT_USEBASE64 < 0 || NETWORK_OUTPUT  < 0 || NETWORK_OUTPUT > 1
	throw new std::exception("Not allowed definitions!");
#endif
#if ENCRYPT_PKT
	HexDecode(hexKey, key);
	HexDecode(hexIV, iv);
#endif
	////Pipe Init Data
	//char buf[100];

	//std::string param = "\\\\.\\pipe\\" + std::string(argv[1]) + "in";
	//LPTSTR lpszPipename1 = const_cast<LPSTR>(param.c_str());
	//param = "\\\\.\\pipe\\" + std::string(argv[1]) + "out";
	//LPTSTR lpszPipename2 = const_cast<LPSTR>(param.c_str());

	//DWORD cbWritten;
	//DWORD dwBytesToWrite = (DWORD)strlen(buf);

	////Thread Init Data
	//DWORD threadId;
	//HANDLE hThread = NULL;

	//BOOL Write_St = TRUE;
	//
	//int ticks = 0;

	//do
	//{
	//	hPipe1 = CreateFile(lpszPipename1, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
	//	Sleep(10);
	//	ticks++;
	//	if(ticks >= 150)
	//	{
	//		exit(2);
	//	}
	//} while (hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE);
	//ticks = 0;
	//do
	//{
	//	hPipe2 = CreateFile(lpszPipename2, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
	//	Sleep(10);
	//	ticks++;
	//	if (ticks >= 150)
	//	{
	//		exit(2);
	//	}
	//} while (hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE);

	//hThread = CreateThread(NULL, 0, &NET_RvThr, NULL, 0, NULL);
	//
	//WriteFile(hPipe1, buf, dwBytesToWrite, &cbWritten, NULL);
	//memset(buf, 0xCC, 100);
	std::cout << "\nStarting Named pipe...\n";
	NamedPipe named = NamedPipe(argv[1], true);
	pipe = named;
	std::cout << "Starting listening...\n";
	named.StartListening();
	char test[] = "Test";
	std::cout << "Write test...\n";
	named.Write(test, 4);
	std::cout << "End\n";
	file.close();
	
	AVConfig avconfig;
	avconfig.bitrate_bps = 4000000; // video bitrate
	avconfig.framerate = 25;        // video framerate
	avconfig.codec = "h264";  // hardware encoder: "h264_nvenc";

	//int opt;
	
	/*if(argc > 1)
	{
		while ((opt = getopt(argc, argv, "fc:")) != -1) {
			switch (opt) {
			case 'f':
				int result;
				if (auto [p, ec] = std::from_chars(optarg, optarg + std::strlen(optarg), result);
					ec == std::errc())
				{
					avconfig.framerate = result > 0 ? result : 25;
					std::cout << (result > 0 ? "" : "Bad frame-rate input -> frame-rate=25");
				}
				break;
			case 'c':
				if(strcmp(optarg, "h264"))
				{
					avconfig.codec = "h264";
				} else
				if (strcmp(optarg, "x264"))
				{
					avconfig.codec = "x264";
				} else
				if (strcmp(optarg, "h264_nvenc") && !nvenc_info.is_supported())
				{
					avconfig.codec = "h264_nvenc";
				} else
				if (strcmp(optarg, "h264_qsv") && !QsvEncoder::IsSupported())
				{
					avconfig.codec = "h264_qsv";
				} else {
					std::cerr << "Usage: MeetingServer.exe [-f framerate] [-c codec]" << std::endl
					<< "Codec types: h264, x264";
					if (nvenc_info.is_supported()) std::cerr << ", h264_nvenc";
					if (QsvEncoder::IsSupported()) std::cerr << ", h264_qsv";
					std::cerr << std::endl;
					exit(EXIT_FAILURE);
				}
				break;
			default:
				std::cerr << "Usage: MeetingServer.exe [-f framerate] [-c codec]" << std::endl;
				exit(EXIT_FAILURE);
			}
		}
	}*/

	LiveConfig live_config;

	// server
	live_config.ip = "0.0.0.0";
	live_config.port = 8554;
	live_config.suffix = "live";

	// pusher
	live_config.rtmp_url = RTMP_PUSHER_TEST;
	live_config.rtsp_url = RTSP_PUSHER_TEST;
	
	if (!ScreenLive::Instance().Init(avconfig)) {
		getchar();
		return 0;
	}
	std::cout << "(ENCRYPT_PKT)=" << ENCRYPT_PKT
			<< ", (ENCRYPT_WHENBUILD)=" << ENCRYPT_WHENBUILD
			<< ", (ENCRYPT_USEBASE64)=" << ENCRYPT_USEBASE64 << std::endl;
	ScreenLive::Instance().StartLive(SCREEN_LIVE_RTSP_SERVER, live_config);
	//ScreenLive::Instance().StartLive(SCREEN_LIVE_RTSP_PUSHER, live_config);
	//ScreenLive::Instance().StartLive(SCREEN_LIVE_RTMP_PUSHER, live_config);

	while (1) {		
		//if (ScreenLive::Instance().IsConnected(SCREEN_LIVE_RTMP_PUSHER)) {

		//}

		//if (ScreenLive::Instance().IsConnected(SCREEN_LIVE_RTSP_PUSHER)) {

		//}

		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
	}

	ScreenLive::Instance().StopLive(SCREEN_LIVE_RTSP_SERVER);
	//ScreenLive::Instance().StopLive(SCREEN_LIVE_RTSP_PUSHER);
	//ScreenLive::Instance().StopLive(SCREEN_LIVE_RTMP_PUSHER);

	ScreenLive::Instance().Destroy();
	/*CloseHandle(hPipe1);
	CloseHandle(hPipe2);*/
	getchar();
	return 0;
}