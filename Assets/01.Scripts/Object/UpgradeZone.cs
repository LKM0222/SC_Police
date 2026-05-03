using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public enum UpgradeType {Weapon2, Weapon3, NPC, MineWorker, Prison}

public class UpgradeZone : MonoBehaviour
{
    [SerializeField] UpgradeType upgradeType;
    [SerializeField] SpriteRenderer fillImg;
    [SerializeField] Transform fillImgTransform;

    [SerializeField] TMP_Text moneyText;
    [SerializeField] int needMoneyCount;
    [SerializeField] int nowMoneyCount;

    [SerializeField] GameObject virtualCam;

    Coroutine payMoneyCoroutine = null;

    void OnEnable()
    {
        StartCoroutine(EnableCoroutine());
    }

    private void Update()
    {
        SetUpgradeZoneState();
    }

    private void SetUpgradeZoneState()
    {
        // Fill관리, 텍스트 수정
        fillImg.size = new Vector2(1, (float)nowMoneyCount / needMoneyCount);
        fillImgTransform.localPosition = new Vector3(0f, 0.45f, -1.3f * (1 - ((float)nowMoneyCount / needMoneyCount)));
        moneyText.text = (needMoneyCount - nowMoneyCount).ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            var player = other.GetComponent<Player>();

            if (payMoneyCoroutine != null)
            {
                StopCoroutine(payMoneyCoroutine);
                payMoneyCoroutine = null;
            }

            payMoneyCoroutine = StartCoroutine(PayMoneyCoroutine(player));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            if (payMoneyCoroutine != null)
            {
                StopCoroutine(payMoneyCoroutine);
                payMoneyCoroutine = null;
            }
        }
    }

    IEnumerator PayMoneyCoroutine(Player player)
    {
        yield return null;

        while (needMoneyCount > nowMoneyCount)
        {
            yield return new WaitUntil(() => player.GetMoneyCount > 0);
            player.PayMoney(1);
            nowMoneyCount++;
            SoundManager.Instance.PlaySound(SoundType.BuyUpgrade);
            yield return new WaitForSeconds(0.01f);
        }

        // 지불 완료, 동작 실행
        UpgradeManager.Instance.OpenUpgrade(upgradeType);
        payMoneyCoroutine = null;
    }

    IEnumerator EnableCoroutine()
    {
        if (virtualCam == null) yield break;

        virtualCam.SetActive(true);
        GameManager.Instance.canInput = false;

        yield return new WaitForSeconds(2f);

        virtualCam.SetActive(false);
        GameManager.Instance.canInput = true;
    }
}
