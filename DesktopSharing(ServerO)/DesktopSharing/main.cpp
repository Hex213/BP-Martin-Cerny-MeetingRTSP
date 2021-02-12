#include <charconv>

#include "ScreenLive.h"
#include "MainWindow.h"
#include <cstdlib>

#include "csrc/getopt.h"

#define ENABLE_SDL_WINDOW 0
#define TESTING 0

#if ENABLE_SDL_WINDOW

#ifndef _DEBUG
#pragma comment(linker, "/subsystem:\"windows\" /entry:\"mainCRTStartup\"")
#endif

static const int SDL_USEREVENT_PAINT = 0x01;

static void OnPaint(void *param)
{
	MainWindow* window = reinterpret_cast<MainWindow*>(param);

	if (window) {
		std::vector<uint8_t> bgra_image;
		uint32_t width = 0;
		uint32_t height = 0;
		if (ScreenLive::Instance().GetScreenImage(bgra_image, width, height)) {
			std::string status_info = ScreenLive::Instance().GetStatusInfo();
			window->SetDebugInfo(status_info);
			window->UpdateARGB(&bgra_image[0], width, height);
		}
	}
}

static uint32_t TimerCallback(uint32_t interval, void *param)
{
	SDL_Event evt;
	memset(&evt, 0, sizeof(evt));
	evt.user.type = SDL_USEREVENT;
	evt.user.timestamp = 0;
	evt.user.code = SDL_USEREVENT_PAINT;
	evt.user.data1 = param;
	evt.user.data2 = nullptr;
	SDL_PushEvent(&evt);

	return 100;
}

int main(int argc, char **argv)
{
	MainWindow window;
	SDL_TimerID timer_id = 0;

	if (window.Create()) {
		if (ScreenLive::Instance().StartCapture() >= 0) {
			timer_id = SDL_AddTimer(1000, TimerCallback, &window);
		}		
		else {
			return -1;
		}
	}

	bool done = false;
	while (!done && window.IsWindow()) {
		SDL_Event event;
		if (SDL_WaitEvent(&event)) {
			window.Porcess(event);

			switch (event.type)
			{
				case SDL_WINDOWEVENT: {
					if (event.window.event == SDL_WINDOWEVENT_SIZE_CHANGED) {
						window.Resize();
					}
					break;
				}

				case SDL_USEREVENT: {
					if (event.user.code == SDL_USEREVENT_PAINT) {
						OnPaint(event.user.data1);
					}
					break;
				}

				case SDL_QUIT: {
					done = true;
					break;
				}

				default: {
					break;
				}
			}
		}
	}
	
	if (timer_id) {
		SDL_RemoveTimer(timer_id);
	}

	window.Destroy();
	ScreenLive::Instance().Destroy();
	return 0;
}

#else 

#define RTSP_PUSHER_TEST "rtsp://127.0.0.1:554/test" 
#define RTMP_PUSHER_TEST "rtmp://127.0.0.1:1935/live/02"  

DWORD WINAPI ThreadProc();
HANDLE hPipe1, hPipe2;
BOOL Finished;

unsigned long __stdcall NET_RvThr(void* pParam) {
	BOOL fSuccess;
	char chBuf[100];
	DWORD dwBytesToWrite = (DWORD)strlen(chBuf);
	DWORD cbRead;
	int i;

	while (1)
	{
		Sleep(1);
		fSuccess = ReadFile(hPipe2, chBuf, dwBytesToWrite, &cbRead, NULL);
		if (fSuccess)
		{
			// cbread = kolko, chbuf = co
		}
		if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
		{
			printf("Can't Read\n");
			if (Finished)
				break;
		}
	}

	return Finished;
}

int main(int argc, char **argv)
{
	//Pipe Init Data
	char buf[100];

#ifndef  _DEBUG
	LPTSTR lpszPipename1 = const_cast<LPSTR>("\\\\.\\pipe\\serverIN");
	LPTSTR lpszPipename2 = const_cast<LPSTR>("\\\\.\\pipe\\serverOUT");

	DWORD cbWritten;
	DWORD dwBytesToWrite = (DWORD)strlen(buf);

	//Thread Init Data
	DWORD threadId;
	HANDLE hThread = NULL;

	BOOL Write_St = TRUE;
	
	int ticks = 0;

	do
	{
		hPipe1 = CreateFile(lpszPipename1, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
		Sleep(10);
		ticks++;
		if(ticks >= 150)
		{
			exit(2);
		}
	} while (hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE);
	ticks = 0;
	do
	{
		hPipe2 = CreateFile(lpszPipename2, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
		Sleep(10);
		ticks++;
		if (ticks >= 150)
		{
			exit(2);
		}
	} while (hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE);

	hThread = CreateThread(NULL, 0, &NET_RvThr, NULL, 0, NULL);

#endif
	
	//WriteFile(hPipe1, buf, dwBytesToWrite, &cbWritten, NULL);
	//memset(buf, 0xCC, 100);


	//CloseHandle(hPipe1);
	//CloseHandle(hPipe2);
	
	AVConfig avconfig;
	avconfig.bitrate_bps = 4000000; // video bitrate
	avconfig.framerate = 25;        // video framerate
	avconfig.codec = "h264";  // hardware encoder: "h264_nvenc";

	int opt;
	
	if(argc > 1)
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
	}

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

	getchar();
	return 0;
}

#endif