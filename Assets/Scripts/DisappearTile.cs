using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearTile : MonoBehaviour
{
	[SerializeField] float fallTime = 0.5f, returnTime = 0.7f;
	Rigidbody2D rigid;
	Vector2 startPos;
	bool isBack;

	void Start()
	{
		rigid = GetComponent<Rigidbody2D>();
		startPos = transform.position;
	}

	void Update()
	{
		if(isBack)
		{
			transform.position = Vector2.MoveTowards(transform.position, startPos, 20 * Time.deltaTime);
		}

		if(transform.position.y == startPos.y)
		{
			isBack = false;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag.Equals("Player") && !isBack)
		{
			Invoke("FallPlatform", fallTime);
		}
	}

	void FallPlatform()
	{
		rigid.isKinematic = false;
		Invoke("BackPlatform", returnTime);
	}

	void BackPlatform()
	{
		rigid.velocity = Vector2.zero;
		rigid.isKinematic = true;
		isBack = true;
	}
}
