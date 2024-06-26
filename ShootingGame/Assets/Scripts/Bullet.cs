using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    bool isShootEnemy = true;

    //���⿡ ������� or �÷��̾ �������
    //���ʵڿ� ������ٰ� ���������
    //ȭ������� ��������

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)//collision�� ��� �ݸ���
    {
        //���� ����� ��Ȯ�� �� �ʿ䰡 ����
        if (isShootEnemy == false && collision.tag == "Enemy")
        {
            Destroy(gameObject);//�Ѿ� ������ ����
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(1);
        }
        if (isShootEnemy == true && collision.tag == "Player")
        {
            Destroy(gameObject);//�Ѿ� ������ ����
            Player player = collision.GetComponent<Player>();
            player.Hit();
        }
    }

    void Start()
    {
        //Destroy(gameObject, 2.5f);
    }

    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    public void ShootPlayer()
    {
        isShootEnemy = false;
    }
}
