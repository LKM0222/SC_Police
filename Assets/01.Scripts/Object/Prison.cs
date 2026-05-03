using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Prison : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("최대 죄수 카운트")][SerializeField] int maxCount; // 최대 죄수 카운트
    [Tooltip("현재 죄수 카운트")][SerializeField] int count; // 현재 죄수 카운트

    [Header("Object")]
    [Tooltip("텍스트가 표시되는 BackWall")][SerializeField] GameObject backWall; // 텍스트가 표시되는 backWall
    [Tooltip("영역 업그레이드 시, 추가되는 공간")][SerializeField] GameObject addArea; // 추가되는 공간

    [Header("UI")]
    [Tooltip("BackWall에 표시될 TextUI")][SerializeField] TMP_Text countText; // 현재 감옥 카운트 텍스트

    [Header("Transform")]
    [Tooltip("구매 완료한 손님이 이동할 Transform")][SerializeField] Transform prisonPos; // 감옥 위치
    [Tooltip("감옥이 가득찼을 때, 새로 들어오는 손님이 대기할 좌표")][SerializeField] Transform maxPrisonPos; // 감옥이 가득 찼을 경우 위치


    // 프로퍼티
    public bool isMax => count >= maxCount;
    public Transform GetPrisonPos => isMax ? maxPrisonPos : prisonPos;

    // 중복 방지 코루틴
    Coroutine maxTextColorCorountine = null;

    #region Life Cycle
    void Start()
    {
        countText.text = $"{count:D2}/{maxCount:D2}";
    }
    #endregion

    #region Public Method
    public void AddCount()
    {
        if (!isMax)
        {
            count++;
            countText.text = $"{count:D2}/{maxCount:D2}";
            if (isMax)
            {
                if (maxTextColorCorountine == null) maxTextColorCorountine = StartCoroutine(MaxTextColorCoroutine());
            }
        }
        else
        {
            UpgradeManager.Instance.SetUpgradeZone(UpgradeType.Prison, true);
        }
    }
    public void OpenArea()
    {
        backWall.gameObject.SetActive(false);
        addArea.gameObject.SetActive(true);
        maxCount = 40;
        countText.text = $"{count:D2}/{maxCount:D2}";
    }
    #endregion

    #region Coroutine
    IEnumerator MaxTextColorCoroutine()
    {
        countText.color = Color.white;

        while (!addArea.activeSelf)
        {
            for (float i = 0f; i <= 1f; i += 0.01f)
            {
                countText.color = new Color(i, 0f, 0f, 1f);
                yield return new WaitForSeconds(0.01f);
            }

            for (float i = 1f; i >= 0f; i -= 0.01f)
            {
                countText.color = new Color(i, 0f, 0f, 1f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        maxTextColorCorountine = null;
    }
    #endregion

}
