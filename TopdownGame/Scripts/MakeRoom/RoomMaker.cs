using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMaker : MonoBehaviour
{
    LobbySession lobbySession;

    InputField titleInput;
    Dropdown capacityBox;
    Button createButton;
    

	private void Awake()
	{
        Transform parentTransform = transform.GetChild(0);

		titleInput = parentTransform.GetChild(0).GetChild(1).GetComponent<InputField>();
        capacityBox = parentTransform.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        createButton = parentTransform.GetChild(2).GetComponent<Button>();
        
        lobbySession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetLobbySession();

        createButton.onClick.AddListener( OnCreateButtonClick );
	}


	void OnCreateButtonClick()
    {
        byte[] title = System.Text.Encoding.Default.GetBytes(titleInput.text + ",");
        byte[] capacity = System.Text.Encoding.Default.GetBytes( capacityBox.options[capacityBox.value].text + "," );
        int len = title.Length + capacity.Length;

        Packet packet = new Packet( TCP.TCP.MAX_PAYLOAD_SIZE );

        packet.head.SetHead( (int) PACKET_TYPE.REQ , (int) FUNCTION_CODE.REQ_MAKE_ROOM , len );

        packet.SetPayload( capacity );
        packet.SetPayload( title, 0 , capacity.Length );

        lobbySession.RequestMakeRoom( packet );

    }

    private void OnDestroy()
	{
        if( lobbySession.IsOpen() )
		    lobbySession.CloseSession();
	}
 
}
