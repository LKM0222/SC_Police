using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable] public enum UpgradeType {Weapon2, Weapon3, NPC, MineWorker, Prison}

public class UpgradeZone : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("현재 업그레이드의 타입")][SerializeField] UpgradeType upgradeType;
    [Tooltip("업그레이드를 위한 필요금액")][SerializeField] int needMoneyCount;
    [Tooltip("현재까지 업그레이드에 지불한 금액")][SerializeField] int nowMoneyCount;

    [Header("Fill")]
    [Tooltip("업그레이드가 얼마나 진행되었는지 나타내는 FillImg")][SerializeField] SpriteRenderer fillImg;
    [Tooltip("FillImg의 크기를 변경하면서 위치를 조정하기 위한 FillImg Transform")][SerializeField] Transform fillImgTransform;

    [Header("UI")]
    [Tooltip("현재 영역 업그레이드를 위해 남은 돈 표시 텍스트")][SerializeField] TMP_Text moneyText;

    [Header("Cam")]
    [Tooltip("개방 시, 강조해야한다면, 캠 등록 (없으면 뛰어넘음 Null가능)")][SerializeField] GameObject virtualCam;

    // 중복 실행 방지 코루틴
    Coroutine payMoneyCoroutine = null;

    #region Life Cycle
    void OnEnable()
    {
        StartCoroutine(EnableCoroutine());
    }

    private void Update()
    {
        SetUpgradeZoneState();
    }
    #endregion

    #region Private Method
    private void SetUpgradeZoneState()
    {
        // Fill관리, 텍스트 수정
        fillImg.size = new Vector2(1, (float)nowMoneyCount / needMoneyCount);
        fillImgTransform.localPosition = new Vector3(0f, 0.45f, -1.3f * (1 - ((float)nowMoneyCount / needMoneyCount)));
        moneyText.text = (needMoneyCount - nowMoneyCount).ToString();
    }
    #endregion

    #region Trigger
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
    #endregion

    #region Coroutine
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
    #endregion
}
