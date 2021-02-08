#include <stdio.h>
#include <windows.h>

unsigned long __stdcall NET_RvThr(void * pParam) ;
DWORD WINAPI ThreadProc() ;
HANDLE hPipe1,hPipe2; 
BOOL Finished;

int main(int argc, char *argv[])
{
	//Pipe Init Data
	char buf[100];
    
	LPTSTR lpszPipename1 = TEXT("\\\\.\\pipe\\serverIN"); 
	LPTSTR lpszPipename2 = TEXT("\\\\.\\pipe\\serverOUT"); 

	DWORD cbWritten;
	DWORD dwBytesToWrite = (DWORD)strlen(buf);

	//Thread Init Data
	DWORD threadId;
	HANDLE hThread = NULL;

	BOOL Write_St=TRUE;

	Finished=FALSE;




	hPipe1=CreateFile(lpszPipename1,	GENERIC_WRITE ,0,NULL,OPEN_EXISTING,FILE_FLAG_OVERLAPPED,NULL);
	hPipe2=CreateFile(lpszPipename2,	GENERIC_READ ,0,NULL,OPEN_EXISTING,FILE_FLAG_OVERLAPPED,NULL);


	 if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE)||(hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE))
    { 
        printf("Could not open the pipe  - (error %d)\n",GetLastError());
        
    }
	 else
	 {
	
		 hThread = CreateThread( NULL, 0, &NET_RvThr, NULL, 0, NULL);
		 do
		 {
			 printf ("Enter your message: ");
			 scanf ("%s",buf);  
			 if	(strcmp (buf,"quit") == 0)
				Write_St=FALSE;
			 else
			 {
				WriteFile(hPipe1, buf, dwBytesToWrite, &cbWritten, NULL);
				memset(buf,0xCC,100);
				
			 }
			
		}while(Write_St);

		CloseHandle(hPipe1);
		CloseHandle(hPipe2);
		Finished=TRUE;
	 }

	 getchar();

	
}
unsigned long __stdcall NET_RvThr(void * pParam) {
	BOOL fSuccess; 
	char chBuf[100];
	DWORD dwBytesToWrite = (DWORD)strlen(chBuf);
	DWORD cbRead;
	int i;

	while(1)
	{
		fSuccess =ReadFile( hPipe2,chBuf,dwBytesToWrite,&cbRead, NULL); 
		if(fSuccess)
		{
			printf("C++ App: Received %d Bytes : ",cbRead);
			for(i=0;i<cbRead;i++)
				printf("%c",chBuf[i]);
			printf("\n");
		}
		if (! fSuccess && GetLastError() != ERROR_MORE_DATA) 
		{
			printf("Can't Read\n");
			if(Finished)
				break;
		}
	}
}