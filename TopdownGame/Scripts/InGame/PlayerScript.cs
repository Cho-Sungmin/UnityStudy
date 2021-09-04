
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	GameSession gameSession;
	GameObject gameManager;
	Rigidbody2D rigid;
	Animator animator;
	PlayerObject player;

	int direction = 0;
	Vector3 dirVec = Vector3.zero;

	private void Awake()
	{
		gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
		gameManager = GameObject.Find("GameManager");
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		player = (PlayerObject) gameSession.gameObjectManager.GetGameObject( System.Convert.ToUInt32(name) );
		name = "Player";

		rigid.velocity = player.velocity;

		SetDirection( 0 );
	}

	private void Update()
	{
		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			GameObject npc = DetectObject( dirVec );
			if( npc != null )
				gameManager.SendMessage( "Interaction" , npc );

		}
	}
	private void FixedUpdate()
	{
		Move();
		dirVec = DrawActionRay();
	}

	void Move()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");

		int newDirection = 0;

		if( x == 0 && y == 0 )
		{
			newDirection = direction - direction%10;
	
			SetDirection( newDirection );

			player.velocity = Vector2.zero;
		}
		else if( x == 0 || y == 0 )
		{
			//--- Set Direction parameter ---//
			if( x == 0 )
			{
				if( y > 0 )
				{
					newDirection = 21;
				}
				else
				{
					newDirection = -21;
				}
			}
			else
			{
				if( x > 0 )
				{
					newDirection = 11;
				}
				else
				{
					newDirection = -11;
				}
			}			
				
			SetDirection( newDirection );
			player.velocity = new Vector2( 3*x , 3*y );
		}

		rigid.velocity = player.velocity;
		player.position = rigid.position;
		player.direction = direction;

		OutputByteStream objectStream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		objectStream.Write( (byte) ReplicationAction.UPDATE );
		objectStream.Write( gameSession.gameObjectManager.GetObjectId( player ) );

		gameSession.NotificateReplication( new InputByteStream( objectStream ) );
	}

	void SetDirection( int newDirection )
	{
		if( newDirection != direction )
		{
			direction = newDirection;
			animator.SetInteger( "moveDirection" , direction );
		}
	}

	Vector3 DrawActionRay()
	{
		Vector3 directionVec;
		int dir = direction/10;

		if( dir == 1 | dir == -1 )
			directionVec = new Vector3( dir , 0 );
		else
			directionVec = new Vector3( 0 , dir/2 );
		
		UnityEngine.Debug.DrawRay( rigid.position , directionVec , Color.red );

		return directionVec;
	}

	GameObject DetectObject( Vector3 dirVec )
	{
		RaycastHit2D rayCast = Physics2D.Raycast( rigid.position , dirVec , 1 , LayerMask.GetMask("NPC"));

		if( rayCast.collider != null )
			return rayCast.collider.gameObject;
		else
			return null;
	}

	private void OnDestroy()
	{
		LOG.printLog( "DEBUG" , "MEMORY" , "OnDestroy() : PlayerScript" );

		OutputByteStream objectStream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		objectStream.Write( (byte) ReplicationAction.DESTROY );
		objectStream.Write( gameSession.gameObjectManager.GetObjectId( player ) );

		gameSession.NotificateReplication( new InputByteStream( objectStream ) );
	}


}
