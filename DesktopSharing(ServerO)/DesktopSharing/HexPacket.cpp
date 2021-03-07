#include "HexPacket.h"

#include <array>
#include <charconv>

#include "Global.h"

void HexPacket::recreatePtr(char* str, uint32_t dataSizes)
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


HexPacket::HexPacket(std::shared_ptr<char> data, uint32_t size, uint32_t offset)
{
	this->recreatePtr(data.get(), size);
	this->size = size;
	this->offset = offset;
}

HexPacket::HexPacket(char* data, uint32_t size, uint32_t offset)
{
	this->recreatePtr(data, size);
	this->size = size;
	this->offset = offset;
}

HexPacket::~HexPacket()
{
	this->data.reset();
	this->offset = 0;
	this->size = 0;
}


bool HexPacket::encryptPacket()
{
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
const char* HexPacket::getDataToSend(size_t& outBytes)
{
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
