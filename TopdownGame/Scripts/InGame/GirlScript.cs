using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GirlScript : ObjectInfo
{

	private void Awake()
	{ 
	
		id = 100;
		isNPC = true;
		Sprite[] portraits = Resources.LoadAll<Sprite>("UI_NPC");
		
		string[] lines = new string[] { "안녕?" , "이곳은 처음이지?" };

		lineInfo = new LineInfo[2];

		lineInfo[0].line = lines[0];
		lineInfo[0].portrait = portraits[2];

		lineInfo[1].line = lines[1];
		lineInfo[1].portrait = portraits[3];


		//Debug.Log(lineInfo[0].line);
	}

}
