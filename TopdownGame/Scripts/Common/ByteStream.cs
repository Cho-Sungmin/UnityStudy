using System;


public class InputByteStream
{
	int cursor = 0;
	int capacity;
	byte[] buffer;

	public InputByteStream( int maxBufSize )
	{
		capacity = maxBufSize;
		buffer = new byte[capacity];
	}

	public InputByteStream( OutputByteStream obstream )
	{
		capacity = obstream.GetLength();
		buffer = new byte[capacity];
		System.Buffer.BlockCopy( obstream.GetBuffer() , 0 , buffer , 0 , capacity );
	}


	public int GetRemainLength()
	{ return capacity - cursor; }

	public byte[] GetBuffer()
	{ return buffer; }

	public void Read( byte[] data , int size )
	{
		if( IsEmpty() )
			return;

		if( size <= GetRemainLength() )
		{
			System.Buffer.BlockCopy( buffer , cursor , data , 0 , size );
			cursor += size;
		}
	}

	public void Read( out byte data )
	{
		if( IsEmpty() )
		{
			data = 0;
			return;
		}

		data = buffer[cursor++];
	}

	public void Read( out UInt16 data )
	{
		if( IsEmpty() )
		{
			data = 0;
			return;
		}

		data = System.BitConverter.ToUInt16( buffer , cursor );
		cursor += sizeof(UInt16);
	}

	public void Read( out UInt32 data )
	{
		if( IsEmpty() )
		{
			data = 0;
			return;
		}

		data = System.BitConverter.ToUInt32( buffer , cursor );
		cursor += sizeof(UInt32);
	}

	public void Read( out int data )
	{
		if( IsEmpty() )
		{
			data = 0;
			return;
		}

		data = System.BitConverter.ToInt32( buffer , cursor );
		cursor += sizeof(int);
	}
	public void Read( out float data )
	{
		if( IsEmpty() )
		{
			data = 0;
			return;
		}

		data = System.BitConverter.ToSingle( buffer , cursor );
		cursor += sizeof(float);
	}

	public void Read( out string data )
	{
		//--- Read length of string ---//
		int len; 

		Read( out len );

		if( len < 1 )
		{
			data = "";
			return;
		}

		//--- Read string ---//
		data = System.Text.Encoding.Default.GetString( buffer , cursor , len );

		cursor += len;
	}

	public void ReUse()
	{
		cursor = 0;
	}

	public int GetCapacity()
	{ return capacity; }

	public int GetCursor()
	{ return cursor; }

	public bool IsEmpty() 
	{ return GetRemainLength() > 0 ? false : true; }
}


public class OutputByteStream
{
	int cursor = 0;
	int capacity;
	byte[] buffer;

	public OutputByteStream( int maxBufSize )
	{
		capacity = maxBufSize;
		buffer = new byte[capacity];
	}

	public OutputByteStream( InputByteStream ibstream )
	{
		capacity = ibstream.GetCapacity();
		buffer = new byte[TCP.TCP.MAX_PAYLOAD_SIZE];
		System.Buffer.BlockCopy( ibstream.GetBuffer() , 0 , buffer , 0 , capacity );
	}

	public void Shift( int size )
	{
		Buffer.BlockCopy( buffer , 0 , buffer , size ,  cursor );
		cursor += size;
	}

	public int GetLength()
	{	return cursor;	}

	public byte[] GetBuffer()
	{	return buffer;	}

	public void SetCursor( int newCursor )
	{ cursor = newCursor; }

	void ReallocBuffer( int newSize )
	{
		byte[] tmp = new byte[newSize];

		System.Buffer.BlockCopy( buffer , 0 , tmp , 0 , cursor );
		capacity = newSize;
		buffer = tmp;
	}

	public void Write( byte[] data , int size )
	{
		int dataLen = size;

		if( dataLen > capacity - cursor )
		{
			int newCapacity = Math.Max( dataLen + cursor , 2 * capacity );

			ReallocBuffer( newCapacity );
		}

		System.Buffer.BlockCopy( data , 0 , buffer , cursor , dataLen );

		cursor += dataLen;
	}

	public void Write( byte data )
	{
		buffer[cursor++] = data;
	}

	public void Write( UInt16 data )
	{
		byte[] dataBuffer = System.BitConverter.GetBytes( data );

		Write( dataBuffer , dataBuffer.Length );
	}

	public void Write( UInt32 data )
	{
		byte[] dataBuffer = System.BitConverter.GetBytes( data );

		Write( dataBuffer , dataBuffer.Length );
	}

	public void Write( int data )
	{
		byte[] dataBuffer = System.BitConverter.GetBytes( data );

		Write( dataBuffer , dataBuffer.Length );
	}

	public void Write( float data )
	{
		byte[] dataBuffer = System.BitConverter.GetBytes( data );

		Write( dataBuffer , dataBuffer.Length );
	}

	public void Write( string data )
	{
		byte[] dataBuffer = System.Text.Encoding.Default.GetBytes( data );
		int dataLen = dataBuffer.Length;

		Write( dataLen );
		Write( dataBuffer , dataLen );
	}

	public void Flush()
	{
		cursor = 0;
	}

	public int GetCapacity()
	{ return capacity; }

	public int GetCursor()
	{ return cursor; }

}
