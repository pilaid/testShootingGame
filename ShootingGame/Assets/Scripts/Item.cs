using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //int, string
    public enum eItemType//�ڷ������� ����
    {
        None,
        PowerUp,
        HpRecovery,
    }

    [SerializeField] eItemType ItemType;

    float moveSpeed;//�����̴� �ӵ�
    Vector3 moveDirection;//������ ����

    [SerializeField] float minSpeed = 1;
    [SerializeField] float maxSpeed = 3;

    Limiter limiter;


    private void Awake()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed); //1~3���� � ���̰��� �ӵ�
        moveDirection.x = Random.Range(-1.0f, 1.0f);
        moveDirection.y = Random.Range(-1.0f, 1.0f);

        moveDirection.Normalize();//���Ϳ��� ���� ������ ���⸸ ����
    }

    void Start()
    {
        limiter = GameManager.Instance._Limiter;
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        checkItemPos();
    }

    private void checkItemPos()
    {
        (bool _x, bool _y) rData = limiter.IsReflectItem(transform.position, moveDirection);
        //var rData = limiter.IsReflectItem(transform.position);
        if (rData._x == true)
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.right);
        }
        if(rData._y == true) 
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.up);
        }
    }

    public eItemType GetItemType()
    {
        return ItemType;
    }
}
