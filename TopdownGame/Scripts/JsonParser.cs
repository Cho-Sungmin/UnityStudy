using System.Collections;
using System.Collections.Generic;


namespace JSON {

	enum DATA_TYPE {
		OBJECT = 0,
		LIST,
		STRING
	}
public class JsonParser
{
	public Hashtable m_hashtable;

	public JsonParser( byte[] json )
	{
		m_hashtable = new Hashtable();

		ParseData( json );
	}

	int GetType( byte[] value )
	{
		if( value[0] == '{' )
			return (int) DATA_TYPE.OBJECT;
		else if( value[0] == '[' )
			return (int) DATA_TYPE.LIST;
		else
			return (int) DATA_TYPE.STRING;
	}

	int GetType( string value )
	{
		if( value.StartsWith("{") )
			return (int) DATA_TYPE.OBJECT;
		else if( value.StartsWith("[") )
			return (int) DATA_TYPE.LIST;
		else
			return (int) DATA_TYPE.STRING;
	}

	int FindChar( byte[] str , char delim , int startIndex=0 )
	{
		int result = -1;

		if( str == null )
			return -1;

		for( int i=startIndex; i<str.Length; i++ )
		{
			if( str[i] == delim )
			{
				result = i;
				break;
			}
		}

		return result;
	}

	int FindCharEnd( byte[] str , char delim , int startIndex=0 )
	{
		int result = -1;

		if( str == null )
			return -1;

		for( int i=str.Length-1; i>=startIndex; i-- )
		{
			if( str[i] == delim )
			{
				result = i;
				break;
			}
		}

		return result;
	}

	void ParseData( byte[] json )
	{
		int begin = FindChar( json , '{' ) + 1;
		int end = FindCharEnd( json , '}' );
		int cursor = begin;
		int pivot = 0;
		byte[] ref_json = json;

		string key = "";
		byte[] value;

		int border = 0;

		while( cursor < end )
		{
			pivot = FindChar( ref_json , ':' , cursor );
			border = FindChar( ref_json , ',' , cursor );

			if( border < 0 )
				border = end;

			key = SubStr( ref_json , cursor , pivot );
			value = GetSubByte( ref_json , pivot+1 , border );

			
			if( GetType(value) == (int) DATA_TYPE.LIST )
			{
				List<string> values = ParseListValue( value );

				if( values.Count == 0 )
					return ;

				//--- CASE : {"key":[{"key":"value"}, ...]} ---//
				if( GetType( values[0] ) == (int) DATA_TYPE.OBJECT )
				{
					List<Hashtable> objects = new List<Hashtable>();
					for( int i=0; i<values.Count; i++ )
					{	
						byte[] obj = System.Text.Encoding.Default.GetBytes(values[i]);

						objects.Add( ParseObject(obj) );

					}
					m_hashtable.Add( key , objects );
				}
				//--- CASE : {"key":["stringA","stringB" , ...]} ---//
				else
					m_hashtable.Add( key , values );
			}
			//--- CASE : {"key":{"key":"value",...}} ---//
			else if( GetType(value) == (int) DATA_TYPE.OBJECT )
			{
				m_hashtable.Add( key , ParseObject(value) );
			}
			//--- CASE : {"key":"value",...} ---//
			else
			{
				m_hashtable.Add( key , value );
			}

			cursor = border + 1;
		}

	}

	//--- CASE : ["stringA","stringB" , ...] OR  [{"key"="value"}, ...] ---//
	List<string> ParseListValue( byte[] value )
	{
		List<string> result = new List<string>();
		int begin = FindChar( value , '[' ) + 1;
		int end = FindCharEnd( value , ']' );
		int cursor = begin;
		int border = 0;

		byte[] subValue = value;

		while( cursor < end )
		{
			border = FindChar( subValue , ',' , cursor );

			if( border < 0 )
				border = end;

			result.Add( SubStr( subValue , cursor , end ) );
		}

		return result;
	}
	
	//--- CASE : {"key"="value",...} ---//
	Hashtable ParseObject( byte[] json )
	{
		Hashtable result = new Hashtable();
		int begin = FindChar( json , '{' ) + 1;
		int end = FindCharEnd( json , '}' );
		int cursor = begin;
		int pivot = 0;
		byte[] ref_json = json;

		string key = "";
		string value = "";

		int border = 0;

		while( cursor < end )
		{
			pivot = FindChar( ref_json , ':' , cursor );
			border = FindChar( ref_json , ',' , cursor );

			if( border < 0 )
				border = end;

			key = SubStr( ref_json , cursor , pivot );
			value = SubStr( ref_json , pivot+1 , border );

			result.Add( key , value );

			cursor = border + 1;
		}

		return result;
	}

	string SubStr( byte[] str , int start , int end )
	{
		string result = System.Text.Encoding.Default.GetString(GetSubByte(str , start , end));

		return result;
	}

	byte[] GetSubByte( byte[] str , int start , int end )
	{
		int size = end - start;
		byte[] subStr = new byte[size];

		System.Buffer.BlockCopy( str , start , subStr , 0 , size );

		return subStr;
	}
}
}
