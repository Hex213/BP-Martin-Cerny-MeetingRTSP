#pragma once
#include <string>

#include "AesGcm.h"

//switch encryption/decryption(0,1)
#define ENCRYPT_PKT 1
//force use base64 encryption(f,t)
#define ENCRYPT_USEBASE64 1
//switch encryption in moment when is request build
#define ENCRYPT_WHENBUILD 0
//turn on/off output
#define NETWORK_OUTPUT 0

static std::string hexKey = "2192B39425BBD08B6E8E61C5D1F1BC9F428FC569FBC6F78C0BC48FCCDB0F42AE";
static std::string hexIV = "E1E592E87225847C11D948684F3B070D";

#if ENCRYPT_PKT == 0
static bool first = true;
#endif

//functions for encrypt and decrypt
//main encrypt
char* encrypt(const char* message, size_t& size);
//clear buffer helper
void _clear_buff(const char* buff, size_t buf_size);
//clear buffers
void clear_buffers(const char* buff1, size_t buff1_size,
	const char* buff2 = nullptr, size_t buff2_size = 0,
	const char* buff3 = nullptr, size_t buff3_size = 0);
//free buffers 2,3 and return length of buff1
int clearRam(const char* buff1, void* buff2 = nullptr, void* buff3 = nullptr);
//main switch function for encrypt(or not) and clear res buffer
int encryptAndClear(const char* buf, int buf_size, char* res, int size = 0);
int encryptAndClearWO(const char* buf, int buf_size, char* res, int size = 0);

//Decrypt
char* decrypt(char* data, size_t len, int& outSize);

std::string GetLastCryptoError();