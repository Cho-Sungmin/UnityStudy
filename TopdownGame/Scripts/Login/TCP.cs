
using System.Net.Sockets;

namespace TCP {

	class TCP {
		public const int MAX_PAYLOAD_SIZE = 200;
		public static void SendPacket( Socket socket , InputByteStream packet )
		{
			int len = 0;
			int head_len = Header.SIZE;
			byte[] dataToSend = packet.GetBuffer();

			//--- Send header ---//
			while( head_len > 0 )
			{
				len = socket.Send( dataToSend , len , head_len , SocketFlags.None );
				head_len -= len;
			}

			if( head_len != 0 )
				throw new SocketException(); 

			len = 0;
			int body_len = packet.GetRemainLength() - Header.SIZE;

			//--- Send payload ---//
			while( body_len > 0 )
			{
				len = socket.Send( dataToSend , Header.SIZE + len , body_len , SocketFlags.None );
				body_len -= len;
			}

			packet.flush();

			if( body_len != 0 )
				throw new SocketException();
		}

		public static void RecvPacket( Socket socket , InputByteStream packet )
		{
			int len = 0;
			int head_len = Header.SIZE;
			byte[] dataToSend = packet.GetBuffer();
			
			//--- Receive header ---//
			do{
				len = socket.Receive( dataToSend , len , head_len , SocketFlags.None );
				head_len -= len;
			}while( head_len > 0 );

			if( head_len != 0 )
				throw new SocketException();

			len = 0;
			Header header = new Header();
			header.Read( packet );

			int body_len = (int)header.len;

			//--- Receive payload ---//
			do{
				len = socket.Receive( dataToSend , Header.SIZE + len , body_len , SocketFlags.None );
				body_len -= len;
			}while( body_len > 0 );

			if( body_len != 0 )
				throw new SocketException();

			
		}
	}
}
