using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
	GameObject player;
	Vector3 zVec = new Vector3(0,0,-1);

	private void Awake()
	{
		player = GameObject.Find("Player");
	}

	private void FixedUpdate()
	{
		transform.position = player.transform.position + zVec;
	}
}
