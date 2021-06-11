
using System.Net.Sockets;

namespace TCP {

	class TCP {
		public const int MAX_PAYLOAD_SIZE = 200;
		public static void SendPacket( Socket socket , InputByteStream packet )
		{
			int len = 0;
			int offset = 0;
			int head_len = Header.SIZE;

			byte[] bufferToSend = new byte[head_len];
			packet.Read( bufferToSend , head_len );

			//--- Send header ---//
			while( head_len > 0 )
			{
				try{
					len = socket.Send( bufferToSend , offset , head_len , SocketFlags.None );
					head_len -= len;
					offset += len;
				}
				catch( SocketException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
				catch( System.ObjectDisposedException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
			}

			if( head_len != 0 )
			{
				LOG.printLog( "TCP" , "ERROR" , "Data dropped" );
				throw new SocketException(); 
			}

			packet.ReUse();
			Header header = new Header();
			header.Read( ref packet );

			len = 0;
			offset = 0;
			int body_len = header.len;

			bufferToSend = new byte[body_len];
			packet.Read( bufferToSend , body_len );

			//--- Send payload ---//
			while( body_len > 0 )
			{
				try{
					len = socket.Send( bufferToSend , offset , body_len , SocketFlags.None );
					body_len -= len;
					offset += len;
				}
				catch( SocketException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
				catch( System.ObjectDisposedException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
			}

			if( body_len != 0 )
			{
				LOG.printLog( "TCP" , "ERROR" , "Data dropped" );
				throw new SocketException(); 
			}
		}

		public static void RecvPacket( Socket socket , OutputByteStream packet )
		{
			int len = 0;
			int offset = 0;
			int head_len = Header.SIZE;
			byte[] bufferToRecv = new byte[head_len];
			
			//--- Receive header ---//
			while( head_len > 0 )
			{
				try{
					len = socket.Receive( bufferToRecv , offset , head_len , SocketFlags.None );
					head_len -= len;
					offset += len;
				}
				catch( SocketException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
				catch( System.ObjectDisposedException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
			}

			if( head_len != 0 )
			{
				LOG.printLog( "TCP" , "ERROR" , "Data dropped" );
				throw new SocketException(); 
			}
			else
				packet.Write( bufferToRecv , Header.SIZE );

			InputByteStream ibstream = new InputByteStream( packet );
			Header header = new Header();
			header.Read( ref ibstream );

			len = 0;
			offset = 0;
			int body_len = header.len;
			bufferToRecv = new byte[body_len];

			//--- Receive payload ---//
			while( body_len > 0 )
			{
				try{
					len = socket.Receive( bufferToRecv , offset , body_len , SocketFlags.None );
					body_len -= len;
					offset += len;
				}
				catch( SocketException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
				catch( System.ObjectDisposedException e )
				{
					LOG.printLog( "TCP" , "ERROR" , e.Message );
					throw e;
				}
			}

			if( body_len != 0 )
			{
				LOG.printLog( "TCP" , "ERROR" , "Data dropped" );
				throw new SocketException(); 
			}
			else
				packet.Write( bufferToRecv , header.len );
		}
	}
}
