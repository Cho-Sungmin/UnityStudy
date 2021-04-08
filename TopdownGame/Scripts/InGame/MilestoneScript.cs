using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MilestoneScript : ObjectInfo
{
	private void Awake()
	{
		id = 1000;
		isNPC = false;
		
		string[] lines = new string[] { "호수로 가는 길" , "<주의>\n수심 1.5m" };

		lineInfo = new LineInfo[2];

		lineInfo[0].line = lines[0];
		lineInfo[0].portrait = null;

		lineInfo[1].line = lines[1];
		lineInfo[1].portrait = null;
	}
	
}
