

public class LOG
{
	public enum TYPE {
		SEND = 0,
		RECV
	}

	static public void printLog( InputByteStream packet , TYPE type )
	{
        packet.ReUse();

		string typeStr = string.Format( "{0,-10}" , "<TCP>" );
		string stateStr = "";
		string logStr = GetLog( packet );

		switch( type )
		{
			case TYPE.SEND :
				stateStr = string.Format( "{0,-13}" , new string( '<' , 8 ) );
				break;
			case TYPE.RECV :
				stateStr = string.Format( "{0,-13}" , new string( '>' , 8 ) );
				break;
			default :
				break;
		}

        packet.ReUse();
        UnityEngine.Debug.Log( typeStr + stateStr + logStr );
	}

	static public void printLog( string type , string state , string contents )
	{
		string typeStr = '<' + type + '>';
		string stateStr = '[' + state + ']';
		string logStr = string.Format( "{0,-10} {1,-10} {2}" , typeStr , stateStr , contents );
		
		switch( state )
		{
			case "ERROR" :
				UnityEngine.Debug.LogError( logStr );
				break;
			case "WARN" :
				UnityEngine.Debug.LogWarning( logStr );
				break;
			default :
				UnityEngine.Debug.Log( logStr );
				break;
		}
	}

    static string GetLog( InputByteStream packet )
    {
        Header header = new Header(); header.Read( ref packet );
        byte[] payload = new byte[header.len];
        packet.Read( payload , header.len );

        string headerStr = GetHeaderString( header );
        string logStr = GetHexaString( payload );

        return headerStr + logStr;
    }

    static string GetHeaderString( Header header )
	{
		string typeStr =  GetTypeString( (PACKET_TYPE) header.type );
		string funcStr = GetFuncString( (FUNCTION_CODE) header.func );
        string lenStr = System.Convert.ToString( header.len );
        string sessionStr = System.Convert.ToString( header.sessionID );

        return '[' + typeStr + ']' + 
               '[' + funcStr + ']' + 
               '[' + lenStr + ']' +
               '[' + sessionStr + ']'; 
	}

    static string GetHexaString( byte[] strArr )
    {
        string result = "[";

        for( int i=0; i<strArr.Length; ++i )
        {
            result += strArr[i].ToString("X2");
            result += ' ';
        }

        result += ']';

        return result;
    }

	static string GetTypeString( PACKET_TYPE type )
	{
		string typeStr = "";

		switch (type)
		{
		case PACKET_TYPE.HB :
			typeStr = "HB";
			break;
		case PACKET_TYPE.NOTI :
			typeStr = "NOTI";
			break;
		case PACKET_TYPE.REQ :
			typeStr = "REQ";
			break;
		case PACKET_TYPE.RES :
			typeStr = "RES";
			break;
		case PACKET_TYPE.MSG :
			typeStr = "MSG";
			break;  
		default:
			typeStr = "UNDEFINED";
			break;
		}

		return typeStr;
	}

	static string GetFuncString( FUNCTION_CODE func )
    {
        string typeStr = "";

        switch (func)
        {
        case FUNCTION_CODE.REQ_VERIFY :
            typeStr = "REQ_VERIFY";
            break;
        case FUNCTION_CODE.REQ_SIGN_IN :
            typeStr = "REQ_SIGN_IN";
            break;
        case FUNCTION_CODE.REQ_MAKE_ROOM :
            typeStr = "REQ_MAKE_ROOM";
            break;
        case FUNCTION_CODE.REQ_ENTER_LOBBY :
            typeStr = "REQ_ENTER_LOBBY";
            break;
        case FUNCTION_CODE.REQ_ROOM_LIST :
            typeStr = "REQ_ROOM_LIST";
            break;
        case FUNCTION_CODE.REQ_JOIN_GAME :
            typeStr = "REQ_JOIN_GAME";
            break;
        case FUNCTION_CODE.REQ_REPLICATION :
            typeStr = "REQ_REPLICATION";
            break;
        case FUNCTION_CODE.RES_VERIFY_SUCCESS :
            typeStr = "RES_VERIFY_SUCCESS";
            break;
        case FUNCTION_CODE.RES_SIGN_IN_SUCCESS :
            typeStr = "RES_SIGN_IN_SUCCESS";
            break;
        case FUNCTION_CODE.RES_MAKE_ROOM_SUCCESS :
            typeStr = "RES_MAKE_ROOM_SUCCESS";
            break;  
        case FUNCTION_CODE.RES_ENTER_LOBBY_SUCCESS :
            typeStr = "RES_ENTER_LOBBY_SUCCESS";
            break;
        case FUNCTION_CODE.RES_ROOM_LIST_SUCCESS :
            typeStr = "RES_ROOM_LIST_SUCCESS";
            break;
        case FUNCTION_CODE.RES_JOIN_GAME_SUCCESS :
            typeStr = "RES_JOIN_GAME_SUCCESS";
            break;
        case FUNCTION_CODE.RES_VERIFY_FAIL :
            typeStr = "RES_VERIFY_FAIL";
            break;
        case FUNCTION_CODE.RES_SIGN_IN_FAIL :
            typeStr = "RES_SIGN_IN_FAIL";
            break;
        case FUNCTION_CODE.RES_MAKE_ROOM_FAIL :
            typeStr = "RES_MAKE_ROOM_FAIL";
            break;
        case FUNCTION_CODE.RES_ENTER_LOBBY_FAIL :
            typeStr = "RES_ENTER_LOBBY_FAIL";
            break;
        case FUNCTION_CODE.RES_ROOM_LIST_FAIL :
            typeStr = "RES_ROOM_LIST_FAIL";
            break;  
        case FUNCTION_CODE.RES_JOIN_GAME_FAIL :
            typeStr = "RES_JOIN_GAME_FAIL";
            break;
        case FUNCTION_CODE.NOTI_WELCOME :
            typeStr = "NOTI_WELCOME";
            break;
        case FUNCTION_CODE.NOTI_REPLICATION :
            typeStr = "NOTI_REPLICATION";
            break;
        case FUNCTION_CODE.SUCCESS :
            typeStr = "SUCCESS";
            break;
        case FUNCTION_CODE.FAIL :
            typeStr = "FAIL";
            break;
        case FUNCTION_CODE.REJECT :
            typeStr = "REJECT";
            break;  
        case FUNCTION_CODE.EXIT :
            typeStr = "EXIT";
            break;
        case FUNCTION_CODE.ANY :
            typeStr = "ANY";
            break;
        case FUNCTION_CODE.NONE :
            typeStr = "NONE";
            break;  
        default:
            typeStr = "UNDEFINED";
            break;
        }

        return typeStr;
    }

}
