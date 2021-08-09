using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DataInfo;
public class GameManager : MonoBehaviour
{
    //랜덤하게 생성할 적군 캐릭터, 에너지
    public GameObject Enemy;
    public GameObject Energy;
    private int Enemynum = 0;
    private int Energynum = 0;
    public GameObject EndPanel;
    public GameObject PauseText;
    private bool isPaused; // 일시정지 여부를 판단하는 변수


    // Inventory 오브젝트의 CanvasGroup 컴포넌트에 대한 리퍼런스 변수
    public CanvasGroup inventoryCG;
    //싱글턴에 접근하기 위한 Static 변수 선언
    public static GameManager instance = null;

    [Header("GameData")]
    public Text livingTime; // 생존시간 표시할 텍스트
    private DataManager dataManager; // DataManager 저장할 변수
    public GameData gameData;

    // 인벤토리의 아이템이 변경에 대한 처리를 담당하는 이벤트 관련 설정
    public delegate void ItemChageDelegate();
    public static event ItemChageDelegate OnItemChange;

    // SlotList 게임 오브젝트를 저장할  변수
    private GameObject slotList;
    // 아이템 리스트 저장할 변수
    public GameObject[] itemObjects;



    void Start()
    {
        // 첫 시작시 인벤토리 비활성화
        OnInventoryOpen(false);
        while (Enemynum < Random.Range(3, 7))
        {
            
           Instantiate(Enemy, new UnityEngine.Vector3(Random.Range(-25.0f, 25.0f), 0f, Random.Range(-25.0f, 25.0f)), UnityEngine.Quaternion.identity);
            Enemynum++;
        }
        //에너지 랜덤 생성
        while (Energynum < Random.Range(1, 5))
        {
            Instantiate(Energy, new UnityEngine.Vector3(Random.Range(-25.0f, 25.0f), 1f, Random.Range(-25.0f, 25.0f)), UnityEngine.Quaternion.identity);
            Energynum++;
        }
    }

    /*void RestartGame()
    {
        //5.3이상의 유니티 버전에서는 아래 코드 사용
        //SceneManager.GetActiveScene();
        Application.LoadLevel(Application.loadedLevel);
        
    }*/

    public void OnInventoryOpen(bool isOpened)
    {
        // 오픈 = 투명도 100%, 클로즈드 = 투명도 0%
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened; // 해당 게임오브젝트가 이벤트에 반응할 것인가 여부
        inventoryCG.blocksRaycasts = isOpened; // 이벤트 신호(메시지)를 받을지 여부
    }



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //instance에 할당된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미함
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        //다른 씬으로 넘어가더라도 삭제하지 않고 유지함
        DontDestroyOnLoad(this.gameObject);

        //DataManger를 이용하기 위해 준비
        dataManager = GetComponent<DataManager>();
        dataManager.Initialize();//초기화


        // 슬롯리스트 획득
        slotList = inventoryCG.transform.Find("SlotList").gameObject;



        LoadGameData();


    }


