
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

				Header header = new Header();
				header.Read( ref packet );

			len = 0;
			int body_len = (int) header.len;

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

		public static void RecvPacket( Socket socket , out InputByteStream packet )
		{
			int len = 0;
			int head_len = Header.SIZE;
			InputByteStream head = new InputByteStream( head_len );
			byte[] headerToRecv = head.GetBuffer();
			
			//--- Receive header ---//
			do{
				len = socket.Receive( headerToRecv , len , head_len , SocketFlags.None );
				head_len -= len;
			}while( head_len > 0 );

			if( head_len != 0 )
				throw new SocketException();

			len = 0;
			Header header = new Header();
			header.Read( ref head );

			int body_len = (int)header.len;
			OutputByteStream obstream = new OutputByteStream( Header.SIZE + body_len );
			header.Write( ref obstream );

			byte[] payloadToRecv = new byte[body_len];

			//--- Receive payload ---//
			while( body_len > 0 ){
				len = socket.Receive( payloadToRecv , len , body_len , SocketFlags.None );
				body_len -= len;
			}

			obstream.Write( payloadToRecv , payloadToRecv.Length );
			packet = new InputByteStream( obstream );

			if( body_len != 0 )
				throw new SocketException();
			
		}
	}
}
