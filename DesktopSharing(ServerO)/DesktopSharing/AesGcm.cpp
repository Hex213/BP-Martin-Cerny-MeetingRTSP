#pragma once

#ifndef CRYPTOPP_DEFAULT_NO_DLL
#define CRYPTOPP_DEFAULT_NO_DLL 1
#endif

#ifndef CRYPTOPP_ENABLE_NAMESPACE_WEAK
#define CRYPTOPP_ENABLE_NAMESPACE_WEAK 1
#endif

#ifdef _DEBUG
#ifndef x64
#pragma comment(lib, "../../Cryptopp840/Win32/Output/Debug/cryptlib.lib")
#else
#pragma comment(lib, "../../Cryptopp840/x64/Output/Debug/cryptlib.lib")
#endif
#else
#ifndef x64
#pragma comment(lib, "../../Cryptopp840/Win32/Output/Release/cryptlib.lib")
#else
#pragma comment(lib, "../../Cryptopp840/x64/Output/Release/cryptlib.lib")
#endif
#endif

// Main Include
#include "AesGcm.h"

// Crypto++ Include
#include "../../Cryptopp840/pch.h"
#include "../../Cryptopp840/files.h"
#include "../../Cryptopp840/default.h"
#include "../../Cryptopp840/base64.h"
#include "../../Cryptopp840/osrng.h"
using CryptoPP::byte;

//AES
#include "../../Cryptopp840/hex.h"
using CryptoPP::HexEncoder;
using CryptoPP::HexDecoder;

#include "../../Cryptopp840/cryptlib.h"
using CryptoPP::BufferedTransformation;
using CryptoPP::AuthenticatedSymmetricCipher;

#include "../../Cryptopp840/filters.h"
using CryptoPP::StringSink;
using CryptoPP::StringSource;
using CryptoPP::AuthenticatedEncryptionFilter;
using CryptoPP::AuthenticatedDecryptionFilter;

#include "../../Cryptopp840/aes.h"
using CryptoPP::AES;

#include "../../Cryptopp840/gcm.h"
using CryptoPP::GCM;
using CryptoPP::GCM_TablesOption;

#include <string>
#include "Global.h"
#include "HexPacket.h"

using namespace CryptoPP;
using namespace std;

std::string m_ErrorMessage;

static inline RandomNumberGenerator& PSRNG(void)
{
	static AutoSeededRandomPool rng;
	rng.Reseed();
	return rng;
}

