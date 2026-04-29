using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 메인UI를 관리하는 스크립트
public class MainUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("플레이어의 돈을 표시하는 UI")]
    [SerializeField] TMP_Text moneyText;
    [Tooltip("게임 시작 시 표시되는 UI")]
    [SerializeField] GameObject IntroUI;

    #region Life Cycle
    private void Start()
    {
        StartCoroutine(InputCheck());
    }

    private void Update()
    {
        Refresh();
    }
    #endregion

    #region Method
    // 플레이어가 가진 돈을 새로고침하는 함수
    void Refresh()
    {
        // moneyText.text = GameManager.Instance.money.ToString();
    }
    #endregion

    #region Coroutine
    // 게임 시작을 위해 입력이 있는지 체크하는 코루틴
    IEnumerator InputCheck()
    {
        yield return new WaitUntil(() => GameManager.Instance.isInput);
        // IntroUI.SetActive(false);
    }
    #endregion
}
