
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
		loginSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetLoginSession();
		loginSession.OpenSession();
		bt_sign.onClick.AddListener( RequstSignIn );
	}
	void RequstSignIn()
	{
		userInfo = new UserInfo();

		userInfo.m_id = id_input.text;
		userInfo.m_pw = pw_input.text;

		//--- Set payload of packet ---//
		OutputByteStream payload = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		userInfo.Write( payload );

		//--- Set header of packet ---//
		Header header = new Header();
		
		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_VERIFY;
		header.len = payload.GetLength();
		header.sessionID = loginSession.GetSessionID();

		OutputByteStream packet = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		header.Write( ref packet );
		packet.Write( payload.GetBuffer() , header.len );

		InputByteStream ibstream = new InputByteStream( packet );

		// Post REQ_MSG to msg_queue
		loginSession.PostMessage( ref ibstream );
	}

	private void OnDestroy()
	{
		loginSession.CloseSession();
		Resources.UnloadUnusedAssets();
	}


}
