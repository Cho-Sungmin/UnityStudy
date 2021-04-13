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
        Room roomInfo = new Room();
        
        roomInfo.m_capacity = System.UInt32.Parse( capacityBox.options[capacityBox.value].text );
        roomInfo.m_id = 0;
        roomInfo.m_presentMembers = 0;
        roomInfo.m_title = titleInput.text;

        int len = roomInfo.GetSize();

        OutputByteStream packet = new OutputByteStream( (System.UInt32)len );
        roomInfo.Write( packet );

        lobbySession.RequestMakeRoom( new InputByteStream( packet ) );

    }

    private void OnDestroy()
	{
        if( lobbySession.IsOpen() )
		    lobbySession.CloseSession();
	}
 
}
