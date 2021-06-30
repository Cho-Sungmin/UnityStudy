
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Client {
	public class TCP_Client
	{
		Socket m_serverSock;
		IPAddress m_serverAddr;

		Thread[] m_threadArr;
		bool m_threadFlag = false;

		System.Collections.Concurrent.ConcurrentQueue<InputByteStream> m_recvQueue;
		System.Collections.Concurrent.ConcurrentQueue<InputByteStream> m_sendQueue;

		Dictionary<int , System.Action<InputByteStream>> m_handlerMap;


		public virtual void Init()
		{
			//--- Init socket ---//
			m_serverAddr = IPAddress.Parse("192.168.0.6");


			//--- Init threads ---//
			m_threadArr = new Thread[1];
			m_threadArr[0] = new Thread( this.RecvProcedure );

			//--- Init queues ---//
			m_recvQueue = new System.Collections.Concurrent.ConcurrentQueue<InputByteStream>();
			m_sendQueue = new System.Collections.Concurrent.ConcurrentQueue<InputByteStream>();

			//--- Register handlers in this class ---//
			m_handlerMap = new Dictionary<int, System.Action<InputByteStream>>();
		}

	
		public void RunThreads()
		{
			for( int i=0; i<m_threadArr.Length; i++ )
			{
				m_threadArr[i].Start();
			}
		}

		public void RegisterHandler( int key , System.Action<InputByteStream> handler )
		{
			m_handlerMap.Add( key , handler );
		}

		public void Connect( int port )
		{
			m_threadFlag = true;
			m_threadArr[0] = new Thread( this.RecvProcedure );

			try {
				m_serverSock = new Socket( AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp );
				m_serverSock.Blocking = true;
				m_serverSock.Connect( m_serverAddr , port );

				LOG.printLog( "TCP" , "OK" , "Connect()" );
			}
			catch( System.Exception e )
			{
				UnityEngine.Debug.LogException( e );
				throw e;
			}
		}

		public void Disconnect()
		{
			m_threadFlag = false;
			ClearQueue();

			if( m_serverSock.Connected )
				m_serverSock.Disconnect(false);

			LOG.printLog( "TCP" , "NOTI" , "Disconnect()" );
		}

		void ClearQueue()
		{
			int cnt = m_recvQueue.Count;
			InputByteStream ibstream;

			for( int i=0; i<cnt; ++i )
				m_recvQueue.TryDequeue( out ibstream );
			for( int i=0; i<cnt; ++i )
				m_sendQueue.TryDequeue( out ibstream );
		}

		//--- Functions ---//
		public void PostMessage( ref InputByteStream msg )
		{
			//--- Enqueue msg to send ---//
			m_sendQueue.Enqueue( msg );
		}
		void ProcessMessage( ref InputByteStream msg )
		{
			Header header = new Header();
			header.Read( ref msg );
			msg.ReUse();

			//--- Process messages fetched ---//
			if( m_handlerMap.ContainsKey(header.func) )
				m_handlerMap[header.func]( msg );
		}
		void DispatchToSend()
		{
			InputByteStream packet;

			if( m_threadFlag && !m_sendQueue.IsEmpty )
			{
				if( m_sendQueue.TryDequeue( out packet ) )
					ProcessMessage( ref packet );
			}
		}
		

		void DispatchToRecv()
		{
			InputByteStream packet;

			if( m_threadFlag && !m_recvQueue.IsEmpty )
			{
				if( m_recvQueue.TryDequeue( out packet ) )
					ProcessMessage( ref packet );
			}
		}
		//--- Thread procedures ---//
		public void DispatchMessages()
		{
			DispatchToRecv();
			DispatchToSend();	
		}

		void RecvProcedure()
		{
			OutputByteStream obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
				
			while( m_threadFlag )
			{
				obstream.Flush();

				//--- Receive messages and enqueue data ---//
				try {
					Recv( obstream );
					m_recvQueue.Enqueue( new InputByteStream(obstream) );
				}
				catch( System.Net.Sockets.SocketException e )
				{
					UnityEngine.Debug.LogException( e );
				}

			}
		}

		public void Send( InputByteStream packet )
		{
			try{
				TCP.TCP.SendPacket( m_serverSock , packet );
				LOG.printLog( packet , LOG.TYPE.SEND );
			}
			catch( SocketException e )
			{
				Disconnect();
			}
		}

		public void Recv( OutputByteStream packet )
		{
			try{
				TCP.TCP.RecvPacket( m_serverSock , packet );
				InputByteStream ibstream = new InputByteStream( packet );
				LOG.printLog( ibstream , LOG.TYPE.RECV );
			}
			catch( SocketException e )
			{
				Disconnect();
			}
		}

	}
}