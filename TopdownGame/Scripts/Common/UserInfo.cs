using System;


public class UserInfo {
    public string m_id = "";  
    public string m_pw = ""; 
    public string m_name = "";
    public byte m_age = 0;



    public int GetSize()
    { return m_id.Length + m_pw.Length + m_name.Length + sizeof(UInt32); }



    public void Write( OutputByteStream stream )
	{
		stream.Write( m_id );
		stream.Write( m_pw );
		stream.Write( m_name );
		stream.Write( m_age );
	}

	public void Read( InputByteStream stream )
	{

		stream.Read( out m_id );
		stream.Read( out m_pw );
		stream.Read( out m_name );
		stream.Read( out m_age );
	}

    #if false
    public byte[] GetBytes()
    {
        int len = m_id.Length + m_pw.Length + m_name.Length + sizeof(int);
        byte[] result = new byte[len];
        int offset = 0;

        System.Buffer.BlockCopy( m_id , 0 , result , offset , m_id.Length );
        offset += m_id.Length;
        System.Buffer.BlockCopy( m_pw , 0 , result , offset , m_pw.Length );
        offset += m_pw.Length;
        System.Buffer.BlockCopy( m_name , 0 , result , offset , m_name.Length );
        offset += m_name.Length;
        System.Buffer.BlockCopy( System.BitConverter.GetBytes(m_age) , 0 , result , offset , sizeof(int) );
        
        return result;

    }

    public void SetBytes( byte[] data )
    {
        int offset = 0;
        
        System.Buffer.BlockCopy( data , offset , m_id , 0 , m_id.Length );
        offset += m_id.Length;
        System.Buffer.BlockCopy( data , offset , m_pw , 0 , m_pw.Length );
        offset += m_pw.Length;
        System.Buffer.BlockCopy( data , offset , m_name , 0 , m_name.Length );
        offset += m_name.Length;
        m_age = System.BitConverter.ToInt32(data , offset );
    }

    #endif
    public int SIZE()
    {
        return m_id.Length + m_pw.Length + m_name.Length + sizeof(UInt32);
    }
}

