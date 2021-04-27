using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
	LobbySession lobbySession;

	Transform listPanel;
	List<GameObject> roomObjectList;
	Text roomTitle;
	Button enterButton;

	private void Awake()
	{
		roomObjectList = new List<GameObject>();
	}

	private void Start()
	{
		lobbySession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetLobbySession(this);
		lobbySession.OpenSession();

		//--- Get components ---//
		listPanel = transform.GetChild(0);

		
		//--- Init components ---//
		int numOfRooms = lobbySession.GetRoomList().Count;

		for( int i=0; i<listPanel.childCount; i++ )
		{
			roomObjectList.Add( listPanel.GetChild(i).gameObject );

			//--- Set button event handler ---//
			roomObjectList[i].transform.GetChild(0).GetComponent<Button>().onClick.AddListener( MakeRoom );
		}

		//--- CASE : Rooms exist in the list ---//
		if( numOfRooms != 0 )
		{
			for( int i=0; i<numOfRooms; i++ )
			{
					
				//--- Set component with room list ---//
				
				roomObjectList[i].transform.GetChild(0).gameObject.SetActive(false);
				roomObjectList[i].transform.GetChild(1).gameObject.SetActive(true);
				roomObjectList[i].transform.GetChild(2).gameObject.SetActive(true);
				roomObjectList[i].transform.GetChild(2).GetComponent<Button>().onClick.AddListener( delegate { JoinRoom(i); } );

				SetRoomInfo( lobbySession.GetRoomList() );
			}
		}
		//--- CASE : Room list is empty ---//
		else
		{
			for( int i=0; i<listPanel.childCount; i++ )
			{
				roomObjectList[i].transform.GetChild(0).gameObject.SetActive(true);
				roomObjectList[i].transform.GetChild(1).gameObject.SetActive(false);
				roomObjectList[i].transform.GetChild(2).gameObject.SetActive(false); 
				
			}
		}
	}

	private void MakeRoom()
	{
		SceneManager.LoadScene("MakeRoom" , LoadSceneMode.Single);
	}

	public void LoadRoom( Room roomInfo )
	{
		SceneManager.LoadScene("InGame" , LoadSceneMode.Single);
	}

	public void JoinRoom( int roomNo )
	{
		Room roomInfo = GetRoomInfo( roomNo );
		GameSession gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession( this );

		gameSession.SetRoomInfo( roomInfo );

		SceneManager.LoadScene("LoadingInGame" , LoadSceneMode.Single);
	}

	public void SetRoomInfo( List<Room> roomList )
	{
		for( int i=0; i<roomList.Count; i++ )
		{
			string title = roomList[i].m_title;
			string capacity = "" + roomList[i].m_capacity;
			string members = "" + roomList[i].m_presentMembers;

			string title_sub = " ( " + members + " / " + capacity + " ) ";

			roomObjectList[i].transform.GetChild(1).GetComponent<Text>().text = title.Insert( title.Length , title_sub );
		}
	}

	public Room GetRoomInfo( int index )
	{
		string title = roomObjectList[index].transform.GetChild(1).GetComponent<Text>().text;
		Room roomInfo = new Room();
		
		title = title.Trim();

		int pivot = title.IndexOf("(");
		int pivot2 = title.IndexOf("/");

		roomInfo.m_title = title.Substring( 0 , pivot );
		roomInfo.m_presentMembers = System.UInt32.Parse( title.Substring( pivot+1 , pivot2 ) );
		roomInfo.m_capacity = System.UInt32.Parse( title.Substring( pivot2 , title.Length-1 ) );

		return roomInfo;
	}

	private void OnDestroy()
	{
		//lobbySession.CloseSession();
	}

}
