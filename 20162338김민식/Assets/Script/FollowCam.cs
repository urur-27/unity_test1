using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr; //추적할 대상
    public float dampTrace = 20.0f; //회전속도 계수
    public float dist = 10.0f; //추적 대상과의 거리
    public float height = 3.0f; //추적 대상과의 높이
    //CameraRig의 Transfrom 컴포넌트
    private Transform tr;

    void Start()
    {
        
        tr = GetComponent<Transform>();


    }

    //주인공 캐릭터의 이동 로직이 완료된 후 처리하기 위해 LateUpdate에서 구현
    void LateUpdate()
    {

        //카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고 height 변수만큼 올림
        tr.position = Vector3.Lerp(tr.position
                                    , targetTr.position - (targetTr.forward*dist)+(Vector3.up*height)
                                    , Time.deltaTime * dampTrace);

        tr.LookAt(targetTr.position);
    }

}
