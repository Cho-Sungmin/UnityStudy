using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
	GameObject player;
	Vector3 zVec = new Vector3(0,0,-2);

	public void SetPlayerComponent()
	{
		player = GameObject.Find("Player");
	}

	private void Awake()
	{
		SetPlayerComponent();
	}

	private void FixedUpdate()
	{
		if( player != null )
			transform.position = player.transform.position + zVec;
	}
}
