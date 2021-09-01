
using UnityEngine;


public class OtherPlayer : MonoBehaviour
{
	PlayerObject player;
	ChatBubble bubble;
	GameSession gameSession;
	Rigidbody2D rigid;
	Animator animator;
	public int direction;

	private void Awake()
	{
		gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		
	}
	private void Start()
	{
		player = (PlayerObject) gameSession.gameObjectManager.GetGameObject( System.Convert.ToUInt32(name) );
		GameObject obj = (GameObject) Instantiate( Resources.Load("Prefabs/Bubble") , GameObject.Find("ChatBubble").transform );
		obj.SetActive(true);
		obj.name = name + "_bubble";
		bubble = obj.transform.GetComponent<ChatBubble>();
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        rigid.velocity = player.velocity;
		rigid.position = player.position;
		bubble.position.x = rigid.position.x;
		bubble.position.y = rigid.position.y + 2.4f;
		SetDirection( player.direction );
		
    }

	void SetDirection( int newDirection )
	{
		if( newDirection != direction )
		{
			direction = newDirection;
			animator.SetInteger( "moveDirection" , direction );
		}
	}

	public void DisplayMSG( string contents )
    {
        if( IsInvoking("ClearBubble") )
            CancelInvoke();

		bubble.gameObject.SetActive(true);
        bubble.SetContents( contents );
        
        Invoke("ClearBubble" , 3);
        
    }
	void ClearBubble()
    {
        bubble.gameObject.SetActive(false);
    }

	private void OnDestroy()
	{
		ClearBubble();
	}
}
