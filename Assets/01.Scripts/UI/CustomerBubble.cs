using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerBubble : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("손님의 요구사항 버블 오브젝트")][SerializeField] GameObject bubble;
    [Tooltip("손님 구매 진행상황을 알려줄 FillImg")][SerializeField] Image fillImg;
    [Tooltip("남은 수량 텍스트")][SerializeField] TMP_Text countText;
    [Tooltip("버블 Offset")][SerializeField] Vector3 offset;

    // 프로퍼티
    Customer nowCustomer => CustomerManager.Instance.nowCustomer;

    #region Life Cycle
    private void Update()
    {
        if (nowCustomer == null) return;

        bubble.SetActive(nowCustomer.isTurn);
        fillImg.fillAmount = nowCustomer.itemRatio;
        bubble.transform.position = Camera.main.WorldToScreenPoint(nowCustomer.transform.position + offset);
        countText.text = nowCustomer.bubbleCount.ToString();
    }
    #endregion
}
