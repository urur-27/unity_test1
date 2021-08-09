using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    // 드래그하면 위치가 변경되므로, 이 스크립트가 적용되는 오브젝트의 위치 컴포넌트에 대한 리퍼런스를 저장
    private Transform itemTr;
    //인벤토리 위치
    private Transform inventoryTr;

    private Transform itemListTr;
    private CanvasGroup canvasGroup;

    public static GameObject draggingItem = null; // 현재 드래그  중인 아이템의 정보를 가지는 게임 오브젝트 리퍼런스

    //private Camera uiCamera;
    private Canvas canvas; // UI용 최 상위 캔버스

    // Start is called before the first frame update
    void Start()
    {
        // 이 스크립트가 적용되는 게임오브젝트의 Transform 컴포넌트 획득
        itemTr = GetComponent<Transform>();

        inventoryTr = GameObject.Find("Inventory").GetComponent<Transform>();
        canvas = GetComponentInParent<Canvas>();
        //uiCamera = canvas.worldCamera;

        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();

        // 캔버스 그룹 획득
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 드래그 이벤트 핸들러 메소드
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 이벤트가 발생한 오브젝트 즉, 내가 지금 끌고 가려는 아이템의 좌표를 마우스 좌표에 맞춘다. 마치 마우스로 끌고가는 것처럼 보인다. 
        itemTr.position = Input.mousePosition;

    }

    // 드래그가 시작되는 시점에서 실행되는는 이벤트 핸들러 메소드
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 스크립트가 적용되는 게임오브젝트의 좌표 기준을 인벤토리 오브젝트의 좌표로 변경함
        this.transform.SetParent(inventoryTr);
        // 드래그가 시작되면, 해당 아이템의 정보(리퍼런스)를 저장한다.
        draggingItem = this.gameObject;

        // 드래그가 시작되면 다른 UI 이벤트를 받지 않도록 설정
        canvasGroup.blocksRaycasts = false;
    }

    // 드래그가 끝날 때 한 번 호출되는 이벤트 핸들러 메소드
    public void OnEndDrag(PointerEventData eventData)
    {
        // 행동이 끝나면, 해당 리퍼런스를 초기화해서 의도치 않은 오동작을 방지한다.
        draggingItem = null;

        // 드래그가 끝나면 다시 UI 이벤트를 받도록 설정
        canvasGroup.blocksRaycasts = true;

        // 슬롯 위에 드랍된 것이 아니면( = 아직 Inventory 오브젝트 위에 놓여있다면)
        // 정리를 위해 ItemList 오브젝트 위로 되돌린다.
        if (itemTr.parent == inventoryTr)
        {
            itemTr.SetParent(itemListTr.transform);

            // 슬롯에 추가된 아이템 갱신
            GameManager.instance.RemoveItem(GetComponent<ItemInfo>().itemData);
        }
    }
}
