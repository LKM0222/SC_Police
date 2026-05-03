using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 메인UI를 관리하는 스크립트
public class MainUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("플레이어의 돈을 표시하는 UI")][SerializeField] TMP_Text moneyText;

    #region Life Cycle
    private void Update()
    {
        Refresh();
    }
    #endregion

    #region Private Method
    // 플레이어가 가진 돈을 새로고침하는 함수
    private void Refresh()
    {
        moneyText.text = GameManager.Instance.money.ToString();
    }
    #endregion
}
