using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;// 실행 중 데이터를 2진(바이너리) 형식으로 변경
using System.IO; // Input/Output - 데이터 입출력을 위해서
using DataInfo; // GameData 클래스를 사용하기 위해

public class DataManager : MonoBehaviour
{
    // 파일이 저장될 물리 경로
    private string dataPath;

    // 초기화
    public void Initialize()
    {
        dataPath = Application.persistentDataPath + "/gameData.dat";
    }

    // 데이터 파일의 생성 및 데이터 저장
    public void Save(GameData gamedata)
    {
        // 데이터를 저장하기 위해, 인스턴스의 데이터를 선형 2진 데이터로 바꿈
        BinaryFormatter bf = new BinaryFormatter();//데이터 변환용 인스턴스
        FileStream file = File.Create(dataPath); 

        GameData data = new GameData();

        data.bestLivingTime = gamedata.bestLivingTime;
        data.armor = gamedata.armor;
        data.drill = gamedata.drill;
        data.characterSpeed = gamedata.characterSpeed;
        data.monsterspeed = gamedata.monsterspeed;
        data.equipItem = gamedata.equipItem;

        bf.Serialize(file, data); // data를 직선데이터로 변경하고 파일 스트림을 통해 디스크영역으로 옮김
        file.Close();
    }

    // 파일에서 데이터를 가져옴
    public GameData Load()
    {
        if (File.Exists(dataPath)) // 파일이 존재하는가?
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open); // 파일을 읽기용으로 통로를 구축

            GameData data = (GameData)bf.Deserialize(file); // 바이너리포매터를 이용하여, 직렬화 형태에서 원본 형태로 되돌림
            file.Close();// 파일스트림 닫아주기
            return data;
        }
        else
        {
            // 저장되어 있는 파일이 없을 경우, 기본값을 가지는 인스턴스를 만들어서 반환
            GameData data = new GameData();
            return data;
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
