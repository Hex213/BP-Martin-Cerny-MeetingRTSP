// TestAES_GCM_256_C.cpp : Defines the entry point for the console application.
//

#ifndef _CRT_SECURE_NO_DEPRECATE
#define _CRT_SECURE_NO_DEPRECATE 1
#endif

#ifndef CRYPTOPP_DEFAULT_NO_DLL
#define CRYPTOPP_DEFAULT_NO_DLL 1
#endif

#ifndef CRYPTOPP_ENABLE_NAMESPACE_WEAK
#define CRYPTOPP_ENABLE_NAMESPACE_WEAK 1
#endif

//#ifdef _DEBUG
//
//#ifndef x64
//#pragma comment(lib, "../Cryptopp840/Win32/Output/Debug/cryptlib.lib")
//#else
//#pragma comment(lib, "../Cryptopp840/x64/Output/Debug/cryptlib.lib")
//#endif
//#else
//#ifndef x64
//#pragma comment(lib, "../Cryptopp840/Win32/Output/Release/cryptlib.lib")
//#else
//#pragma comment(lib, "../Cryptopp840/x64/Output/Release/cryptlib.lib")
//#endif
//#endif

#include "Hex/Crypto/AesGcm256.h"

// Crypto++ Include

#include "../Cryptopp840/pch.h"
#include "../Cryptopp840/files.h"
#include "../Cryptopp840/default.h"
#include "../Cryptopp840/base64.h"
#include "../Cryptopp840/osrng.h"
using CryptoPP::byte;

//AES
#include "../Cryptopp840/hex.h"
using CryptoPP::HexEncoder;
using CryptoPP::HexDecoder;

#include "../Cryptopp840/cryptlib.h"
using CryptoPP::BufferedTransformation;
using CryptoPP::AuthenticatedSymmetricCipher;

#include "../Cryptopp840/filters.h"
using CryptoPP::StringSink;
using CryptoPP::StringSource;
using CryptoPP::AuthenticatedEncryptionFilter;
using CryptoPP::AuthenticatedDecryptionFilter;

#include "../Cryptopp840/aes.h"
using CryptoPP::AES;

#include "../Cryptopp840/gcm.h"
using CryptoPP::GCM;
using CryptoPP::GCM_TablesOption;

#include <iostream>
#include <string>

USING_NAMESPACE(CryptoPP)
USING_NAMESPACE(std)

static inline RandomNumberGenerator& PSRNG(void);
static inline RandomNumberGenerator& PSRNG(void)
{
	static AutoSeededRandomPool rng;
	rng.Reseed();
	return rng;
}

static std::string m_ErrorMessage;

//int main()
//{
//	//using above code these key and iv was generated
//	std::string hexKey = "2192B39425BBD08B6E8E61C5D1F1BC9F428FC569FBC6F78C0BC48FCCDB0F42AE";
//	std::string hexDecodedKey;
//	HexDecode(hexKey, hexDecodedKey);
//
//	std::string hexIV = "E1E592E87225847C11D948684F3B070D";
//	std::string hexDecodedIv;
//	HexDecode(hexIV, hexDecodedIv);
//
//	std::string plainText = "Test encryption and decryption";
//	std::string encryptedWithJava = "A/boAixWJKflKviHp2cfDl6l/xn1qw2MsHcKFkrOfm2XOVmawIFct4fS1w7wKw==";
//	std::string encryptedWithCsharp = "A/boAixWJKflKviHp2cfDl6l/xn1qw2MsHcKFkrOH78KH7SmFDexI5kIja8JIfO42LGmGo5P";
//
//	printf("%s%s\n", "Plain Text: ", plainText.c_str());
//
//	//encrypt - result base64 encoded string
//	char* outEncryptedText = NULL;
//	int outDataLength = 0;
//	bool bR = encrypt_aes256_gcm(hexDecodedKey.c_str(),
//		hexDecodedIv.c_str(), plainText.c_str(), &outEncryptedText, outDataLength);
//	printf("%s%s\n", "Encrypted base64 encoded: ", outEncryptedText);
//
//	//decrypt - result plain string
//	char* outDecryptedText = NULL;
//	int outDecryptedDataLength = 0;
//	bR = decrypt_aes256_gcm(hexDecodedKey.c_str(),
//		hexDecodedIv.c_str(), encryptedWithCsharp.c_str(), &outDecryptedText, outDecryptedDataLength);
//	printf("%s%s\n", "Decrypted Text Encrypted by Java: ", outDecryptedText);
//
//	/*if (plainText == outDecryptedText)
//	{
//		printf("%s\n", "Test Passed");
//	}
//	else
//	{
//		printf("%s\n", "Test Failed");
//	}*/
//
//	return 0;
//}

