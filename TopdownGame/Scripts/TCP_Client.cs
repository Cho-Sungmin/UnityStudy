using System.Collections;
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
		bool m_threadFlag = true;

		System.Collections.Concurrent.ConcurrentQueue<Packet> m_recvQueue;
		System.Collections.Concurrent.ConcurrentQueue<Packet> m_sendQueue;

		Dictionary<int , System.Action<Packet>> m_handlerMap;


		public virtual void Init()
		{

			//--- Init socket ---//
			m_serverSock = new Socket( AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp );
			m_serverAddr = IPAddress.Parse("192.168.0.5");

			m_serverSock.Blocking = true;


			//--- Init threads ---//
			m_threadArr = new Thread[1];
			m_threadArr[0] = new Thread( this.RecvProcedure );

			//--- Init queues ---//
			m_recvQueue = new System.Collections.Concurrent.ConcurrentQueue<Packet>();
			m_sendQueue = new System.Collections.Concurrent.ConcurrentQueue<Packet>();

			//--- Register handlers in this class ---//
			m_handlerMap = new Dictionary<int, System.Action<Packet>>();
		
		}

	
		public void RunThreads()
		{
			for( int i=0; i<m_threadArr.Length; i++ )
			{
				m_threadArr[i].Start();
			}
		}

		public void RegisterHandler( int key , System.Action<Packet> handler )
		{
			m_handlerMap.Add( key , handler );
		}

		public void Connect( int port )
		{
			try {
				m_serverSock.Connect( m_serverAddr , port );
				System.Console.Write("서버 연결  성공!!!");
			}
			catch( System.Exception e )
			{
				System.Console.Write( e.Message );
				throw e;
			}
		}

		public void Disconnect()
		{
			m_threadFlag = false;
			if( m_serverSock.Connected )
				m_serverSock.Disconnect(false);
		}

		//--- Functions ---//
		public void PostMessage( Packet msg )
		{
			//--- Enqueue msg to send ---//
			System.Console.WriteLine("PostMessage");
			m_sendQueue.Enqueue( msg );
		}
		void ProcessMessage( ref Packet msg )
		{
			//--- Process messages fetched ---//
			if( m_handlerMap.ContainsKey(msg.head.func) )
				m_handlerMap[msg.head.func]( msg );
		}
		void DispatchToSend()
		{
			Packet packet;

			if( m_threadFlag && !m_sendQueue.IsEmpty )
			{
				if( m_sendQueue.TryDequeue( out packet ) )
					ProcessMessage( ref packet );
			}
		}

		void DispatchToRecv()
		{
			Packet packet;

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
			System.Console.WriteLine("Start RecvProcedure()");
			Packet packet = new Packet( TCP.TCP.MAX_PAYLOAD_SIZE );

			while( m_threadFlag )
			{
				//--- Receive messages and enqueue data ---//
				try {
					TCP.TCP.RecvPacket( m_serverSock , ref packet );
					m_recvQueue.Enqueue( packet );
					packet.DisplayPacket();
				}
				catch( System.Net.Sockets.SocketException e )
				{
					System.Console.WriteLine( e.Message + " in Function '" + e.TargetSite + "'");
				}

			}
		}

		public void Send( Packet packet )
		{
			TCP.TCP.SendPacket( m_serverSock , packet );
		}

		public void Recv( ref Packet packet )
		{
			TCP.TCP.RecvPacket( m_serverSock , ref packet );
		}
	}
}