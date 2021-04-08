using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	GameObject gameManager;
	Rigidbody2D rigid;
	Animator animator;

	int direction = 0;
	Vector3 dirVec = Vector3.zero;

	private void Awake()
	{
		gameManager = GameObject.Find("GameManager");
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

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

			rigid.velocity = Vector2.zero;
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

			//transform.Translate( new Vector2( x, y ) );
			rigid.velocity = new Vector2( 3*x , 3*y );

		}
		
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
		
		Debug.DrawRay( rigid.position , directionVec , Color.red );

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
}
