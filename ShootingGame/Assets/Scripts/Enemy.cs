using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //private �ڽĿ��� �ƹ������͵� ���� Ȥ�� Ȱ���Ҽ� �ֵ��� �������� ����
    //protected ����� �ڽĵ� Ȱ���Ҽ� �ֵ��� ����

    public enum eEnemyType
    {
        EnemyA,
        EnemyB,
        EnemyC,
        EnemyBoss,
    }

    [SerializeField] protected eEnemyType enemyType;

    #region ������Ƽ�� �����͵�
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float hp;
    protected bool isDied = false;//���Ⱑ �װ��� ���̻� ����� �ݺ��������� �ʵ��� ����
    protected GameObject fabExplosion;
    protected GameManager gameManager;
    protected SpriteRenderer spriteRenderer;
    #endregion
    #region ��������Ʈ ������
    Sprite defaultSprite;
    [SerializeField] private Sprite hitSprite;
    bool haveItem = false;
    [Header("������ ������ �÷�")]
    [SerializeField] Color colorHaveItem;
    #endregion

    [Header("�ı��� ����")]
    [SerializeField] protected int score;//�ڽ��� �ı� �Ǿ����� ������ �÷��ٰ�����

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        defaultSprite = spriteRenderer.sprite;

        if (haveItem == true)
        {
            spriteRenderer.color = colorHaveItem;
        }

        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
    }

    protected virtual void Update()
    {
        moving();
        shooting();
    }

    protected virtual void moving()
    {
        transform.position -= transform.up * moveSpeed * Time.deltaTime;

        //transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        //transform.position += transform.rotation * Vector3.down * moveSpeed * Time.deltaTime;
    }

    protected virtual void shooting()
    {
        
    }

    public virtual void Hit(float _damage)
    {
        if (isDied == true)
        {
            return;    
        }

        hp -= _damage;

        if (hp <= 0)
        {
            isDied = true;
            Destroy(gameObject);//������ ����
            //�Ŵ����κ��� �޾ƿ� ���� ������ �� ��ġ�� �����ϰ� �θ�� ������� ���̾ �������
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //���簢��
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//���� ��ü�� �̹��� ���̸� �־���

            //�Ŵ����� ȣ���� ���� �� ��ġ�� �����ϸ� �Ŵ����� �������� �� ��ġ�� �������
            if (haveItem == true)
            { 
                gameManager.createItem(transform.position);
            }

            gameManager.AddScore(score);
            gameManager.AddKillCount();
        }
        else
        {
            //hit ���� ��������Ʈ ������
            spriteRenderer.sprite = hitSprite;
            //�ణ�� �ð��� �����ڿ� � �Լ��� �����ϰ� ������
            Invoke("setDefaultSprite", 0.04f);
        }
    }

    private void setDefaultSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    public void SetItem()
    {
        haveItem = true;
    }
}
