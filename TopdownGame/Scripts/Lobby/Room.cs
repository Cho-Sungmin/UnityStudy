using System;


public class Room
{
	public string m_roomId;
	public UInt32 m_capacity { get; set; }
	public UInt32 m_presentMembers { get; set; }

	public string m_title;

	public int GetSize() 
	{	return m_title.Length + sizeof(int) + ( sizeof(UInt32) * 2 );	}

	public void Write( OutputByteStream stream )
	{
		stream.Write( m_roomId );
		stream.Write( (UInt32)m_capacity );
		stream.Write( (UInt32)m_presentMembers );
		stream.Write( m_title );
	}

	public void Read( InputByteStream stream )
	{
		UInt32 tmp;

		stream.Read( out m_roomId );
		stream.Read( out tmp );
		m_capacity = tmp;
		stream.Read( out tmp );
		m_presentMembers = tmp;
		stream.Read( out m_title );
	}
}
