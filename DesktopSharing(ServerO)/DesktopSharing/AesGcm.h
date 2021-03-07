#include <cryptlib.h>

bool encrypt_aes256_gcm(const char* aesKey, const char* aesIV,
                        const char* inPlainText, size_t& size, char** outEncryptedBase64, int& dataLength);
bool decrypt_aes256_gcm(const char* aesKey, const char* aesIV,
	const char* inBase64Text, char** outDecrypted, int& dataLength);
void Base64Decode(const std::string& inString, std::string& outString);
void HexDecode(const std::string& inString, std::string& outString);
static inline CryptoPP::RandomNumberGenerator& PSRNG(void);
std::string converter(uint8_t* str);