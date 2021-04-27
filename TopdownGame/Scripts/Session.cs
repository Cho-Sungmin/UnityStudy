

public class Session
{
	int PORT;
	uint id = 0;
	public Client.TCP_Client client;
	
	bool isOpen = false;
	//Thread messageThread;
	
	public Session( int port )
	{
		PORT = port;
		client = new Client.TCP_Client();
		//messageThread = new Thread( Run );
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
			//messageThread.Start();
			client.RunThreads();
		}
		catch( System.Exception e )
		{
		}
	}
	public void CloseSession()
	{
		isOpen = false;
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
		packet.flush();
	}
	
}

