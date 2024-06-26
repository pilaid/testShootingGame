using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� HP�� ������ ����� HP�������� ��� �����ϰ� Effect�� �ش� �������� �ʴ� �����ϰ� ������ݴϴ�.
/// </summary>

public class FunctionHP : MonoBehaviour
{
    [SerializeField] Image imgHp;
    [SerializeField] Image imgEffect;

    [SerializeField, Range(0.1f, 10f)] float effectTime = 1;
    GameManager gameManager;

    bool isEnded = false;

    private void Awake()
    {
        initHp();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void initHp()
    {
        imgHp.fillAmount = 1;
        imgEffect.fillAmount = 1;
    }

    void Update()
    {
        checkFillAmount();
        chasePlayer();
        checkPlayerDestory();
    }

    private void checkFillAmount()
    {
        if (imgHp.fillAmount == imgEffect.fillAmount)
        {
            return;
        }

        if (imgHp.fillAmount < imgEffect.fillAmount)
        {
            imgEffect.fillAmount -= (Time.deltaTime / effectTime);
            if (imgHp.fillAmount > imgEffect.fillAmount)
            {
                imgEffect.fillAmount = imgHp.fillAmount;
            }
        }
        else if (imgHp.fillAmount > imgEffect.fillAmount)
        {
            imgEffect.fillAmount = imgHp.fillAmount;
        }
    }

    private void chasePlayer()
    {
        if (gameManager.GetPlayerPosition(out Vector3 pos) == true)
        {
            pos.y -= 0.7f;
            transform.position = pos;
        }
    }

    public void SetHp(float _maxHp, float _curHp)//0~1
    {
        imgHp.fillAmount = _curHp / _maxHp;
    }

    private void checkPlayerDestory()
    {
        //if (isEnded == false && gameManager._Player == null)
        //{
        //    isEnded = true;
        //    Destroy(gameObject, 0.5f);
        //}

        if (imgEffect.fillAmount == 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
