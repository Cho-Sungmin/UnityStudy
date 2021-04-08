using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
	Image talkPanel;
    Text talkLine;		// Child index - 0
	Image talkCursor;	// Child index - 1
	Image portrait;		// Child index - 2
	int	lineIndex = 0;
	bool isNPC = false;

	private void Awake()
	{
		talkPanel = gameObject.transform.GetChild(0).GetComponent<Image>();

		talkLine = talkPanel.transform.GetChild(0).GetComponent<Text>();
		talkCursor = talkPanel.transform.GetChild(1).GetComponent<Image>();
		portrait = talkPanel.transform.GetChild(2).GetComponent<Image>();

		SetOnTalkPanel(false);
		
	}
	public void Interaction( GameObject npc )
    {
		int id = npc.GetComponent<ObjectInfo>().id;
		isNPC = npc.GetComponent<ObjectInfo>().isNPC;

		LineInfo lineInfo;
		try {
			lineInfo = gameObject.GetComponent<TalkManagerScript>().GetLineInfo( id , lineIndex++ );
			talkLine.text = lineInfo.line;

			if( isNPC )
			{
				portrait.sprite = lineInfo.portrait;
			}

				
		}
		catch( System.IndexOutOfRangeException e) {
			Debug.Log( e.Message );
			talkLine.text = "";
		}
		

		if( talkLine.text != "" )
		{
			SetOnTalkPanel( true );
		}
		else
		{
			isNPC = false;
			lineIndex = 0;
			SetOnTalkPanel( false );
			
		}

    }

	void SetOnTalkPanel( bool value )
	{
		portrait.enabled = isNPC;
		talkCursor.enabled = value;
		talkLine.enabled = value;
		talkPanel.enabled = value;
	}
}
