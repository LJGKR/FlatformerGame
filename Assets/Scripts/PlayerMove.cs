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

	void Update() //단발적인 키 입력은 업데이트가 좋다(ex - 점프)
	{
		//플레이어 점프
		if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
		{
			rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			PlaySound("JUMP");
		}

		//키를 놓을 시 캐릭의 노말벡터에 0.5를 곱해 가속력을 확 낮춤
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
		}

		//캐릭터 방향전환
		if (Input.GetButton("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		//캐릭 움직임에 따라 애니메이션 변경
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
		//플레이어 이동 (AddForce 사용)
		float h = Input.GetAxisRaw("Horizontal");

		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		if(rigid.velocity.x > maxSpeed) //오른쪽 최대속력 제한
		{
			rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
		}
		else if(rigid.velocity.x < maxSpeed * (-1)) { //왼쪽 최대속력 제한
			rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
		}

		//플레이어 착륙 감지
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
		//점수획득
		gameManager.stagePoint += 200;
		//적 처치 효과
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
		//적 처치
		EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
		enemyMove.OnDamaged();
	}

	void OnDamaged(Vector2 targetPos)
	{
		//체력 하락
		gameManager.HealthDown();

		gameObject.layer = 9; //데미지를 입지 않는 레이어로 변경

		//캐릭을 살짝 투명하게
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		//살짝 밀려남
		int direct = transform.position.x - targetPos.x > 0 ? 1 : -1;
		rigid.AddForce(new Vector2(direct, 1) * 7, ForceMode2D.Impulse);

		//애니메이션
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
			//점수 획득
			gameManager.stagePoint += 300;
			//동전 삭제
			collision.gameObject.SetActive(false);

			PlaySound("ITEM");
		}
		else if(collision.gameObject.tag == "Finish")
		{
			//스테이지 변경
			gameManager.NextStage();

			PlaySound("FINISH");
		}
	}

	public void OnDie()
	{
		//캐릭터를 흐리게
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		//방향 뒤집기
		spriteRenderer.flipX = true;
		//콜라이더 비활성화
		capsuleCollider.enabled = false;
		//살짝 튀어오름
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
