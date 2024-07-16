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
		//���� �⺻ ������
		rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

		//������������ üũ
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.4f, rigid.position.y); //��� �������� üũ�ϴ� ����
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		if (rayHit.collider == null)
		{
			Turn();
		}
	}

	//����Լ�
	void Think()
	{
		//������ ���� Ȱ���� �����ϴ� �Լ�
		nextMove = Random.Range(-1, 2);

		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);

		//���� �ִϸ��̼� ����
		anim.SetInteger("walkSpeed", nextMove);

		//���� ���� ����
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
		//���͸� �帮��
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		//���� ������
		spriteRenderer.flipX = true;

		nextMove = 0;
		//�ݶ��̴� ��Ȱ��ȭ
		boxCollider.enabled = false;
		//���� �� ��¦ Ƣ�����
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
		//���� �ı�
		Invoke("DeActive", 5);

	}

	void DeActive()
	{
		gameObject.SetActive(false);
	}
}
