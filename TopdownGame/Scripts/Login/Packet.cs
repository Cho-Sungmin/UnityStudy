using System.Collections;
using System.Collections.Generic;

	
	public struct HEAD
	{
		public int type;
		public int func;
		public int len;
		public int sessionID;

		public const int SIZE = sizeof(int) * 4;

		public HEAD( int _t=0 , int _f=0 , int _l=0 , int _s=0 )
		{
			type = _t;
			func = _f;
			len = _l;
			sessionID = _s;
		}

		public void SetHead( int _t , int _f , int _l )
		{
			type = _t;
			func = _f;
			len = _l;
		}

		public byte[] GetBytes()
		{
			byte[] src;
			byte[] dest = new byte[SIZE];

			int size = sizeof(int);
			int offset = 0;

			try {
				src = System.BitConverter.GetBytes(type);
				System.Buffer.BlockCopy( src , 0 , dest , offset , size );
				offset += size;

				src = System.BitConverter.GetBytes(func);
				System.Buffer.BlockCopy( src , 0 , dest , offset , size );
				offset += size;

				src = System.BitConverter.GetBytes(len);
				System.Buffer.BlockCopy( src , 0 , dest , offset , size );
				offset += size;

				src = System.BitConverter.GetBytes(sessionID);
				System.Buffer.BlockCopy( src , 0 , dest , offset , size );

				return dest;
			}
			catch( System.ArgumentOutOfRangeException e )
			{
				System.Console.WriteLine( e.Message + " in Function '" + e.TargetSite + "'");

				return null;
			}
		}

		public void SetHead( byte[] head , int startIndex = 0 )
		{
			int int32Size = sizeof(int);
			int offset = startIndex;

			type = System.BitConverter.ToInt32( head , offset );
			offset += int32Size;

			func = System.BitConverter.ToInt32( head , offset );
			offset += int32Size;

			len = System.BitConverter.ToInt32( head , offset );
			offset += int32Size;

			sessionID = System.BitConverter.ToInt32( head , offset );
		}


	

	}


	public struct Packet
	{
		public HEAD head;
		public byte[] data;

		public Packet( int max_len )
		{
			head = new HEAD();
			data = new byte[ max_len ];
		}

		public void SetPacket( byte[] array )
		{
			head.SetHead( array );
			SetPayload( array , HEAD.SIZE );
		}

		public void SetPayload( byte[] array , int startIndex = 0 , int offset = 0 )
		{
			try {
				System.Buffer.BlockCopy( array , startIndex , data , offset , array.Length - startIndex );
			}
			catch( System.ArgumentOutOfRangeException e )
			{
				System.Console.WriteLine( e.Message + " in Function '" + e.TargetSite + "'");
				//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
			}
		}

		public void DisplayPacket()
		{
			System.Console.WriteLine("[" + head.type + "] " + "[" + head.func + "] " + "[" + head.len + "] " + "[" + head.sessionID + "]\n" 
						+ "[" + System.Text.Encoding.Default.GetString( data ) + "]\n" );
					
		}
	}
	public enum PACKET_TYPE : int
	{
		HB	= 0,
		NOTI = 1,
		REQ	,
		RES	,
		MSG
	}

	public enum FUNCTION_CODE : int
	{
		//--- Function Key ---//
		REQ_VERIFY	= 0	,
		REQ_SIGN_IN		,
		REQ_MAKE_ROOM	,
		REQ_ENTER_LOBBY	,
		REQ_ROOM_LIST	,
		REQ_JOIN_GAME	,

		//--- Result Code ---//
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
		SUCCESS	,
		FAIL	,
		REJECT	,
		EXIT	, 
		ANY		,
		NONE
	}