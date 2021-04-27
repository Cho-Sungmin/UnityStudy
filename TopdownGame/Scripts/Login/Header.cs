using System;

public struct Header
{
	public byte type;
	public UInt16 func;
	public UInt32 len;
	public UInt32 sessionID;

	public const int SIZE = sizeof(byte) + sizeof(UInt16) + sizeof(UInt32) + sizeof(UInt32);

	public void Read( ref InputByteStream stream )
	{
		stream.Read( out type );
		stream.Read( out func );
		stream.Read( out len );
		stream.Read( out sessionID );
	}

	public void Write( ref OutputByteStream stream )
	{
		stream.Write( type );
		stream.Write( func );
		stream.Write( len );
		stream.Write( sessionID );
	}
		
	public static byte[] GetBytes( InputByteStream stream )
	{
		byte[] bytes = new byte[SIZE];
		stream.Read( bytes , SIZE );

		return bytes;
	}

	public void Display()
	{
		System.Console.WriteLine("[" + type + "] " + "[" + func + "] " + "[" + len + "] " + "[" + sessionID + "]\n");			
	}

}

public enum PACKET_TYPE : byte
{
	HB = 0,
	NOTI = 1,
	REQ,
	RES,
	MSG
}

public enum FUNCTION_CODE : UInt16
{
	//--- Function map keys ---//
	REQ_VERIFY = 0,
	REQ_SIGN_IN,
	REQ_MAKE_ROOM,
	REQ_ENTER_LOBBY,
	REQ_ROOM_LIST,
	REQ_JOIN_GAME,

		
	RES_VERIFY_SUCCESS,
	RES_SIGN_IN_SUCCESS,
	RES_MAKE_ROOM_SUCCESS,
	RES_ENTER_LOBBY_SUCCESS,
	RES_ROOM_LIST_SUCCESS,
	RES_JOIN_GAME_SUCCESS,
	RES_VERIFY_FAIL,
	RES_SIGN_IN_FAIL,
	RES_MAKE_ROOM_FAIL,
	RES_ENTER_LOBBY_FAIL,
	RES_ROOM_LIST_FAIL,
	RES_JOIN_GAME_FAIL,
	WELCOME,
	SUCCESS,
	FAIL,
	REJECT,
	EXIT, 
	ANY,
	NONE
}