void Update()
    {
        //게임이 끝났을때 R버튼을 누르면 재시작
        /*if(EndPanel.activeSelf==true && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();

        }*/
    }

    // 일시 정지 버튼 클릭 시 호출할 이벤트 핸들러 메소드
    public void OnPauseClick()
    {

        isPaused = !isPaused; // 일시정지 값을 토글

        if (isPaused)
        {
            Debug.Log("STOP!");
        }

        // 3항 연산을 이용하여, Time.timeScale 값을 0 or 1로 설정
        Time.timeScale = (isPaused) ? 0.0f : 1.0f;


        PauseText.SetActive(isPaused);
        //플레이어 오브젝트의 이벤트 발생도 막아야 한다.
        var playerObj = GameObject.FindGameObjectWithTag("Player"); // 플레이어 오브젝트 획득

        // 플레이어 오브젝트에 포함된 모든 스크립트를 추출해서 획득
        // 설명 : 플레이어 오브젝트에 대해, 해당 오브젝트가 가지고 있는 MonoBehaviour 타입의 컴포넌트를 전부 획득
        var scripts = playerObj.GetComponents<MonoBehaviour>(); 

        //획득한 스크립트 오브젝트 전체에 대해, isPause 값에 따라 활성화/비활성화를 행한다
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }

        // 인벤토리 선택 버튼에 대해, 정지 상태일 때는 이벤트를 받아들이지 않도록 
        // CanvasGroup의 Blocks Raycasts 항목을 조정한다
        var InventoryButtoncanvasGroup = GameObject.Find("InventoryButton").GetComponent<CanvasGroup>();
        var InventroycanvasGroup = GameObject.Find("Inventory").GetComponent<CanvasGroup>();
        InventoryButtoncanvasGroup.blocksRaycasts = !isPaused;
        InventroycanvasGroup.blocksRaycasts = !isPaused;
    }




    public void LoadGameData()
    {

        GameData data = dataManager.Load(); 
         //불러온 데이터를 세팅해주는 작업
        gameData.armor = data.armor;
        gameData.bestLivingTime = data.bestLivingTime;
        gameData.drill = data.drill;
        gameData.characterSpeed = data.characterSpeed;
        gameData.equipItem = data.equipItem;
        gameData.monsterspeed = data.monsterspeed;

        livingTime.text = "당신의 최고기록은 : " + gameData.bestLivingTime.ToString("00.00") + "초 입니다!";
        // 보유한 아이템이 존재할 경우  실행
        if (gameData.equipItem.Count > 0)
        {
            InventorySetup();
        }

    }

    void InventorySetup()
    {
        // SlotList 하위에 있는 모든 Slot을 추출
        var slots = slotList.GetComponentsInChildren<Transform>();

        for (int idx = 0; idx < gameData.equipItem.Count; idx++) // n번 아이템에 대해 수행. 보유하고 있는 아이템만큼반복
        {
            // 인벤토리 UI에 있는 슬롯 개수만큼 반복. 0 은 자기 자신이니 1번 부터
            for (int j = 1; j < slots.Length; j++) // 슬롯이 비어있는지 순서대로 확인
            {
                if (slots[j].childCount > 0) { continue; }//이 슬롯에 뭔가 아이템이 이미 적재되어 있다면 패스

                // 보유 아이템의 종류에 따라 인덱스 확보
                int itemIndex = (int)gameData.equipItem[idx].itemType; // enum타입을 int로 변경

                // 아이템의 부모를 Slot 게임오브젝트로 변경
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j]);
                // 아이템이 가지는 ItemInfo 정보를 적용시킴
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[idx];

                break;
            }
        }
    }

    // 게임 데이터 저장
    public void SaveGameData()
    {
        // 현재 활성화 되어 있는 GameData 인스턴스의 내용을 파일에 저장한다.
        dataManager.Save(gameData);

    }

    // 현재 작동 중인 앱이 종료될 때 실행
    public void OnApplicationQuit()
    {
        SaveGameData(); // 종료 시, 파일에 현재의 GameData를 저장
    }

    //게임이 끝나고 최고기록 비교 기록
    public void CheckBestLivingTime(float time)
    {
        if (gameData.bestLivingTime < time)
        {
            gameData.bestLivingTime = time;
            livingTime.text = "당신의 최고기록은 : "+gameData.bestLivingTime.ToString("00.00")+"초 입니다!";
        }
    }

    // 아이템을 인벤토리에 추가했을 때의 처리
    public void AddItem(Item item)
    {
        // 기존 보유 아이템과 종류가 중복되면 추가하지 않음
        if (gameData.equipItem.Contains(item)) { return; }
        // 신규 요소면 equipItem에 추가
        gameData.equipItem.Add(item);


        // 추가한 아이템 처리
        // 아이템 데이터 요소 : 타입, 계산 방식
        // 아이템 타입 개수가 적으므로, 교재에서는 switch 방식 사용
        switch (item.itemType)
        {
            case Item.ItemType.ARMOR:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.armor += item.value;
                }
                else
                {
                    gameData.armor += gameData.armor * item.value;
                }
                break;
            case Item.ItemType.DRILL:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.drill += item.value;
                }
                else
                {
                    gameData.drill += gameData.drill * item.value;
                }
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.characterSpeed += item.value;
                }
                else
                {
                    gameData.characterSpeed += gameData.characterSpeed * item.value;
                }
                break;
            case Item.ItemType.SLOW:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.monsterspeed -= item.value;
                }
                else
                {
                    gameData.monsterspeed -= gameData.monsterspeed / (1.0f + item.value);
                }
                
                break;
        }




        // 아이템 변경을 실시간으로 적용시키기 위해, 정의해 둔 이벤트를 발생(실행)시킨다.
        OnItemChange();


    }


    // 아이템을 인벤토리에서 제거했을 때의 처리
    public void RemoveItem(Item item)
    {
        // 해당 아이템을 gameData의 아이템리스트에서 삭제
        gameData.equipItem.Remove(item); // 인자값으로 지정한 인스턴스에 대한 리퍼런스 연결을 제거. 리스트에서만 제거하는거지 인스턴스는 살아있음

        // 종류에 따라 처리
        switch (item.itemType)
        {
            case Item.ItemType.ARMOR:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.armor -= item.value;
                }
                else
                {
                    gameData.armor= gameData.armor / (1.0f + item.value);
                }
                break;
            case Item.ItemType.DRILL:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.drill -= item.value;
                }
                else
                {
                    gameData.drill = gameData.drill / (1.0f + item.value);
                }
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.characterSpeed -= item.value;
                }
                else
                {
                    gameData.characterSpeed = gameData.characterSpeed / (1.0f + item.value);
                }
                break;
            case Item.ItemType.SLOW:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.monsterspeed += item.value;
                }
                else
                {
                    gameData.monsterspeed += gameData.monsterspeed * item.value;
                }
                break;
        }



        OnItemChange();

    }

}
