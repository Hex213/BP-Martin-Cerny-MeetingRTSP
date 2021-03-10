#pragma once
#include <memory>

enum class Packet_type
{
	None,
	Encrypt,
	Decrypt
};

class HexPacket
{
private:
	std::shared_ptr<char> data;
	uint32_t size;
	uint32_t offset;
	bool sended = false;
	Packet_type type = Packet_type::None;

	void _recreatePtr(char* str, uint32_t dataSizes);
	void _constructor_helper(char* str, uint32_t size, uint32_t offset, Packet_type type);
	
public:
	HexPacket(std::shared_ptr<char> data, uint32_t size, uint32_t offset, Packet_type type);
	HexPacket(char* str, uint32_t size, uint32_t offset, Packet_type type);
	~HexPacket();
	//Encrypt data
	bool EncryptPacket();
	//Don't forget free ptr
	const char* GetDataToSend(size_t& outBytes);
	//Decrypt data
	bool DecryptPacket(char* outPtr, size_t& outBytes);

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

