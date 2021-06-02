#include "HexPacket.h"

#include <array>
#include <charconv>

#include "Global.h"

void HexPacket::_recreatePtr(char* str, uint32_t dataSizes)
{
	auto* tmp = this->data.get();
	if(tmp != nullptr)
	{
		delete[](tmp);
	}
	tmp = new char[dataSizes];
	memcpy_s(tmp, dataSizes, str, dataSizes);
	this->data.reset(tmp);
}

void HexPacket::_constructor_helper(char* str, uint32_t size, uint32_t offset, Packet_type type)
{
	this->_recreatePtr(str, size);
	this->size = size;
	this->offset = offset;
	this->type = type;
}

HexPacket::HexPacket(std::shared_ptr<char> data, uint32_t size, uint32_t offset, Packet_type type)
{
	_constructor_helper(data.get(), size, offset, type);
}

HexPacket::HexPacket(char* str, uint32_t size, uint32_t offset, Packet_type type)
{
	_constructor_helper(str, size, offset, type);
}

HexPacket::~HexPacket()
{
	this->data.reset();
	this->offset = 0;
	this->size = 0;
}


bool HexPacket::EncryptPacket()
{
	if (type != Packet_type::Encrypt)
	{
		return false;
	}
	try
	{
		size_t size = this->size;
		auto* const encrypted = encrypt(data.get(), size);
		if (size + 16 >= this->size)
		{
			this->data.reset(reinterpret_cast<char*>(encrypted));
			this->size = size;
		}else
		{
			return false;
		}
	}
	catch (...)
	{
		return false;
	}
	
	return true;
}

//Don't forget free ptr after use
const char* HexPacket::GetDataToSend(size_t& outBytes)
{
	if (type == Packet_type::None)
	{
		return nullptr;
	}
	//create new data (4_null,x_lenght,y_data
	auto s = (this->size - this->offset) + 4 + sizeof(this->size);
	uint32_t toSendBytes = this->size - this->offset;
	char* toSend = new char[s];
	
	std::memset(toSend, '\0', s);
	std::memcpy(&(toSend[4]), &toSendBytes, sizeof(toSendBytes));
	std::memcpy(&(toSend[4+sizeof(toSendBytes)]), &((this->data.get())[this->offset]), (this->size - this->offset));
	
	outBytes = s;
	return static_cast<char*>(toSend);
}

/*
 * Packet Struct
 * [null(4)][size(4)][data(x)]
 */
bool HexPacket::DecryptPacket(char* outPtr, size_t& outBytes)
{
	if (type != Packet_type::Decrypt || static_cast<long>(size - offset) < 9 || data == nullptr)
	{
		return false;
	}

	
}
