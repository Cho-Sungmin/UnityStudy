

public class LoginSession : Session
{
	LoginManager loginManager;

	public UserInfo userInfo;
	
	public LoginSession( ref UserInfo _userInfo ) : base( 1091 )
	{
		userInfo = _userInfo;
	}

	public LoginSession GetInstance( LoginManager mgr )
	{
		loginManager = mgr;

		return this;
	}
	

	public override void Init() 
	{
		client.Init();
		client.RegisterHandler( (int) FUNCTION_CODE.REQ_VERIFY , ReqVerifyUserInfo );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_VERIFY_SUCCESS , ResVerifyUserInfo );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_VERIFY_FAIL , ResVerifyUserInfo );
	}

	void ReqVerifyUserInfo( Packet packet )
	{
		//Debug.Log("ReqVerifyUserInfo()");
		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}
	void ResVerifyUserInfo( Packet packet )
	{
		bool result = false;

		if( packet.head.func == (int) FUNCTION_CODE.RES_VERIFY_SUCCESS )
		{
			result = true;
			userInfo.SetBytes( packet.data );
		}

		loginManager.OnSignIn( result );
	}

	

	
}


