using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
	public int nextMove;
	Animator anim;
	SpriteRenderer spriteRenderer;
	BoxCollider2D boxCollider;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();

		Think();

		Invoke("Think", 5);
	}

	void FixedUpdate()
	{
		//몬스터 기본 움직임
		rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

		//낭떠러지인지 체크
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.4f, rigid.position.y); //어느 방향인지 체크하는 벡터
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		if (rayHit.collider == null)
		{
			Turn();
		}
	}

	//재귀함수
	void Think()
	{
		//몬스터의 다음 활동을 결정하는 함수
		nextMove = Random.Range(-1, 2);

		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);

		//몬스터 애니메이션 설정
		anim.SetInteger("walkSpeed", nextMove);

		//몬스터 방향 설정
		if(nextMove != 0)
		{
			spriteRenderer.flipX = nextMove == 1;
		}
	}

	void Turn()
	{
		nextMove *= -1;
		spriteRenderer.flipX = nextMove == 1;

		CancelInvoke();
		Invoke("Think", 5);
	}

	public void OnDamaged()
	{
		//몬스터를 흐리게
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		//방향 뒤집기
		spriteRenderer.flipX = true;

		nextMove = 0;
		//콜라이더 비활성화
		boxCollider.enabled = false;
		//죽을 때 살짝 튀어오름
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
		//몬스터 파괴
		Invoke("DeActive", 5);

	}

	void DeActive()
	{
		gameObject.SetActive(false);
	}
}
