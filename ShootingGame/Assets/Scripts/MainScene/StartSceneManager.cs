using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnRanking;
    [SerializeField] Button btnExitRanking;
    [SerializeField] Button btnExit;
    [SerializeField] GameObject viewRank;

    [Header("��ũ ������")]
    [SerializeField] GameObject fabRank;
    [SerializeField] Transform contents;

    private string dataPath;
    private void Awake()
    {
        Tool.isStartingMainScene = true;

        #region ��������
        //btnStart.onClick.AddListener(function);

        //UnityAction<float> action = (float _value) => 
        //{
        //    Debug.Log($"���ٽ��� ���� �Ǿ��� => {_value}");
        //};

        //���ٽ�
        //�̸����� �Լ�
        //action.Invoke(0.1f);//���ٽ��� Ư�� �̺�Ʈ�� invoke�� ���ؼ� ���డ��

        //btnStart.onClick.AddListener(() => 
        //{
        //    gameStart(1, 2, 3, 4, 5);
        //});
        #endregion
        btnStart.onClick.AddListener(gameStart);
        btnRanking.onClick.AddListener(showRanking);
        btnExitRanking.onClick.AddListener(() => { viewRank.SetActive(false); });
        btnExit.onClick.AddListener(gameExit);
        #region ��������
        //json
        //string ���ڿ�, Ű�� ����
        //{key:value};

        //save���, ���� ���� �̵��Ҷ� ������ �����ϴ� �����Ͱ� �ִٸ�

        //1.�÷��̾� �������� �̿��� ����Ƽ�� �����ϴ� ���
        //PlayerPrefs//����Ƽ�� ������ �����͸� �����ϵ��� ����Ƽ ���ο� ���� 

        //PlayerPrefs.SetInt("test", 999);//���� ������ 1���� ���� setint setfloat
        //�����͸� �������� �ʴ��� //test 999, ������ �����ϸ� �̵����ʹ� �����ǰ� �ҷ��ü� ����
        //int value = PlayerPrefs.GetInt("test", -1);//int�� ����Ʈ 0�� ���
        //Debug.Log(value);

        //PlayerPrefs.hasKey
        //PlayerPrefs.DeleteKey("test");

        //string path = Application.streamingAssetsPath;//os�� ���� �б��������� ����
        //~/Assets/StreamingAssets
        //File.WriteAllText(path + "/abc.json", "�׽�Ʈ22");
        //File.Delete(path + "/abc.json");
        //string result = File.ReadAllText(path + "/abc.json");
        //Debug.Log(result);

        //string path = Application.persistentDataPath + "/Jsons";//R/W�� ������ ������ġ
        //~/AppData/LocalLow/DefaultCompany/Class6/Jsons

        //if (Directory.Exists(path) == false)
        //{
        //    Directory.CreateDirectory(path);
        //}
        //if (File.Exists(path + "/Test/abc.json") == true)
        //{
        //    string result = File.ReadAllText(path + "Test/abc.json");
        //}
        //else//������ ������ �������� ����
        //{
        //    //���ο� ���� ��ġ�� �����͸� ����� �����
        //    File.Create(path + "/Test");//������ �������
        //}

        //string jsonData = JsonUtility.ToJson(cUserData);
        //{"Name":"������","Score":100}

        //cUserData user2 = new cUserData();
        //user2 = JsonUtility.FromJson<cUserData>(jsonData);

        //string jsonData = JsonUtility.ToJson(listUserData);
        //JsonUtility �� list�� json���� �����ϸ� Ʈ������ ������

        //List<cUserData> listUserData = new List<cUserData>();
        //listUserData.Add(new cUserData() { Name = "������", Score = 100 });
        //listUserData.Add(new cUserData() { Name = "�󸶹�", Score = 200 });

        //string jsonData = JsonConvert.SerializeObject(listUserData);

        //List<cUserData> afterData = JsonConvert.DeserializeObject<List<cUserData>>(jsonData);
        #endregion

        initRankView();
        viewRank.SetActive(false);
    }

    /// <summary>
    /// ��ũ�� ����Ǿ� �ִٸ� ����� ��ũ �����͸� �̿��ؼ� ��ũ�並 ������ְ�
    /// ��ũ�� ����Ǿ� ���� �ʴٸ� ����ִ� ��ũ�� ����� ��ũ�並 �������
    /// </summary>
    private void initRankView()
    {
        List<cUserData> listUserData = null;
        clearRankView();
        if (PlayerPrefs.HasKey(Tool.rankKey) == true)//��ũ �����Ͱ� ������ �Ǿ��־��ٸ�
        {
            listUserData = JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));
        }
        else//��ũ�����Ͱ� ����Ǿ� ���� �ʾҴٸ�
        {
            listUserData = new List<cUserData>();
            int rankCount = Tool.rankCount;
            for (int iNum = 0; iNum < rankCount; ++iNum)
            {
                listUserData.Add(new cUserData() { Name = "None", Score = 0 });
            }

            string value = JsonConvert.SerializeObject(listUserData);
            PlayerPrefs.SetString(Tool.rankKey, value);
        }

        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; ++iNum)
        {
            cUserData data = listUserData[iNum];

            GameObject go = Instantiate(fabRank, contents);
            FabRanking goSc = go.GetComponent<FabRanking>();
            goSc.SetData((iNum + 1).ToString(), data.Name, data.Score);
        }
    }

    private void clearRankView()
    {
        int count = contents.childCount;
        for (int iNum = count - 1; iNum > -1; --iNum)
        {
            Destroy(contents.GetChild(iNum).gameObject);
        }
    }

    private void gameStart()
    {
        FunctionFade.Instance.ActiveFade(true, () => 
        {
            SceneManager.LoadScene(1);
            FunctionFade.Instance.ActiveFade(false);
        });
    }

    private void showRanking()
    {
        viewRank.SetActive(true);
    }

    private void gameExit()
    {
        //�����Ϳ��� �÷��̸� ���� ���, ������ ���� ���
        //���带 ���ؼ� ������ ������ �������� �ȵ�
        //��ó��,�ڵ尡 ���ǿ� ���ؼ� ������ ���°�ó�� Ȥ�� �ִ°�ó�� 
        //�����ϰ� ����

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else//����Ƽ �����Ϳ��� �������� �ʾ�����
        //���������� ���� ����
        Application.Quit();
#endif
    }
}
