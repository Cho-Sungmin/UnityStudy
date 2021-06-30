using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TalkManagerScript : MonoBehaviour
{
   Dictionary<int, LineInfo[]> NPCLineTable;
   

	private void Awake()
	{
		NPCLineTable = new Dictionary<int, LineInfo[]>();
		
	}
	private void Start()
	{
		InitNPCs();
	}
	void InitNPCs()
	{

		GameObject npc;
		GameObject npcGroup = GameObject.Find("NPC"); 
		int n = npcGroup.transform.childCount;

		for(int i = 0; i<n; i++ )
		{
			npc = npcGroup.transform.GetChild(i).gameObject;
			int id = npc.GetComponent<ObjectInfo>().id;
			ObjectInfo test = npc.GetComponent<ObjectInfo>();
			ref LineInfo[] lineInfo = ref npc.GetComponent<ObjectInfo>().GetLineInfo();
			
			NPCLineTable.Add( id , lineInfo);
			
		}
	}

	public LineInfo[] GetLines( int key )
	{
		return NPCLineTable[key];
	}

	public ref LineInfo GetLineInfo( int key , int index )
	{
		if( NPCLineTable[key].Length > index )
			return ref NPCLineTable[key][index];
		else
			throw new System.IndexOutOfRangeException();
	}

}
