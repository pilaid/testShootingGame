using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//null ä�������
    [Header("�����")]
    [SerializeField] List<GameObject> listEnemy;
    GameObject fabExplosion;//���� �����͸� ������ �ִ� ������ private�� �����ϰ�
    [SerializeField] GameObject fabBoss;

    [Header("�� ���� ����")]
    [SerializeField] bool isSpawn = false;//������ �����ϰų� ���ϴ� ������ ������ �̰���
    [SerializeField] Color sliderDefaultColor;
    [SerializeField] Color sliderBossSpawnColor;

    bool isSpawnBoss = false;//������ ���� ���ӿ� ���� ������
    bool IsSpawnBoss
    {
        set
        {
            isSpawnBoss = value;
            StartCoroutine(sliderColorChange(value));
        }
    }

    IEnumerator sliderColorChange(bool _spawnBoss)//true�� �Ǹ� ������ �⵿�ؼ� ü�¹ٷ� ����� �뵵
    {
        float timer = 0.0f;

        while (timer < 1.0f)//���ǹ��� ���̶�� �ݺ�
        {
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                timer = 1.0f;
            }

            if (_spawnBoss == true)
            {
                imgSliderFill.color = Color.Lerp(sliderDefaultColor, sliderBossSpawnColor, timer);
            }
            else
            {
                imgSliderFill.color = Color.Lerp(sliderBossSpawnColor, sliderDefaultColor, timer);
            }
            yield return null;
        }
    }


    //true �� �����ϸ� ������ ���̻� ������ �ʰ��ϴ� �뵵�� Ȱ��

    [Header("�� ���� �ð�")]
    float enemySpawnTimer = 0.0f;//0�ʿ��� ���۵Ǵ� Ÿ�̸�
    [SerializeField, Range(0.1f, 5f)] float spawnTime = 1.0f;

    [Header("�� ���� ��ġ")]
    [SerializeField] Transform trsSpawnPosition;
    [SerializeField] Transform trsDynamicObject;

    [Header("��Ӿ�����")]
    [SerializeField] List<GameObject> listItem;

    [Header("��� Ȯ��")]
    [SerializeField, Range(0.0f, 100.0f)] float itemDropRate;

    [Header("ü�� ������")]
    [SerializeField] FunctionHP functionHP;
    [SerializeField] Slider slider;
    [SerializeField] Image imgSliderFill;

    [Header("���� ������")]
    [SerializeField] Transform trsBossPostion;
    public Transform TrsBossPostion => trsBossPostion;//get ���


    Limiter limiter;
    public Limiter _Limiter
    {
        get { return limiter; }
        set { limiter = value; }
    }

    Player player;
    public Player _Player
    {
        get { return player; }
        set { player = value; }
    }

    [Header("�������� ����")]
    [SerializeField] int killCount = 100;
    [SerializeField] int curKillCount = 80;
    [SerializeField] TMP_Text textSlider;

    [SerializeField] float bossSpawnTime = 60;
    [SerializeField] float bossSpawnTimer = 0f;

    [Header("����")]
    [SerializeField] TMP_Text textScore;
    private int score;

    private bool gameStart = false;

    [Header("��ŸƮ�ؽ�Ʈ")]
    [SerializeField] TMP_Text textStart;

    [Header("���ӿ����޴�")]
    [SerializeField] GameObject objGameOverMenu;
    [SerializeField] TMP_Text textGameOverMenuScore;
    [SerializeField] TMP_Text textGameOverMenuRank;
    [SerializeField] TMP_Text textGameOverMenuBtn;
    [SerializeField] TMP_InputField IFGameOverMenuRank;
    [SerializeField] Button btnGameOverMenu;


    public bool GetPlayerPosition(out Vector3 _pos)
    {
        _pos = default;
        if (player == null)
        {
            return false;
        }
        else
        {
            _pos = player.transform.position;
            return true;
        }
    }

    public GameObject FabExplosion//������ ���� Ȥ�� �����;��Ҷ��� �Լ��μ� ��밡��
    {
        get
        {
            return fabExplosion;
        }
        //set { fabExplosion = value; }
    }

    //�ν������� ���� ������ ������ ���Լ��� ���� ȣ��
    //private void OnValidate()
    //{
    //    if (Application.isPlaying == false) return;

    //    if (spawnTime < 0.1f)
    //    {
    //        spawnTime = 0.1f;
    //    }
    //}

    private void Awake()
    {
        if (Tool.isStartingMainScene == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        //1���� �����ؾ���
        if (Instance == null)
        {
            Instance = this;
        }
        else//�ν��Ͻ��� �̹� �����Ѵٸ� ���� ����������
        {
            //Destroy(this);//�̷��� ������Ʈ�� ������
            Destroy(gameObject);//������Ʈ�� �������鼭 ��ũ��Ʈ�� ���� ������
        }

        fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion");

        initSlider();
    }

    private void initSlider()
    {
        //ųī��Ʈ ���� 
        //slider.minValue = 0;
        //slider.maxValue = killCount;
        //slider.value = 0;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //Ÿ�̸� ����
        slider.minValue = 0;
        slider.maxValue = bossSpawnTime;
        modifySlider();
    }

    void Start()
    {
        StartCoroutine(doStartText());
    }

    IEnumerator doStartText()
    {
        Color color = textStart.color;
        color.a = 0f;
        textStart.color = color;

        while (true)
        {
            color = textStart.color;
            color.a += Time.deltaTime;
            if (color.a > 1.0f)
            {
                color.a = 1.0f;
            }
            textStart.color = color;

            if (color.a == 1.0f)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            color = textStart.color;
            color.a -= Time.deltaTime;
            if (color.a < 0.0f)
            {
                color.a = 0.0f;
            }
            textStart.color = color;

            if (color.a == 0.0f)
            {
                break;
            }
            yield return null;
        }

        Destroy(textStart.gameObject);

        gameStart = true;
        isSpawn = true;
    }
    
    void Update()//�����Ӵ� �ѹ� ����Ǵ� �Լ�
    {
        if (gameStart == false) return;
        createEnemy();
        checkTimer();
    }

    private void checkTimer()
    {
        if (isSpawnBoss == false)
        {
            bossSpawnTimer += Time.deltaTime;
            modifySlider();
            if (bossSpawnTimer >= bossSpawnTime)//�ð� ������ �Ϸ�ǰ� ���� ������ ����
            {
                checkSpawnBoss();
            }
        }
    }

    private void createEnemy()
    {
        if (isSpawn == false) return;

        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer > spawnTime)
        {
            //���� ����
            int count = listEnemy.Count; //���� ���� 0 ~ 2
            int iRand = Random.Range(0, count);//0, 3

            Vector3 defulatPos = trsSpawnPosition.position;//y => 7 
            float x = Random.Range(limiter.WorldPosLimitMin.x, limiter.WorldPosLimitMax.x);//x => -2.4 ~ 2.4
            defulatPos.x = x;

            GameObject go = Instantiate(listEnemy[iRand], defulatPos, Quaternion.identity, trsDynamicObject);
            //������ ��ġ, ���̳��� ������Ʈ ��ġ�� �ʿ�

            //�ֻ����� ����
            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                //���Ⱑ �������� ������ ����
                Enemy goSc = go.GetComponent<Enemy>();
                goSc.SetItem();
            }

            enemySpawnTimer = 0.0f;
        }
    }

    public void createItem(Vector3 _pos)
    {
        int count = listItem.Count;
        int iRand = Random.Range(0, count);
        Instantiate(listItem[iRand], _pos, Quaternion.identity, trsDynamicObject);
    }

    public void createItem(Vector3 _pos, Item.eItemType _type)//0�� ����, 1 �Ŀ���, 2 ü��ȸ��
    {
        if (_type == Item.eItemType.None) return;
        Instantiate(listItem[(int)_type - 1], _pos, Quaternion.identity, trsDynamicObject);
    }

    public void SetHp(float _maxHp, float _curHp)
    {
        //��� hp���� �˷������
        functionHP.SetHp(_maxHp, _curHp);
    }

    public void AddKillCount()
    {
        curKillCount++;
        //modifySlider();
        //checkSpawnBoss();
    }

    public void AddScore(int _value)//�ڽ��� �������� ����
    {
        score += _value;
        textScore.text = $"{score.ToString("d8")}";
    }

    private void modifySlider()
    {
        //ų ī��Ʈ ����
        //slider.value = curKillCount;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //Ÿ�̸� ����
        slider.value = bossSpawnTimer;
        textSlider.text = $"{((int)bossSpawnTimer).ToString("d4")} / {((int)bossSpawnTime).ToString("d4")}";
    }

    private void checkSpawnBoss()
    {
        //ų ī��Ʈ ����
        //if (isSpawnBoss == false && curKillCount >= killCount)//���� ����
        //{
        //    isSpawn = false;
        //    isSpawnBoss = true;

        //    GameObject go = Instantiate(fabBoss, trsSpawnPosition.position, Quaternion.identity, trsDynamicObject);
        //}

        //Ÿ�̹� ����
        if (isSpawnBoss == false)//���� ����
        {
            isSpawn = false;
            IsSpawnBoss = true;

            GameObject go = Instantiate(fabBoss, trsSpawnPosition.position, Quaternion.identity, trsDynamicObject);
            //����ü���� �ִ� ������ �����ߴ���
            EnemyBoss goSc = go.GetComponent<EnemyBoss>();
            setSliderBossType(goSc.Hp);
        }
    }

    private void setSliderBossType(float _maxHp)
    {
        slider.maxValue = _maxHp;
        slider.value = _maxHp;
        textSlider.text = $"{(int)_maxHp} / {(int)_maxHp}";
    }

    public void ModifyBossHp(float _hp)
    {
        slider.value = _hp;
        textSlider.text = $"{(int)_hp} / {(int)slider.maxValue}";
    }

    public void KillBoss()
    {
        bossSpawnTimer = 0;
        bossSpawnTime += 10;

        spawnTime -= 0.1f;

        //���̵� ���� ����� �߰��ϸ� ��

        isSpawn = true;
        initSlider();

        IsSpawnBoss = false;
    }

    public void GameOver()
    {
        List<cUserData> listUserData = 
            JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));

        int rank = -1;//0 �� 1��
        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            cUserData userData = listUserData[iNum];
            if (userData.Score < score)//�������� ��ũ���� ���ٸ� �ش� ��ũ�� ���� �ؾ���
            {
                rank = iNum;
                break;
            }
        }

        textGameOverMenuScore.text = $"���� : {score.ToString("d8")}";

        //�÷��̾ ��ũ�� ������� Ȯ��,������� ������ �ʿ�
        if (rank != -1)
        {
            textGameOverMenuRank.text = $"��ŷ : {rank + 1}��";
            IFGameOverMenuRank.gameObject.SetActive(true);
            textGameOverMenuBtn.text = "���";
        }
        else//��ũ�ȿ� ���� ���ߴٸ� �̸��� ���� �ʿ䰡 ����
        {
            textGameOverMenuRank.text = "��ũ�� ���� ���߽��ϴ�";
            IFGameOverMenuRank.gameObject.SetActive(false);
            textGameOverMenuBtn.text = "���θ޴���";
        }

        //textGameOverMenuRank.text = rank != -1 ? $"��ŷ : {rank + 1}��" : "��ũ�� ���� ���߽��ϴ�";
        //IFGameOverMenuRank.gameObject.SetActive(rank != -1);
        //textGameOverMenuBtn.text = rank != -1 ? "���" : "���θ޴���";

        btnGameOverMenu.onClick.AddListener(() => 
        {
            //��ũ���� �ߴٸ� ��ũ�� �̸��� ����
            if (rank != -1)
            {
                string name = IFGameOverMenuRank.text;

                if (name == string.Empty)
                {
                    name = "AAA";
                }

                cUserData newRank = new cUserData();
                newRank.Score = score;
                newRank.Name = name;

                listUserData.Insert(rank, newRank);
                listUserData.RemoveAt(listUserData.Count - 1);

                string value = JsonConvert.SerializeObject(listUserData);//����Ǿ�� �� ����
                PlayerPrefs.SetString(Tool.rankKey, value);
            }

            FunctionFade.Instance.ActiveFade(true, () => 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                FunctionFade.Instance.ActiveFade(false);
            });
        });

        objGameOverMenu.SetActive(true);
    }
}
