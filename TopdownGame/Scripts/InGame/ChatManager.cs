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
    GameSession gameSession;
    OutputByteStream obstream;
    uint playerObjectId;
    ChatBubble bubble;
    PlayerObject playerInfo;

	private void Awake()
	{
		chatPanel = gameObject.GetComponent<Image>();
            chatContext = transform.GetChild(0).GetComponent<InputField>();
            sendButton = transform.GetChild(1).GetComponent<Button>();
	}
	// Start is called before the first frame update
	void Start()
    {
        SetOnChatPanel(false);
        gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
        playerObjectId = gameSession.gameObjectManager.GetPlayerObjectId();
        obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );

        GameObject obj = (GameObject) Instantiate( Resources.Load("Prefabs/Bubble") , GameObject.Find("ChatBubble").transform );
		obj.SetActive(true);
		obj.name = name + "_bubble";
		bubble = obj.transform.GetComponent<ChatBubble>();
        playerInfo = (PlayerObject) gameSession.gameObjectManager.GetGameObject(playerObjectId);
    }

	private void FixedUpdate()
	{
		bubble.position.x = playerInfo.position.x;
		bubble.position.y = playerInfo.position.y + 2.4f;
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

        bubble.gameObject.SetActive(true);
        bubble.SetContents( contents );

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
        bubble.gameObject.SetActive(false);
    }
}
