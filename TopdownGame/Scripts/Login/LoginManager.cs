
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

		userInfo.SetId( id_input.text );
		userInfo.SetPw( pw_input.text );

		int payload_len = userInfo.GetId().Length + userInfo.GetPw().Length;

		//--- Set packet header ---//
		HEAD header = new HEAD( (int)PACKET_TYPE.REQ , (int)FUNCTION_CODE.REQ_VERIFY , payload_len , 0 );

		//--- Convert string to bytes ---//
		byte[] tmp_data = new byte[payload_len];
		System.Buffer.BlockCopy( userInfo.GetBytes() , 0 , tmp_data , 0 , payload_len );

		//--- Set payload of packet ---//
		Packet packet = new Packet( TCP.TCP.MAX_PAYLOAD_SIZE );
		packet.head = header;
		packet.SetPayload( tmp_data );

		// Post REQ_MSG to msg_queue
		loginSession.PostMessage( packet );
		
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
