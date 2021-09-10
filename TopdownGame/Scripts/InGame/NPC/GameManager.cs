
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	IEnumerator gameObjectCoroutine;
	Button menuButton;	

	Image optionsPanel;
	Button continueButton;	// Child index - 0
	Button exitButton;		// Child index - 1

	Image talkPanel;
    Text talkLine;		// Child index - 0
	Image talkCursor;	// Child index - 1
	Image portrait;		// Child index - 2
	int	lineIndex = 0;
	bool isNPC = false;

	GameSession gameSession;

	private void Awake()
	{
		gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();

		//--- Attach player object ---//
		GameObjectInfo playerObject = gameSession.gameObjectManager.GetPlayerObject();
		UnityEngine.GameObject player = GameObjectInstantiator.InstantiateObject( playerObject , gameSession.gameObjectManager.GetObjectId( playerObject ) );
		player.AddComponent<PlayerScript>();

		//--- Panels about interaction ---//
		menuButton = gameObject.transform.GetChild(0).GetComponent<Button>();
		menuButton.onClick.AddListener( OnClickMenu );

		optionsPanel = gameObject.transform.GetChild(1).GetComponent<Image>();
			continueButton = optionsPanel.transform.GetChild(0).GetComponent<Button>();
			continueButton.onClick.AddListener( OnClickContinue );
			exitButton = optionsPanel.transform.GetChild(1).GetComponent<Button>();
			exitButton.onClick.AddListener( OnClickExit );

		talkPanel = gameObject.transform.GetChild(2).GetComponent<Image>();
			talkLine = talkPanel.transform.GetChild(0).GetComponent<Text>();
			talkCursor = talkPanel.transform.GetChild(1).GetComponent<Image>();
			portrait = talkPanel.transform.GetChild(2).GetComponent<Image>();

		SetOnTalkPanel(false);
		SetOnOptions(false);
		
	}
	private void Start()
	{
		gameObjectCoroutine = GameObjectCoroutine();
		StartCoroutine(gameObjectCoroutine);
	}

	public void OnClickMenu()
	{
		SetOnOptions(!optionsPanel.IsActive());
	}

	public void OnClickContinue()
	{
		SetOnOptions(false);
	}

	public void OnClickExit()
	{
		StopCoroutine(gameObjectCoroutine);
		gameSession.RequestQuitGame();
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
			talkLine.text = "";
			LOG.printLog( "EXCEPT" , "WARN" , e.Message + " : " + e.TargetSite ); 
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

	void SetOnOptions( bool value )
	{
		continueButton.gameObject.SetActive(value);
		exitButton.gameObject.SetActive(value);
		optionsPanel.gameObject.SetActive(value);
	}

	private void OnDestroy()
	{
		LOG.printLog( "DEBUG" , "MEMORY" , "OnDestroy() : GameManager" );
		gameSession.RequestDestroyAllGameObject();
		gameSession.CloseSession();
		Resources.UnloadUnusedAssets();
	}

	void CheckRottenObject()
	{
		GameObjectManager gameObjectMgr = gameSession.gameObjectManager;
		OutputByteStream obstream = new OutputByteStream(TCP.TCP.MAX_PAYLOAD_SIZE);
		
		var keys = gameObjectMgr.objectStateTable.Keys;
		List<uint> objectList = new List<uint>();

		//--- 갱신되지 않은 데이터 확인 ---//
		foreach( var key in gameObjectMgr.objectStateTable.Keys )
		{
			objectList.Add(key);
		}

		for(int i=0; i<objectList.Count; ++i)
		{
			uint key = objectList[i];
			if( gameObjectMgr.objectStateTable[key] == 0 )
			{
				ReplicationHeader header = new ReplicationHeader( ReplicationAction.DESTROY , key , 0 );
				header.Write(obstream);
				InputByteStream ibstream = new InputByteStream(obstream);
				gameSession.replicationManager.Replicate(ibstream);
			}
			else
				gameObjectMgr.objectStateTable[key] = 0;
		}

		
	}

	IEnumerator GameObjectCoroutine()
	{
		while(gameSession.IsOpen())
		{
			Debug.Log("코루틴 시작");
			yield return null;
			CheckRottenObject();
			yield return new WaitForSeconds(0.5f);

			
			Debug.Log("코루틴 끝");
		}
	}

}
