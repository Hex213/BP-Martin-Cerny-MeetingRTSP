#pragma once
#include <memory>

class HexPacket
{
private:
	std::shared_ptr<char> data;
	uint32_t size;
	uint32_t offset;
	bool sended = false;

	void recreatePtr(char* str, uint32_t dataSizes);
	
public:
	HexPacket(std::shared_ptr<char> data, uint32_t size, uint32_t offset);
	HexPacket(char* str, uint32_t uint32, uint32_t i);
	~HexPacket();
	//Encrypt data - maybe ram leak
	bool encryptPacket();
	//Don't forget free ptr
	const char* getDataToSend(size_t& outBytes);


	void set_offset(uint32_t offset)
	{
		this->offset = offset;
	}

	[[nodiscard]] const char* get_data() const
	{
		return data.get();
	}

	[[nodiscard]] uint32_t get_size() const
	{
		return size;
	}

	[[nodiscard]] uint32_t get_offset() const
	{
		return offset;
	}
};

