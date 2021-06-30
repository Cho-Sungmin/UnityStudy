using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatManager: MonoBehaviour
{
    Image chatPanel;
    InputField chatContext;
    Button sendButton;
    Image chatBubble;
    TextMeshProUGUI bubbleText;
    GameSession gameSession;
    OutputByteStream obstream;
    uint playerObjectId;

	private void Awake()
	{
		chatPanel = gameObject.GetComponent<Image>();
            chatContext = transform.GetChild(0).GetComponent<InputField>();
            sendButton = transform.GetChild(1).GetComponent<Button>();
            chatBubble = transform.GetChild(2).GetComponent<Image>();
            bubbleText = chatBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}
	// Start is called before the first frame update
	void Start()
    {
        SetOnChatPanel(false);
        gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
        playerObjectId = gameSession.gameObjectManager.GetPlayerObjectId();
        obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Return ) )
        {
            if( chatPanel.enabled )
            {
                SendMSG(chatContext.text);
                DisplayMSG(chatContext.text);
                chatContext.text = "";
                SetOnChatPanel(false);
            }
            else
            {
				SetOnChatPanel(true);
                chatContext.ActivateInputField();
            }
        }
    }

    void SendMSG( string contents )
    {
        obstream.Write( playerObjectId );
        obstream.Write( contents );

        Header header = new Header();
        header.type = (byte) PACKET_TYPE.MSG;
        header.func = (ushort) FUNCTION_CODE.CHAT;
        header.len = obstream.GetLength();
        header.sessionID = gameSession.GetSessionID();

        header.InsertFront( ref obstream );
        
        InputByteStream packet = new InputByteStream( obstream );
        gameSession.client.Send( packet );

        obstream.Flush();
    }
    
    void DisplayMSG( string contents )
    {
        if( IsInvoking("ClearBubble") )
            CancelInvoke();

        bubbleText.text = contents;
        chatBubble.gameObject.SetActive(true);

        Invoke("ClearBubble" , 3);
        
    }

    void SetOnChatPanel( bool value )
    {
        chatPanel.enabled = value;
        chatContext.gameObject.SetActive(value);
        sendButton.gameObject.SetActive(value);
    }

    void ClearBubble()
    {
        chatBubble.gameObject.SetActive(false);
    }
}
