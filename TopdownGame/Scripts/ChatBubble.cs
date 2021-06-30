using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBubble : MonoBehaviour
{
	TextMeshProUGUI contents;
    public Vector2 position;
	Image bubble;

	private void Awake()
	{
        position = Vector2.zero;
	}

	private void Start()
	{
		contents = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		bubble = GetComponent<Image>();
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        gameObject.transform.position = position;
		LOG.printLog("DEBUG","POSITION","("+transform.position.x+","+transform.position.y+")");
    }

	public void SetContents( string newContents )
	{
		contents.text = newContents;
	}
}