string converter(uint8_t* str) {
	uint8_t key[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31 };
	std::ostringstream ss;
	std::copy(key, key + sizeof(key), std::ostream_iterator<int>(ss, ""));
	return ss.str();
}
/// <summary>
/// 
/// </summary>
/// <param name="aesKey">Key (32 byte version)</param>
/// <param name="aesIV">IV (16 byte version)</param>
/// <param name="inPlainText">Input data</param>
/// <param name="size">Input data size</param>
/// <param name="outEncryptedBase64">Encrypted data</param>
/// <param name="dataLength">Length of encrypted data</param>
/// <returns>If is encryption success</returns>
bool encrypt_aes256_gcm(const char* aesKey, const CryptoPP::SecByteBlock& aesIV,
	const char* inPlainText, size_t& size, char** outEncryptedBase64, int& dataLength)
{
	m_ErrorMessage.clear();
	bool bR = false;
	//const int TAG_SIZE = 12;
	std::string outText;
	std::string outBase64;

	if (strlen(aesKey) > 31 && CryptoPP::BytePtrSize(aesIV) > 15)
	{
		try
		{
			auto data = reinterpret_cast<const CryptoPP::byte*>(inPlainText);
			
			GCM< AES >::Encryption aesEncryption;
			aesEncryption.SetKeyWithIV(reinterpret_cast<const CryptoPP::byte*>(aesKey),
				AES::MAX_KEYLENGTH, aesIV.BytePtr(),/*reinterpret_cast<const CryptoPP::byte*>(aesIV),*/ AES::BLOCKSIZE);
			StringSource(data, size/*inPlainText*/, true, new AuthenticatedEncryptionFilter
			(aesEncryption, new StringSink(outText)
			) // AuthenticatedEncryptionFilter
			); // StringSource

#if ENCRYPT_USEBASE64
			CryptoPP::Base64Encoder* base64Encoder = new CryptoPP::Base64Encoder
			(new StringSink(outBase64), false);
			base64Encoder->PutMessageEnd(reinterpret_cast<const CryptoPP::byte*> (outText.data()), outText.length());
			delete base64Encoder;

			dataLength = outBase64.length();
			if (outBase64.length() > 0)
			{
				if (*outEncryptedBase64) free(*outEncryptedBase64);
				*outEncryptedBase64 = static_cast<char*>(malloc(dataLength + 1));
				memset(*outEncryptedBase64, '\0', dataLength + 1);
				memcpy(*outEncryptedBase64, outBase64.c_str(), dataLength);

				bR = true;
			}
			else
			{
				m_ErrorMessage.append("Encryption Failed");
			}
#else
			if (outText.length() == size + 16)
			{
				dataLength = outText.length();
				if (*outEncryptedBase64) free(*outEncryptedBase64);
				*outEncryptedBase64 = static_cast<char*>(malloc(dataLength + 16 + 1));
				memset(*outEncryptedBase64, '\0', dataLength + 16 + 1);
				memcpy(*outEncryptedBase64, aesIV, 16);
				memcpy(*outEncryptedBase64+16, outText.c_str(), dataLength);

				dataLength += 16;

				bR = true;
			}
#endif
		}
		catch (CryptoPP::InvalidArgument& e)
		{
			m_ErrorMessage.append(e.what());
		}
		catch (CryptoPP::Exception& e)
		{
			m_ErrorMessage.append(e.what());
		}
		catch(...)
		{
			m_ErrorMessage.append("Unknown error!");
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

//datalen input/output var
bool decrypt_aes256_gcm(const char* aesKey, const CryptoPP::SecByteBlock& aesIV,
	const char* inBase64Text, char** outDecrypted, int& dataLength)
{
	m_ErrorMessage.clear();
	bool bR = false;
	std::string outText;
	std::string pszDecodedText;
#if ENCRYPT_USEBASE64
	string* toDecode = new string(inBase64Text, dataLength);
	Base64Decode(*toDecode, pszDecodedText);
	free(toDecode);
#else
	pszDecodedText = std::string(inBase64Text, dataLength);
#endif
	if (strlen(aesKey) > 31 && /*strlen*/CryptoPP::BytePtrSize(aesIV) > 15)
	{
		//std::cout << "Decryption:\nKey=";
		//for (int i = 0; i < 32; ++i)
		//{
		//	std::cout << +((unsigned char)aesKey[i]) << "-";
		//}std::cout << "\nIV=";
		//for (int i = 0; i < 16; ++i)
		//{
		//	std::cout << +((unsigned char)aesIV[i]) << "-";
		//}std::cout << "Decryption end" << std::endl;
		/*std::cout << "\nData=\n";
		for (int i = 0; i < pszDecodedText.size(); ++i)
		{
			std::cout << +((unsigned char)pszDecodedText[i]) << "-";
		}std::cout << std::endl;*/
		try
		{
			GCM< AES >::Decryption aesDecryption;
			aesDecryption.SetKeyWithIV(reinterpret_cast<const CryptoPP::byte*>(aesKey),
				AES::MAX_KEYLENGTH, /*reinterpret_cast<const CryptoPP::byte*>(aesIV)*/aesIV.BytePtr(), AES::BLOCKSIZE);
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
		m_ErrorMessage.append(" - IV:");
		m_ErrorMessage.append(std::to_string(CryptoPP::BytePtrSize(aesIV)));
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