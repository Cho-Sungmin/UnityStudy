using System.Collections;
using System.Collections.Generic;


public class UserInfo {
    byte[] m_id;  
    byte[] m_pw; 

    byte[] m_name;
    int m_age = 0;


    //--- Constructor ---//
    
    public UserInfo()
    {
        m_id = new byte[12];
        m_pw = new byte[12];
        m_name = new byte[12];
    }

    //--- Getter/Setter ---//

    public byte[] GetId()
    {
        return m_id;
    }
    public byte[] GetPw()
    {
        return m_pw;
    }
    public byte[] GetName()
    {
        return m_name;
    }

    public void SetId( string id )
    {
        if( id.Length < m_id.Length )
		{
			for( int i=id.Length; i<m_id.Length; i++ ) 
				id += " ";
		}
        byte[] tmp_id = System.Text.Encoding.UTF8.GetBytes(id);

        System.Buffer.BlockCopy( tmp_id , 0 , m_id , 0 , tmp_id.Length );
    }
    public void SetPw( string pw )
    {
        if( pw.Length < m_pw.Length )
		{
			for( int i=pw.Length; i<m_pw.Length; i++ ) 
				pw += " ";
		}
        byte[] tmp_pw = System.Text.Encoding.UTF8.GetBytes(pw);

        System.Buffer.BlockCopy( tmp_pw , 0 , m_pw , 0 , tmp_pw.Length );
    }
    public void SetName( string name )
    {
        if( name.Length < m_name.Length )
		{
			for( int i=name.Length; i<m_name.Length; i++ ) 
				name += " ";
		}
        byte[] tmp_name = System.Text.Encoding.UTF8.GetBytes(name);

        System.Buffer.BlockCopy( tmp_name , 0 , m_name , 0 , tmp_name.Length );
        //strncpy( m_name , newName.c_str() , sizeof(m_name) );
    }

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

    public int Size()
    {
        return m_id.Length + m_pw.Length + m_name.Length + sizeof(int);
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

}

