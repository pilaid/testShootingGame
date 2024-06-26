using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public float Hp => hp;//get
    //{
    //    get
    //    {
    //        return hp;
    //    }
    //}


    Transform trsBossPosition;//������ ��ġ

    bool isMovingTrsBossPosition = false;//������ ����ġ���� �̵��� �Ϸ��ߴ���
    bool patternChange = false;//������ �ٲٰ� �׵��� ������ �ص��� Ÿ�̹��� �������

    Vector3 createPos = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    float timer = 0.0f;
    float velocityX = 0;
    float velocityY = 0;

    bool isSwayRight = false;

    [Header("������ġ���� ��������")]
    [SerializeField] private int pattern1Count = 10;
    [SerializeField] private float pattern1Reload = 0.5f;
    [SerializeField] private GameObject pattern1Fab;

    [Header("����")]
    [SerializeField] private int pattern2Count = 5;
    [SerializeField] private float pattern2Reload = 0.3f;
    [SerializeField] private GameObject pattern2Fab;

    [Header("���ع߻�")]
    [SerializeField] private int pattern3Count = 30;
    [SerializeField] private float pattern3Reload = 0.1f;
    [SerializeField] private GameObject pattern3Fab;

    Limiter limiter;

    private int curPattern = 1;
    private int curPatternShootCount = 0;

    [Header("�߻���ġ")]
    [SerializeField] List<Transform> trsShootPos;//public���� �����ϰų� �ø�������� �ʵ�� �����Ͽ� �ν����Ϳ��� �����ؼ� ���ÿ��� ���� �����Ҵ��� ���� �ʿ䰡 ����

    Animator anim;

    //[System.Serializable]
    //public class cPattern//�����е��� ����
    //{
    //    public string explain;
    //    public int patternCount;
    //    public float patternReload;
    //    public GameObject patternFab;
    //}
    //[SerializeField] List<cPattern> listPattern;

    protected override void Start()
    {
        gameManager = GameManager.Instance;
        trsBossPosition = gameManager.TrsBossPostion;
        fabExplosion = gameManager.FabExplosion;
        createPos = transform.position;
        anim = GetComponent<Animator>();
    }

    protected override void moving()
    {
        //float posX = Mathf.SmoothDamp(transform.position.x, trsBossPosition.position.x, ref velocityX, 0.5f);
        //float posY = Mathf.SmoothDamp(transform.position.y, trsBossPosition.position.y, ref velocityY, 0.5f);
        //transform.position = new Vector3(posX, posY, 0);

        if (isMovingTrsBossPosition == false) //��ġ���� �̵� 
        {
            if (timer < 1.0f)
            {
                timer += Time.deltaTime;
                //��������
                //transform.position = Vector3.Lerp(createPos, trsBossPosition.position, timer);

                float posX = Mathf.SmoothStep(createPos.x, trsBossPosition.position.x, timer);//0~1
                float posY = Mathf.SmoothStep(createPos.y, trsBossPosition.position.y, timer);
                transform.position = new Vector3(posX, posY, 0);

                if (timer >= 1.0f)
                {
                    isMovingTrsBossPosition = true;
                    timer = 0f;
                }
            }
            return;
        }

        if (isSwayRight == true)
        {
            transform.position += Vector3.right * Time.deltaTime * moveSpeed;
        }
        else
        {
            transform.position += Vector3.left * Time.deltaTime * moveSpeed;
        }

        checkMovingLimit();
    }

    protected override void shooting()
    {
        if (isMovingTrsBossPosition == false)
        {
            return;
        }

        timer += Time.deltaTime;

        if (patternChange == true)
        {
            if (timer >= 1.0f)
            {
                timer = 0.0f;
                patternChange = false;
            }
            return;
        }

        if (curPattern == 1)//�������� �߻�
        {
            if (timer >= pattern1Reload)
            {
                timer = 0.0f;
                shootStraight();
                if (curPatternShootCount >= pattern1Count)
                {
                    curPattern++;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
        else if (curPattern == 2)//����
        {
            if (timer >= pattern2Reload)
            {
                timer = 0.0f;
                shootShotgun();
                if (curPatternShootCount >= pattern2Count)
                {
                    curPattern++;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
        else if (curPattern == 3)//���ع߻�
        {
            if (timer >= pattern3Reload)
            {
                timer = 0.0f;
                shootgatling();
                if (curPatternShootCount >= pattern3Count)
                {
                    curPattern = 1;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
    }

    private void shootgatling()
    {
        Vector3 playerPos;//����Ʈ x 0, y 0, z 0;
        if (gameManager.GetPlayerPosition(out playerPos) == true)
        {
            Vector3 distance = playerPos - transform.position;//�÷��̾��� ��ġ�κ��� ����ġ�� �Ÿ�

            float angle = Quaternion.FromToRotation(Vector3.up, distance).eulerAngles.z;
            //�÷��̾�� �������� �Ÿ� ������ �̿��� y�� 0���� ���� ���� ��ġ�� ���� z�� ����

            Instantiate(pattern3Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, angle)), transform.parent);
        }

        curPatternShootCount++;
    }

    private void shootShotgun()
    {
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 15f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 15f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 30f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 30f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 45f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 45f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 60f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 60f)), transform.parent);

        curPatternShootCount++;
    }

    private void shootStraight()
    {
        int count = 4;
        for(int iNum = 0; iNum < count; ++iNum)
        {
            Instantiate(pattern1Fab, trsShootPos[iNum].position, Quaternion.Euler(new Vector3(0, 0, 180)), transform.parent);
        }

        curPatternShootCount++;
    }

    private void checkMovingLimit()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }

        if (limiter.checkMovePosition(transform.position) == true)
        {
            isSwayRight = !isSwayRight;
        }
    }

    public override void Hit(float _damage)
    {
        if (isDied == true)
        {
            return;
        }

        hp -= _damage;
        gameManager.ModifyBossHp(hp);

        if (hp <= 0)
        {
            isDied = true;
            Destroy(gameObject);
            //�Ŵ����κ��� �޾ƿ� ���� ������ �� ��ġ�� �����ϰ� �θ�� ������� ���̾ �������
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //���簢��
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//���� ��ü�� �̹��� ���̸� �־���

            gameManager.createItem(transform.position, Item.eItemType.PowerUp);
            gameManager.createItem(transform.position, Item.eItemType.HpRecovery);

            //gameManager.AddKillCount();//������ �׾��ٰ� ���� //�ٽ� ������ �⵿�ϵ��� ����
            gameManager.KillBoss();
        }
        else
        {
            //�� ģ���� ��������Ʈ�� Ȱ���ϴ°��� �ƴ϶� ��������Ʈ �ִϸ��̼��� Ȱ�������� �ִϸ��̼ǿ��� ��Ʈ �ִ��� ����
            anim.SetTrigger("bossHit");
        }
    }
}
