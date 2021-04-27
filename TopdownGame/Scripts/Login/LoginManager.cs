
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoginManager : MonoBehaviour
{
	InputField id_input;
	InputField pw_input;

	Button bt_sign;
	Button bt_assign;
	Button bt_reset;

	LoginSession loginSession;
	UserInfo userInfo;

	private void Awake()
	{

		Transform logInPanel = transform.GetChild(0);

		id_input = logInPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
		pw_input = logInPanel.GetChild(1).GetChild(1).GetComponent<InputField>();

		bt_sign = logInPanel.GetChild(2).GetComponent<Button>();
		bt_assign = logInPanel.GetChild(3).GetComponent<Button>();
		bt_reset = logInPanel.GetChild(4).GetComponent<Button>();
	}

	private void Start()
	{
		loginSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetLoginSession( this );
		loginSession.OpenSession();
		bt_sign.onClick.AddListener( RequstSignIn );
	}
	void RequstSignIn()
	{
		userInfo = new UserInfo();

		userInfo.m_id = id_input.text;
		userInfo.m_pw = pw_input.text;

		//--- Set payload of packet ---//
		OutputByteStream payload = new OutputByteStream( (UInt32)TCP.TCP.MAX_PAYLOAD_SIZE );
		userInfo.Write( payload );

		//--- Set header of packet ---//
		Header header = new Header();
		
		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_VERIFY;
		header.len = (UInt32) payload.GetLength();
		header.sessionID = loginSession.GetSessionID();

		OutputByteStream packet = new OutputByteStream( Header.SIZE + (UInt32)payload.GetLength() );
		header.Write( ref packet );
		packet.Write( payload.GetBuffer() , header.len );

		InputByteStream ibstream = new InputByteStream( packet );

		// Post REQ_MSG to msg_queue
		loginSession.PostMessage( ref ibstream );
		
	}

	public void OnSignIn( bool result )
	{
		if( result )
		{
			loginSession.userInfo.m_id = userInfo.m_id;
			loginSession.userInfo.m_pw = userInfo.m_pw;
			loginSession.userInfo.m_name = userInfo.m_name;
			loginSession.userInfo.m_age = userInfo.m_age;
			
			SceneManager.LoadScene( "LoadingLobby" , LoadSceneMode.Single );
			Debug.Log("로그인 성공!!!");
		}
		else
		{
			Debug.Log("로그인 실패!!!");
		}
	}

	private void OnDestroy()
	{
		loginSession.CloseSession();
		Resources.UnloadUnusedAssets();
	}


}
