using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
	PlayerObject player;
	GameSession gameSession;
	Rigidbody2D rigid;

	private void Awake()
	{
		gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
		rigid = GetComponent<Rigidbody2D>();
		
	}
	private void Start()
	{
		player = (PlayerObject) gameSession.gameObjectManager.GetGameObject( System.Convert.ToUInt32(name) );
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        rigid.velocity = player.velocity;
		rigid.position = player.position;
    }
}
