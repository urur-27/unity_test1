using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler
{
    
    public void OnDrop(PointerEventData eventData)
    {
        // 드래깅 작업이 종료되고 드랍 이벤트가 발생했을 때, 끌고가던 GameObject(아이템)의 Parent 속성을
        // 지금 마우스 위치에 존재하는 Slot Object로 변경하려고함

        if (transform.childCount == 0) // 현재 오브젝트 위에 다른 아이템이나 기타 오브젝트가 올려져 있지 않으면
        {
            // 아이템 오브젝트의 패런트를 이 오브젝트로 변경하여, 이 오브젝트 위에 올라오도록 한다.
            Drag.draggingItem.transform.SetParent(this.transform);

            // 아이템을 슬롯에 올려놓을 때 GameData에 추가
            Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
            GameManager.instance.AddItem(item);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}