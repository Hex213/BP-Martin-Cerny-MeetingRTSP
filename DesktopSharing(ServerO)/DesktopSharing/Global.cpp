#include "Global.h"

#if ENCRYPT_PKT
extern std::string key;
extern std::string iv;
#endif

char* encrypt(const char* message, size_t& size)
{
	int _size = 0;
	char* outEncryptedText = nullptr;
	char* data_to_encrypt = nullptr;
#if ENCRYPT_PKT
	std::string pktData;
	size_t pktDataLen;
	if (size == 0)
	{
		pktData = std::string(message);
		pktDataLen = pktData.length();
		if (pktData.length() > 3)
		{
			pktData.append(".");
			pktDataLen++;
		}
	}
	else
	{
		pktData = std::string(message, size);
		pktDataLen = size;
	}

	if (encrypt_aes256_gcm(key.c_str(),
		iv.c_str(), pktData.c_str(), pktDataLen, &outEncryptedText, _size) == false)
	{
		std::cout << "enrcypt err";
	}
	size = _size;
#endif
	return outEncryptedText;
}

void _clear_buff(const char* buff, size_t buf_size)
{
	if (buff != nullptr && buf_size > 0)
	{
		memset((void*)buff, 0, buf_size);
	}
}

void clear_buffers(const char* buff1, size_t buff1_size,
	const char* buff2, size_t buff2_size,
	const char* buff3, size_t buff3_size)
{
	_clear_buff(buff1, buff1_size);
	_clear_buff(buff2, buff2_size == 0 && buff2 != nullptr ? buff1_size : buff2_size);
	_clear_buff(buff3, buff3_size == 0 && buff3 != nullptr ? buff2_size : buff3_size);
}

int clearRam(const char* buff1, void* buff2, void* buff3)
{
	int out = (int)strlen(buff1);
	if (buff2 != nullptr) free(buff2);
	if (buff3 != nullptr) free(buff3);
	return out;
}

//buf - where save
//buf_size - size of buffer
//res - message to encrypt
int encryptAndClear(const char* buf, int buf_size, char* res, int size)
{	
#if ENCRYPT_PKT == 1 
	size_t sz = size;
	auto* const encrypted = encrypt(res, sz);
	snprintf((char*)buf, buf_size, encrypted);
	return clearRam(buf, res, encrypted);
#else
	return encryptAndClearWO(buf, buf_size, res, size);
#endif
}

int encryptAndClearWO(const char* buf, int buf_size, char* res, int size)
{
	if (res != nullptr) free(res);
	return (int)strlen(buf);
}