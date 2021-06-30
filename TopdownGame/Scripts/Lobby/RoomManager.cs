
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
				int roomNo = i;
				//--- Set component with room list ---//
				roomObjectList[i].transform.GetChild(0).gameObject.SetActive(false);
				roomObjectList[i].transform.GetChild(1).gameObject.SetActive(true);
				roomObjectList[i].transform.GetChild(2).gameObject.SetActive(true);
				roomObjectList[i].transform.GetChild(2).GetComponent<Button>().onClick.AddListener( delegate { JoinRoom(roomNo); } );

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
		SceneManager.LoadScene("MakeRoom" , LoadSceneMode.Additive);
	}

	public void JoinRoom( int roomNo )
	{
		Room roomInfo = GetRoomInfo( roomNo );
		GameSession gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
		gameSession.SetRoomInfo( roomInfo );

		lobbySession.CloseSession();
		SceneManager.LoadScene("LoadingInGame" , LoadSceneMode.Single);
	}

	public void SetRoomInfo( List<Room> roomList )
	{
		for( int i=0; i<roomList.Count; i++ )
		{
			string roomNumber = "[" + roomList[i].m_roomId + "] ";
			string title = roomNumber + roomList[i].m_title;
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

		int delimIndex = title.IndexOf("]");
		string roomNumber = title.Substring( 0 , delimIndex+1 );
		title = title.Substring( delimIndex+1 );

		delimIndex = title.LastIndexOf("(");
		string roomTitle = title.Substring( 0 , delimIndex );
		title = title.Substring( delimIndex );

		delimIndex = title.LastIndexOf(")");
		string roomState = title.Substring( 0 , delimIndex+1 ).Trim( '(' , ')' );
		roomState = roomState.Trim();

		delimIndex = roomState.IndexOf("/");

		roomInfo.m_roomId = roomNumber.Trim( '[' , ' ' , ']' );
		roomInfo.m_title = roomTitle.Trim();
		roomInfo.m_presentMembers = System.UInt32.Parse( roomState.Substring( 0 , delimIndex ) );
		roomInfo.m_capacity = System.UInt32.Parse( roomState.Substring( delimIndex+1 ) );

		return roomInfo;
	}

	private void OnDestroy()
	{
		//lobbySession.CloseSession();
	}

}
