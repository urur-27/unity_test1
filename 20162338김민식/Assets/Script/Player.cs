using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    //컴포넌트 할당
    private Transform tr;
    //이동속도
    public float moveSpeed = 10.0f;
    //회전속도
    public float rotSpeed = 100.0f;


    //충돌시 재 생산할 적군 캐릭터
    public GameObject Enemy;
    //종료 UI
    public GameObject EndPanel;

    //HP 표기해주기 위한 텍스트
    public Text hpText;
    //speed 업 상태 표시를 위한 텍스트
    public Text SpeedUP;
    //시간출력 UI
    public Text timeText;
    //종료시간출력 UI
    public Text EndTime;
    //HP 이미지 Image UI
    public Image HPImg;
    //시간
    public float time;
    //방어도(피해감소량)
    private float armor=0.0f;
    // 드릴 (벽 제거, 사용시 감소)
    private float drill = 0.0f;
    

    //체력
    private float hp = 100;
    const float fullHp = 100;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        EndPanel.SetActive(false);
        armor = GameManager.instance.gameData.armor;
        moveSpeed = GameManager.instance.gameData.characterSpeed;
        drill = GameManager.instance.gameData.drill;
    }

    // Update is called once per frame
    void Update()
    {
        //UI에 플레이 시간 표시
        if (hp > 0)
        {
            time += Time.deltaTime;
            timeText.text = string.Format("{0:N2}", time);
        }

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        //전후좌우 이동 방향 벡터 계산 & 이동
        UnityEngine.Vector3 moveDir = (UnityEngine.Vector3.forward * v) + (UnityEngine.Vector3.right * h);

        tr.Translate(moveDir*Time.deltaTime*moveSpeed, Space.Self);

        tr.Rotate(UnityEngine.Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));
    }

    void OnCollisionEnter(Collision coll)
    {
        //적과 충돌시 체력 -10, 충돌한 적은 랜덤 위치에 재 생산
        if(coll.gameObject.tag=="Enemy")
        {
            hp -= (10-armor);
            Debug.Log("HP=" + hp.ToString());
            Destroy(coll.gameObject);
            //coll.gameObject.SetActive(false);
            Instantiate(Enemy, new UnityEngine.Vector3(Random.Range(-25.0f, 25.0f), 0f, Random.Range(-25.0f, 25.0f)), UnityEngine.Quaternion.identity);
            if (hp<=0)
            {
                PlayerDie();
            }
        }
        // 에너지와 충돌시 체력이 100이면 에너지는 삭제되고 3초간 스피드업, 체력이 100이 아니라면 체력+10
        else if (coll.gameObject.tag == "Energy")
        {
            Destroy(coll.gameObject);
            if (hp == 100)
            {
                StartCoroutine("Speedup");
                return;
            }
            hp += 10;
            Debug.Log("HP=" + hp.ToString());
            
        }
        //장애물들에 충돌시 적들이 이동속도가 1~5사이에서 랜덤하게 증가
        else if(coll.gameObject.tag =="Block")
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SendMessage("EnemySpeedUP", SendMessageOptions.DontRequireReceiver);
            }
            if (drill > 0)
            {
                Destroy(coll.gameObject);
            }

        }

        //생명 게이지의 색상 및 크기 변경 함수를 호출
        DisplayHpbar();
    }

    IEnumerator Speedup()
    {
        moveSpeed += 10.0f;
        SpeedUP.text = string.Format("<color=#0067a3>Speed UP!</color>");
        yield return new WaitForSeconds(3f);
        moveSpeed -= 10.0f;
        SpeedUP.text = string.Format("");
    }
    void PlayerDie()
    {
        Debug.Log("Player Die");
        EndPanel.SetActive(true);
        EndTime.text= string.Format("생존 시간 : {0:N2} 초!", time);
        GameManager.instance.CheckBestLivingTime(time);

    }

    void DisplayHpbar()
    {

        //HpBar의 크기 변경
        HPImg.fillAmount = (hp / fullHp);
        //HpBar에 현재 체력 표기
        hpText.text = string.Format("HP <color=#ff0000>{0}/{1}</color>", hp,fullHp);
    }


    private void OnEnable()
    {

        GameManager.OnItemChange += UpdateSetup;
        
    }


    void UpdateSetup()
    {
        armor = GameManager.instance.gameData.armor;
        moveSpeed = GameManager.instance.gameData.characterSpeed;
        drill = GameManager.instance.gameData.drill;
    }
}
