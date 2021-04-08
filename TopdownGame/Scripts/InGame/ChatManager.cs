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

	private void Awake()
	{
		chatPanel = transform.GetChild(0).GetComponent<Image>();

        chatContext = chatPanel.transform.GetChild(0).GetComponent<InputField>();
        
        sendButton = chatPanel.transform.GetChild(1).GetComponent<Button>();

        chatBubble = transform.GetChild(1).GetComponent<Image>();
        bubbleText = chatBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}
	// Start is called before the first frame update
	void Start()
    {
        chatBubble.gameObject.SetActive(false);
        SetOnChatPanel(false);
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Return ) )
        {
            if( chatPanel.IsActive() )
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
        chatPanel.gameObject.SetActive(value);
        
    }

    void ClearBubble()
    {
        chatBubble.gameObject.SetActive(false);
    }
}
