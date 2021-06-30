using System;

public struct Header
{
	public byte type;
	public UInt16 func;
	public int len;
	public UInt32 sessionID;

	public const int SIZE = sizeof(byte) + sizeof(UInt16) + sizeof(int) + sizeof(UInt32);

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

	public void InsertFront( ref OutputByteStream stream )
	{
		stream.Shift( Header.SIZE );
		int newCursor = stream.GetLength();
		stream.Flush();

		stream.Write( type );
		stream.Write( func );
		stream.Write( len );
		stream.Write( sessionID );

		stream.SetCursor( newCursor );
	}

		
	public static byte[] GetBytes( InputByteStream stream )
	{
		byte[] bytes = new byte[SIZE];
		stream.Read( bytes , SIZE );

		return bytes;
	}

	public void Display()
	{
		UnityEngine.Debug.Log("[" + type + "] " + "[" + func + "] " + "[" + len + "] " + "[" + sessionID + "]\n");			
	}

}

enum PACKET_TYPE : byte
{
	HB = 0,
	NOTI = 1,
	REQ,
	RES,
	MSG
}

enum FUNCTION_CODE : UInt16 {

	//--- Request ---//
	REQ_VERIFY = 0,
	REQ_SIGN_IN,
	REQ_MAKE_ROOM,
	REQ_ENTER_LOBBY,
	REQ_ROOM_LIST,
	REQ_JOIN_GAME,
	REQ_REPLICATION,

	//--- Response ---//
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

	//--- Others ---//
	NOTI_WELCOME,
	NOTI_REPLICATION,
	NOTI_BYE,
	CHAT,
	SUCCESS,
	FAIL,
	REJECT,
	EXIT, 
	ANY,
	NONE,

	MAX_CODE
};

