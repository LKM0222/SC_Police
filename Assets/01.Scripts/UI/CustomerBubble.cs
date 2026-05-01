using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerBubble : MonoBehaviour
{
    [SerializeField] GameObject bubble;
    [SerializeField] Image fillImg;
    [SerializeField] TMP_Text countText;
    [SerializeField] Vector3 offset;
    Customer nowCustomer => CustomerManager.Instance.nowCustomer;
    private void Update()
    {
        if (nowCustomer == null) return;

        bubble.SetActive(nowCustomer.isTurn);
        fillImg.fillAmount = nowCustomer.itemRatio;
        bubble.transform.position = Camera.main.WorldToScreenPoint(nowCustomer.transform.position + offset);
        countText.text = nowCustomer.bubbleCount.ToString();
    }
}
