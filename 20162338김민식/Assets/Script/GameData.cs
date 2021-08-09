using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 게임에 사용하는 정보만을 저장하는 모델 클래스

namespace DataInfo
{
    // GameData 클래스는 바이너리 파일 형식으로 저장될 것을 전제로 한다
    // 직렬화 Serialize
    [System.Serializable] // 이 클래스틑 자동적으로 시리얼라이즈(직렬화) 작업이 가능해진다
    public class GameData // 게임에 사용되는 기본 정보를 저장하는 클래스
    {
        public float bestLivingTime = 0.0f; // 생존시간. 최고 생존시간 저장
        public float armor = 0;    // 방어도. 방어도가 올라가면 적에게 맞았을때 내려가는 체력 감소
        public float drill = 0;   // 벽 제거가능 드릴. 드릴이 온 일때 벽과 충돌하면 해당 벽 제거 
        public float characterSpeed = 10.0f; // 캐릭터 이동속도 증가
        public float monsterspeed = 6.0f;    // 적 이동속도. 적 이동속도 감소 
        public List<Item> equipItem = new List<Item>(); // 플레이어가 가지고 있는 아이템의 리스트
    }

    [System.Serializable]
    public class Item // 각각의 아이템에 대한 정보를 저장하는 클래스
    {
        public enum ItemType { ARMOR, SLOW, DRILL, SPEED }//ARMOR, SLOW, DRILL, SPEED
        public enum ItemCalc { INC_VALUE, PERCENT } // 계산 방식 

        public ItemType itemType; // 이 아이템 종류
        public ItemCalc itemCalc; // 이 아이템이 적용될 때의 조건
        public string name;
        public string desc; //소개, description
        public float value; // 적용될 수치
    }
}
