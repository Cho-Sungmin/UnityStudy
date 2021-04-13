
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

		//--- Set packet header ---//
		Header header = new Header();
		
		header.type = (int) PACKET_TYPE.REQ;
		header.func = (int) FUNCTION_CODE.REQ_VERIFY;
		header.len = ( UInt32 ) userInfo.GetSize();
		header.sessionID = 0;

		//--- Set payload of packet ---//
		OutputByteStream obstream = new OutputByteStream( Header.SIZE + header.len );

		userInfo.Write( obstream );

		// Post REQ_MSG to msg_queue
		loginSession.PostMessage( new InputByteStream( obstream ) );
		
	}

	public void OnSignIn( bool result )
	{
		if( result )
		{
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
