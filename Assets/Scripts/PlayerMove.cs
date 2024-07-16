using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public GameManager gameManager;
	public AudioClip audioJump;
	public AudioClip audioAttack;
	public AudioClip audioDamaged;
	public AudioClip audioItem;
	public AudioClip audioDie;
	public AudioClip audioFinish;

	public float maxSpeed;
	public float jumpPower;

	Rigidbody2D rigid;
	SpriteRenderer spriteRenderer;
	Animator anim;
	CapsuleCollider2D capsuleCollider;
	AudioSource audioSource;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update() //�ܹ����� Ű �Է��� ������Ʈ�� ����(ex - ����)
	{
		//�÷��̾� ����
		if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
		{
			rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			PlaySound("JUMP");
		}

		//Ű�� ���� �� ĳ���� �븻���Ϳ� 0.5�� ���� ���ӷ��� Ȯ ����
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
		}

		//ĳ���� ������ȯ
		if (Input.GetButton("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		//ĳ�� �����ӿ� ���� �ִϸ��̼� ����
		if(Mathf.Abs(rigid.velocity.x) < 0.4)
		{
			anim.SetBool("isWalking", false);
		}
		else
		{
			anim.SetBool("isWalking", true);
		}
	}

	void FixedUpdate()
	{
		//�÷��̾� �̵� (AddForce ���)
		float h = Input.GetAxisRaw("Horizontal");

		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		if(rigid.velocity.x > maxSpeed) //������ �ִ�ӷ� ����
		{
			rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
		}
		else if(rigid.velocity.x < maxSpeed * (-1)) { //���� �ִ�ӷ� ����
			rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
		}

		//�÷��̾� ���� ����
		if (rigid.velocity.y < 0)
		{
			Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

			RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

			if (rayHit.collider != null)
			{
				if (rayHit.distance < 0.5f)
				{
					anim.SetBool("isJumping", false);
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
			{
				OnAttack(collision.transform);
				PlaySound("ATTACK");
			}
			else
			{
				OnDamaged(collision.transform.position);
				PlaySound("DAMAGED");
			}
		}
	}

	void OnAttack(Transform enemy)
	{
		//����ȹ��
		gameManager.stagePoint += 200;
		//�� óġ ȿ��
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
		//�� óġ
		EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
		enemyMove.OnDamaged();
	}

	void OnDamaged(Vector2 targetPos)
	{
		//ü�� �϶�
		gameManager.HealthDown();

		gameObject.layer = 9; //�������� ���� �ʴ� ���̾�� ����

		//ĳ���� ��¦ �����ϰ�
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		//��¦ �з���
		int direct = transform.position.x - targetPos.x > 0 ? 1 : -1;
		rigid.AddForce(new Vector2(direct, 1) * 7, ForceMode2D.Impulse);

		//�ִϸ��̼�
		anim.SetTrigger("doDamaged");
		Invoke("offDamaged", 2);
	}

	void offDamaged()
	{
		gameObject.layer = 8;

		spriteRenderer.color = new Color(1, 1, 1, 1);
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Item")
		{
			//���� ȹ��
			gameManager.stagePoint += 300;
			//���� ����
			collision.gameObject.SetActive(false);

			PlaySound("ITEM");
		}
		else if(collision.gameObject.tag == "Finish")
		{
			//�������� ����
			gameManager.NextStage();

			PlaySound("FINISH");
		}
	}

	public void OnDie()
	{
		//ĳ���͸� �帮��
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		//���� ������
		spriteRenderer.flipX = true;
		//�ݶ��̴� ��Ȱ��ȭ
		capsuleCollider.enabled = false;
		//��¦ Ƣ�����
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

		PlaySound("DIE");
	}

	public void VelocityZero()
	{
		rigid.velocity = Vector2.zero;
	}

	void PlaySound(string action)
	{
		switch (action)
		{
			case "JUMP":
				audioSource.clip = audioJump;
				break;
			case "ATTACK":
				audioSource.clip = audioAttack;
				break;
			case "DAMAGED":
				audioSource.clip = audioDamaged;
				break;
			case "ITEM":
				audioSource.clip = audioItem;
				break;
			case "DIE":
				audioSource.clip = audioDie;
				break;
			case "FINISH":
				audioSource.clip = audioFinish;
				break;

		}
		audioSource.Play();
	}
}
