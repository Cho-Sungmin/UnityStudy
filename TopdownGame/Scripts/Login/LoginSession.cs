

public class LoginSession : Session
{

	public UserInfo userInfo;
	
	public LoginSession( ref UserInfo _userInfo ) : base( 1091 )
	{
		userInfo = _userInfo;
	}

	public LoginSession GetInstance()
	{
		return this;
	}
	

	public override void Init() 
	{
		client.Init();
		client.RegisterHandler( (int) FUNCTION_CODE.ANY , Heartbeat );
		client.RegisterHandler( (int) FUNCTION_CODE.NOTI_WELCOME , NotiWelcomeInfo );
		client.RegisterHandler( (int) FUNCTION_CODE.REQ_VERIFY , ReqVerifyUserInfo );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_VERIFY_SUCCESS , ResVerifyUserInfo );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_VERIFY_FAIL , ResVerifyUserInfo );
	}

	void ReqVerifyUserInfo( InputByteStream packet )
	{
		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
		}
	}
	void ResVerifyUserInfo( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( ref packet );

		bool result = false;

		if( header.func == (ushort) FUNCTION_CODE.RES_VERIFY_SUCCESS )
		{
			result = true;
			userInfo.Read( packet );
		}

		if( result )
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene( "LoadingLobby" , UnityEngine.SceneManagement.LoadSceneMode.Single );
			LOG.printLog( "DEBUG" , "OK" , "OnSignIn()" );
		}
		else
		{
			LOG.printLog( "DEBUG" , "FAIL" , "OnSignIn()" );
		}
	}

	

	
}


