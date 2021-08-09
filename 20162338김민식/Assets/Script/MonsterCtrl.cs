using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;

    void Start()
    {
        
        //몬스터의 Transform 할당
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적대상인 Player의 Transform 할당
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgent 컴포넌트 할당
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        nvAgent.speed += GameManager.instance.gameData.monsterspeed;

    }

    // Update is called once per frame
    void Update()
    {
        nvAgent.SetDestination(playerTr.position);
        
    }
    
    public void EnemySpeedUP()
    {
        nvAgent.speed += 5f;
        nvAgent.acceleration += Random.Range(1f,5f);
        Debug.Log("박스와충돌!");
    }

    private void OnEnable()
    {

        GameManager.OnItemChange += UpdateSetup;

    }


    void UpdateSetup()
    {
            nvAgent.speed += GameManager.instance.gameData.monsterspeed; // 오브젝트 풀 구현?
    }
}
