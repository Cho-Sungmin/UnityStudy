using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Session
{
	int PORT;
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

	//--- Functions ---//
	public void PostMessage( InputByteStream msg )
	{
		//--- Enqueue msg to send ---//
		System.Console.WriteLine("PostMessage");
		client.PostMessage( msg );
	}
	
}

