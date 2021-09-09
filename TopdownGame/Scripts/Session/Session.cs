

public class Session
{
	int PORT;
	uint id = 0;
	public Client.TCP_Client client;
	
	bool isOpen = false;
	
	public Session( int port )
	{
		PORT = port;
		client = new Client.TCP_Client();
	}

	public virtual void Init()
	{
		client.Init();
	}

	public bool IsOpen()
	{
		return isOpen;
	}

	void Run()
    {
		//--- Main thread routine ---//
		while( isOpen )
			client.DispatchMessages();
    }

	public void OpenSession()
	{
		try {
			client.Connect( PORT );
			isOpen = true;
			client.RunThreads();
		}
		catch( System.Exception e )
		{
		}
	}
	public void CloseSession()
	{
		isOpen = false;
		NotiBye();
		client.Disconnect();
	}

	public uint GetSessionID()
	{ return id; }

	//--- Functions ---//
	public void PostMessage( ref InputByteStream msg )
	{
		//--- Enqueue msg to send ---//
		client.PostMessage( ref msg );
	}

	public void NotiWelcomeInfo( InputByteStream packet )
	{
		Header header = new Header(); header.Read( ref packet );

		id = header.sessionID;
	}

	public void NotiBye()
	{
		Header header = new Header();
		header.type = (byte) PACKET_TYPE.NOTI;
		header.func = (ushort) FUNCTION_CODE.NOTI_BYE;
		header.len = 0;
		header.sessionID = id;

		OutputByteStream obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		header.Write( ref obstream );

		client.Send( new InputByteStream( obstream ) );
	}

	public void Heartbeat( InputByteStream packet )
	{
		client.Send( packet );
	}
	
}

