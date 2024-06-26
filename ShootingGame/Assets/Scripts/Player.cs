using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;

    [Header("�÷��̾� ����"), SerializeField, Tooltip("�÷��̾��� �̵��ӵ�")] float moveSpeed;

    Vector3 moveDir;

    [Header("�Ѿ�")]
    [SerializeField] GameObject fabBullet;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet2;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet3;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet4;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet5;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] Transform dynamicObject;
    [SerializeField] bool autoFire = false;//�ڵ����ݱ��
    [SerializeField] float fireRateTime = 0.5f;//�̽ð��� ������ �Ѿ��� �߻��
    float fireTimer = 0;

    GameManager gameManager;
    GameObject fabExplosion;
    SpriteRenderer spriteRenderer;
    Limiter limiter;
    [Header("ü��")]
    [SerializeField] int maxHp = 3;
    [SerializeField] int curHp;
    int beforeHp;
    bool invincibilty;//����
    [SerializeField] float invincibiltyTime = 1f;//�����ð�
    float invincibiltyTimer;

    [Header("�÷��̾� ����")]
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField, Range(1, 5)] int curLevel;

    //[SerializeField] float distanceBullet;//2���� �̻�� �Ѿ��� �߽����� ���� �������� �Ÿ� //�÷��̾� ���濡�� �߻�
    //[SerializeField] float angleBullet;//4���� �̻�� �Ѿ��� ȸ���� ��;
    [SerializeField] Transform shootTrs;
    //[SerializeField] Transform shootTrsLevel4;//4���� �̻�� �Ѿ��� �߻�� ��ġ
    //[SerializeField] Transform shootTrsLevel5;//4���� �̻�� �Ѿ��� �߻�� ��ġ

    private void OnValidate()//�ν����Ϳ��� ����� ������ ����� ȣ��
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        if (beforeHp != curHp)
        {
            beforeHp = curHp;
            GameManager.Instance.SetHp(maxHp, curHp);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Tool.GetTag(GameTags.Enemy))
        {
            //ü�� ����
            Hit();
            
            //ü���� 0�� �Ǹ� ������ ����
            //������ ��ũ���� �Ǹ� �̸� �Է��ϴ� ���
            //���� �޴����� 1~10�� ��ũ

            //ª���ð� ����

            //������ ��ȭ�ڵ� ����
        }
        else if (collision.tag == Tool.GetTag(GameTags.Item))
        {
            Item item = collision.GetComponent<Item>();
            Destroy(item.gameObject);//�� �Լ��� �� �Լ��� ��� ���� ��ġ�� �Ǹ� �����ش޶� ��� �����ϴ� ���

            if (item.GetItemType() == Item.eItemType.PowerUp)
            {
                curLevel++;
                if(curLevel > maxLevel) 
                {
                    curLevel = maxLevel;
                }
            }
            else if (item.GetItemType() == Item.eItemType.HpRecovery)
            {
                //ü�� ȸ��
                curHp++;
                if (curHp > maxHp)
                {
                    curHp = maxHp;
                }
                gameManager.SetHp(maxHp, curHp);
            }
        }
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static  void initCode()
    //{
    //    Debug.Log("initCode");
    //}

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        curHp = maxHp;
        curLevel = minLevel;
    }

    private void Start()
    {
        //cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
        gameManager._Player = this;
    }

    void Update()
    {
        moving();
        doAnimation();
        checkPlayerPos();

        shoot();
        checkInvincibilty();
    }

    private void checkInvincibilty()//�����϶��� �۵��Ͽ� �����ð��� ������ ���� �ٽ� ������ Ǯ����
    {
        if (invincibilty == false) return;

        if (invincibiltyTimer > 0f)
        {
            invincibiltyTimer -= Time.deltaTime;
            if (invincibiltyTimer <= 0f)
            {
                setSprInvincibilty(false);
            }
        }
    }

    private void setSprInvincibilty(bool _value)
    {
        Color color = spriteRenderer.color;
        if (_value == true)//������ �Ȱ�ó�� ������ �ٿ� �������� �����̶� �˷���
        {
            color.a = 0.5f;
            invincibilty = true;
            invincibiltyTimer = invincibiltyTime;
        }
        else
        {
            color.a = 1.0f;
            invincibilty = false;
            invincibiltyTimer = 0f;
        }
        spriteRenderer.color = color;
    }

    /// <summary>
    /// �÷��̾� ��ü�� �⵿�� �����մϴ�.
    /// </summary>
    private void moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");//���� Ȥ�� ������ �Է�// -1 0 1
        moveDir.y = Input.GetAxisRaw("Vertical");//�� Ȥ�� �Ʒ� �Է� // -1 0 1

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �ִϸ��̼ǿ� � �ִϸ��̼��� �������� �Ķ���͸� ���� �մϴ�.
    /// </summary>
    private void doAnimation()//�ϳ��� �Լ����� �ϳ��� ���
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }

    private void checkPlayerPos()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
        transform.position = limiter.checkMovePosition(transform.position, false);
    }

    private void shoot()
    {
        if (autoFire == false && Input.GetKeyDown(KeyCode.Space) == true)//������ �����̽� Ű�� �����ٸ�
        {
            createBullet();
        }
        else if (autoFire == true)
        {
            //�����ð��� ������ �Ѿ� �ѹ� �߻�
            fireTimer += Time.deltaTime;//1�ʰ� ������ 1�� �ɼ��ֵ��� �Ҽ������� fireTimer�� ����
            if (fireTimer > fireRateTime)
            {
                createBullet();
                fireTimer = 0;
            }
        }
    }

    private void createBullet()//�Ѿ��� �����Ѵ�
    {
        if (curLevel == 1)
        {
            GameObject go = Instantiate(fabBullet, shootTrs.position, Quaternion.identity, dynamicObject);
            Bullet goSc = go.GetComponent<Bullet>();
            goSc.ShootPlayer();
            //instBullet(shootTrs.position, Quaternion.identity);
        }
        else if (curLevel == 2)
        {
            Instantiate(fabBullet2, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
        }
        else if (curLevel == 3)
        {
            Instantiate(fabBullet3, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
        }
        else if (curLevel == 4)
        {
            Instantiate(fabBullet4, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);

            //Vector3 lv4Pos = shootTrsLevel4.position;
            //instBullet(lv4Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv4localPos = shootTrsLevel4.localPosition;
            //lv4localPos.x *= -1;
            //lv4localPos += transform.position;

            //instBullet(lv4localPos, new Vector3(0, 0, -angleBullet));
        }
        else if (curLevel == 5)
        {
            Instantiate(fabBullet5, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);

            //Vector3 lv4Pos = shootTrsLevel4.position;
            //instBullet(lv4Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv4localPos = shootTrsLevel4.localPosition;
            //lv4localPos.x *= -1;
            //lv4localPos += transform.position;

            //instBullet(lv4localPos, new Vector3(0, 0, -angleBullet));

            //Vector3 lv5Pos = shootTrsLevel5.position;
            //instBullet(lv5Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv5localPos = shootTrsLevel5.localPosition;
            //lv5localPos.x *= -1;
            //lv5localPos += transform.position;
            //instBullet(lv5localPos, new Vector3(0, 0, -angleBullet));
        }
    }

    private void instBullet(Vector3 _pos, Vector3 _angle)
    {
        GameObject go = Instantiate(fabBullet, _pos, Quaternion.Euler(_angle), dynamicObject);
        Bullet goSc = go.GetComponent<Bullet>();
        goSc.ShootPlayer();
    }
    private void instBullet(Vector3 _pos, Quaternion _quat)
    {
        GameObject go = Instantiate(fabBullet, _pos, _quat, dynamicObject);
        Bullet goSc = go.GetComponent<Bullet>();
        goSc.ShootPlayer();
    }

    public void Hit()
    {
        //�������¶�� �������� ���� ����
        if (invincibilty == true) return;

        setSprInvincibilty(true);
        
        curHp--;
        if(curHp < 0)
        {
            curHp = 0;
        }
        GameManager.Instance.SetHp(maxHp, curHp);

        curLevel--;
        if(curLevel < minLevel)
        {
            curLevel = minLevel;
        }

        if (curHp == 0)
        {
            Destroy(gameObject);
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //���簢��
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//���� ��ü�� �̹��� ���̸� �־���

            gameManager.GameOver();
        }
    }
}