bool encrypt_aes256_gcm(const char* aesKey, const char* aesIV,
	const char* inPlainText, char** outEncryptedBase64, int& dataLength)
{
	bool bR = false;
	//const int TAG_SIZE = 12;
	std::string outText;
	std::string outBase64;

	CryptoPP::byte key[32];
	memset(key, 0, sizeof(key));
	CryptoPP::byte iv[16];
	memset(iv, 0, sizeof(iv));

	if (strlen(aesKey) > 31 && strlen(aesIV) > 15)
	{
		try
		{
			GCM< AES >::Encryption aesEncryption;
			aesEncryption.SetKeyWithIV(key, AES::MAX_KEYLENGTH, iv, AES::BLOCKSIZE);
			StringSource(inPlainText, true, new AuthenticatedEncryptionFilter
			(aesEncryption, new StringSink(outText)
			) // AuthenticatedEncryptionFilter
			); // StringSource

			CryptoPP::byte* outTextBytes = new CryptoPP::byte[outText.length()]();
			memcpy_s(outTextBytes, outText.length(), outText.c_str(), outText.length());

			CryptoPP::Base64Encoder* base64Encoder = new CryptoPP::Base64Encoder
			(new StringSink(outBase64), false);
			base64Encoder->PutMessageEnd(outTextBytes, outText.length());
			delete base64Encoder;

			dataLength = outBase64.length();
			if (outBase64.length() > 0)
			{
				if (*outEncryptedBase64) free(*outEncryptedBase64);
				*outEncryptedBase64 = (char*)malloc(dataLength + 1);
				memset(*outEncryptedBase64, '\0', dataLength + 1);
				memcpy(*outEncryptedBase64, outBase64.c_str(), dataLength);

				bR = true;
			}
			else
			{
				m_ErrorMessage.append("Encryption Failed");
			}

		}
		catch (CryptoPP::InvalidArgument& e)
		{
			m_ErrorMessage.append(e.what());
		}
		catch (CryptoPP::Exception& e)
		{
			m_ErrorMessage.append(e.what());
		}
	}
	else
	{
		m_ErrorMessage.append("AES Key or IV cannot be empty");
	}

	outText.clear();
	outBase64.clear();

	return bR;
}

bool decrypt_aes256_gcm(const char* aesKey, const char* aesIV,
	const char* inBase64Text, char** outDecrypted, int& dataLength)
{
	bool bR = false;
	std::string outText;
	std::string pszDecodedText;
	Base64Decode(inBase64Text, pszDecodedText);

	const unsigned char* aesKeyTmp = reinterpret_cast<const unsigned char*>(aesKey);
	const unsigned char* aesIVTmp = reinterpret_cast<const unsigned char*>(aesIV);

	SecByteBlock key(aesKeyTmp, AES::MAX_KEYLENGTH);
	SecByteBlock IV(aesIVTmp, AES::BLOCKSIZE);

	if (strlen(aesKey) > 31 && strlen(aesIV) > 15)
	{
		try
		{
			GCM< AES >::Decryption aesDecryption;
			aesDecryption.SetKeyWithIV(key, key.SizeInBytes(), IV, IV.SizeInBytes());
			AuthenticatedDecryptionFilter df(aesDecryption, new StringSink(outText));

			StringSource(pszDecodedText, true,
				new Redirector(df /*, PASS_EVERYTHING */)
			); // StringSource

			bR = df.GetLastResult();

			dataLength = outText.length();
			if (outText.length() > 0)
			{
				if (*outDecrypted) free(*outDecrypted);
				*outDecrypted = (char*)malloc(dataLength + 1);
				memset(*outDecrypted, '\0', dataLength + 1);
				memcpy(*outDecrypted, outText.c_str(), dataLength);

				bR = true;
			}
			else
			{
				m_ErrorMessage.append("Decryption Failed");
			}
		}
		catch (CryptoPP::HashVerificationFilter::HashVerificationFailed& e)
		{
			m_ErrorMessage.append(e.what());
		}
		catch (CryptoPP::InvalidArgument& e)
		{
			m_ErrorMessage.append(e.what());
		}
		catch (CryptoPP::Exception& e)
		{
			m_ErrorMessage.append(e.what());
		}
	}
	else
	{
		m_ErrorMessage.append("AES Key or IV cannot be empty");
	}

	return bR;
}

void Base64Decode(const std::string& inString, std::string& outString)
{
	StringSource(inString, true, new Base64Decoder(new StringSink(outString)));
}

void HexDecode(const std::string& inString, std::string& outString)
{
	StringSource(inString, true, new HexDecoder(new StringSink(outString)));
}

//AutoSeededRandomPool prng;
//
//SecByteBlock key(AES::DEFAULT_KEYLENGTH);
//prng.GenerateBlock(key, key.size());
//
//byte ctr[AES::BLOCKSIZE];
//prng.GenerateBlock(ctr, sizeof(ctr));
//
//string plain = "CTR Mode Test";
//string cipher, encoded, recovered;
//
///*********************************\
//\*********************************/
//
//try
//{
//	cout << "plain text: " << plain << endl;
//
//	CTR_Mode< AES >::Encryption e;
//	e.SetKeyWithIV(key, key.size(), ctr);
//
//	// The StreamTransformationFilter adds padding
//	//  as required. ECB and CBC Mode must be padded
//	//  to the block size of the cipher. CTR does not.
//	StringSource ss1(plain, true,
//		new StreamTransformationFilter(e,
//			new StringSink(cipher)
//		) // StreamTransformationFilter      
//	); // StringSource
//}
//catch (CryptoPP::Exception& e)
//{
//	cerr << e.what() << endl;
//	exit(1);
//}
//
///*********************************\
//\*********************************/
//
//// Pretty print cipher text
//StringSource ss2(cipher, true,
//	new HexEncoder(
//		new StringSink(encoded)
//	) // HexEncoder
//); // StringSource
//cout << "cipher text: " << encoded << endl;
//
///*********************************\
//\*********************************/
//
//try
//{
//	CTR_Mode< AES >::Decryption d;
//	d.SetKeyWithIV(key, key.size(), ctr);
//
//	// The StreamTransformationFilter removes
//	//  padding as required.
//	StringSource ss3(cipher, true,
//		new StreamTransformationFilter(d,
//			new StringSink(recovered)
//		) // StreamTransformationFilter
//	); // StringSource
//
//	cout << "recovered text: " << recovered << endl;
//}
//catch (CryptoPP::Exception& e)
//{
//	cerr << e.what() << endl;
//	exit(1);
//}