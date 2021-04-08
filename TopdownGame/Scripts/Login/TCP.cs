using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TCP {

	class TCP {
		public const int MAX_PAYLOAD_SIZE = 200;
		public static void SendPacket( Socket socket , Packet packet )
		{
			int len = 0;
			int head_len = HEAD.SIZE;

			while( head_len > 0 )
			{
				len = socket.Send( packet.head.GetBytes() , len , head_len , SocketFlags.None );
				head_len -= len;
			}

			if( head_len != 0 )
				throw new SocketException();

			len = 0;
			int body_len = packet.head.len;

			while( body_len > 0 )
			{
				len = socket.Send( packet.data , len , body_len , SocketFlags.None );
				body_len -= len;
			}

			if( body_len != 0 )
				throw new SocketException();
		}

		public static void RecvPacket( Socket socket , ref Packet packet )
		{
			int len = 0;
			int head_len = HEAD.SIZE;
			

			byte[] data_buf = new byte[ head_len + packet.data.Length ];


			
			do{
				len = socket.Receive( data_buf , len , head_len , SocketFlags.None );
				head_len -= len;
			}while( head_len > 0 );

			if( head_len != 0 )
				throw new SocketException();

			len = 0;
			int body_len = data_buf[8];

			
			do{
				len = socket.Receive( data_buf , HEAD.SIZE + len , body_len , SocketFlags.None );
				body_len -= len;
			}while( body_len > 0 );

			if( body_len != 0 )
				throw new SocketException();

			packet.SetPacket( data_buf );
		}
	}
}
