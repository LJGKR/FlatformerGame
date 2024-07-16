using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;

    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject restartButton;


	void Update()
	{
		UIPoint.text = (totalPoint + stagePoint).ToString();
		UIStage.text = "STAGE " + (stageIndex + 1).ToString();
	}
	public void NextStage()
    {
        //스테이지 관리
        if(stageIndex < Stages.Length-1) {
			Stages[stageIndex].SetActive(false);
			stageIndex++;
			Stages[stageIndex].SetActive(true);
			PlayerReposition();
		}
        else
        {
            //게임 클리어
            Time.timeScale = 0;
			Debug.Log("게임 클리어!!!!");
			restartButton.SetActive(true);
            Text btnText = restartButton.GetComponentInChildren<Text>();
            btnText.text = "Clear!!";
            restartButton.SetActive(true);
		}
		

        //점수 계산
		totalPoint += stagePoint;
        stagePoint = 0;
    }
    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
			UIhealth[0].color = new Color(1, 0, 0, 0.4f);

			player.OnDie();

            Debug.Log("죽었습니다..");

            restartButton.SetActive(true);
		}
    }
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
        {
            if (health > 1)
            {
                //플레이어 위치 초기화
                PlayerReposition();

			}

			HealthDown();

		}
	}

    void PlayerReposition()
    {
        player.VelocityZero();
		player.transform.position = new Vector3(-6.88f, -0.5f, 0);
	}

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
