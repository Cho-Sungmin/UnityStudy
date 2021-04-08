using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyScript : ObjectInfo
{

	private void Awake()
	{
		id = 1001;
		isNPC = true;

		Sprite[] portraits = Resources.LoadAll<Sprite>("UI_NPC");
		
		string[] lines = new string[] { "이 호수 정말 아름답지 않아?" , "보기와는 다르게 사연이 많은 곳이야..." };

		lineInfo = new LineInfo[2];

		lineInfo[0].line = lines[0];
		lineInfo[0].portrait = portraits[7];

		lineInfo[1].line = lines[1];
		lineInfo[1].portrait = portraits[4];
	}

}
