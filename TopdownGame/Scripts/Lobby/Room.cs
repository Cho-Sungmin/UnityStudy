using System.Collections;
using System.Collections.Generic;


public class Room
{
	public int m_id { get; set; }
	public int m_capacity { get; set; }
	public int m_presentMembers { get; set; }

	public string m_title;

	public int SIZE() 
	{
		return m_title.Length + ( sizeof(int)*2 );
	}

	
	public void SetBytes( byte[] data , int startIndex = 0 )
	{
		int offset = startIndex;

		m_id = System.BitConverter.ToInt32( data , offset );
		offset += sizeof(int);

		byte[] tmp_title = new byte[20];

		System.Buffer.BlockCopy( tmp_title , 0 , data , offset , tmp_title.Length );

		m_title = System.Text.Encoding.Default.GetString( tmp_title );
		offset += m_title.Length;

		m_capacity = System.BitConverter.ToInt32( data , offset );
		offset += sizeof(int);

		m_presentMembers = System.BitConverter.ToInt32( data , offset );
	}


}
