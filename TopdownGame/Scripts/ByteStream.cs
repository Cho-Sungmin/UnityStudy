using System;


public class InputByteStream
{
	int cursor = 0;
	UInt32 capacity;
	byte[] buffer;

	public InputByteStream( UInt32 maxBufSize )
	{
		capacity = maxBufSize;
		buffer = new byte[capacity];
	}

	public InputByteStream( OutputByteStream obstream )
	{
		//capacity = (UInt32)obstream.GetLength();
		//buffer = new byte[capacity];

		//System.Buffer.BlockCopy( obstream.GetBuffer() , 0 , buffer , 0 , (int)capacity );

		capacity = (uint) obstream.GetLength();
		buffer = obstream.GetBuffer();
	}

	public int GetLength()
	{	return (int)capacity - cursor;	}

	public byte[] GetBuffer()
	{	return buffer;	}

	void ReallocBuffer( UInt32 newSize )
	{
		byte[] tmp = new byte[newSize];

		System.Buffer.BlockCopy( buffer , 0 , tmp , 0 , cursor );
		capacity = newSize;
		buffer = tmp;
	}

	public void Read( byte[] data , UInt32 size )
	{
		System.Buffer.BlockCopy( buffer , cursor , data , 0 , (int)size );
		cursor += (int)size;
	}
	public void Read( out byte data )
	{
		data = buffer[cursor++];
	}

	public void Read( out UInt16 data )
	{
		data = System.BitConverter.ToUInt16( buffer , cursor );
		cursor += sizeof(UInt16);
	}

	public void Read( out UInt32 data )
	{
		data = System.BitConverter.ToUInt32( buffer , cursor );
		cursor += sizeof(UInt32);
	}
	public void Read( out string data )
	{
		//--- Read length of string ---//
		UInt32 len; 
		Read( out len );

		if( len < 1 )
		{
			data = "";
			return;
		}

		//--- Read string ---//
		data = System.Text.Encoding.Default.GetString( buffer , cursor , (int)len );

		cursor += (int)len;
	}

	public void flush()
	{
		cursor = 0;
	}

	public uint GetCapacity()
	{ return capacity; }

	public int GetCursor()
	{ return cursor; }
}


public class OutputByteStream
{
	int cursor = 0;
	UInt32 capacity;
	byte[] buffer;

	public OutputByteStream( UInt32 maxBufSize )
	{
		capacity = maxBufSize;
		buffer = new byte[capacity];
	}

	public OutputByteStream( InputByteStream ibstream )
	{
		//capacity = (UInt32)ibstream.GetLength();
		//buffer = new byte[capacity];
		//Write( ibstream.GetBuffer() , (UInt32)ibstream.GetLength() );
		ibstream.flush();
		capacity = ibstream.GetCapacity();
		buffer = ibstream.GetBuffer();
	}

	public int GetLength()
	{	return cursor;	}

	public byte[] GetBuffer()
	{	return buffer;	}

	void ReallocBuffer( UInt32 newSize )
	{
		byte[] tmp = new byte[newSize];

		System.Buffer.BlockCopy( buffer , 0 , tmp , 0 , cursor );
		capacity = newSize;
		buffer = tmp;
	}

	public void Write( byte[] data , UInt32 size )
	{
		UInt32 dataLen = size;

		if( dataLen > capacity - cursor )
		{
			UInt32 newCapacity = Math.Max( dataLen , 2 * capacity );

			ReallocBuffer( newCapacity );
		}

		System.Buffer.BlockCopy( data , 0 , buffer , cursor , (int)dataLen );

		cursor += (int)dataLen;
	}

	public void Write( byte data )
	{
		buffer[cursor++] = data;
	}

	public void Write( UInt16 data )
	{
		byte[] dataBuffer = System.BitConverter.GetBytes( data );

		Write( dataBuffer , (UInt32)dataBuffer.Length );
	}

	public void Write( UInt32 data )
	{
		byte[] dataBuffer = System.BitConverter.GetBytes( data );

		Write( dataBuffer , (UInt32)dataBuffer.Length );
	}

	public void Write( string data )
	{
		byte[] dataBuffer = System.Text.Encoding.Default.GetBytes( data );
		UInt32 dataLen = (UInt32)dataBuffer.Length;

		Write( dataLen );
		Write( dataBuffer , dataLen );
	}

	public void flush()
	{
		cursor = 0;
	}

	public uint GetCapacity()
	{ return capacity; }

	public int GetCursor()
	{ return cursor; }
}
