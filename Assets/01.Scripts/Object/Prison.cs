using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Prison : MonoBehaviour
{
    [SerializeField] int count; // 현재 죄수 카운트
    [SerializeField] int maxCount; // 최대 죄수 카운트

    [SerializeField] GameObject backWall; // 텍스트가 표시되는 backWall
    [SerializeField] GameObject addArea; // 추가되는 공간

    [SerializeField] TMP_Text countText; // 현재 감옥 카운트 텍스트

    [SerializeField] Transform prisonPos; // 감옥 위치
    [SerializeField] Transform maxPrisonPos; // 감옥이 가득 찼을 경우 위치

    Coroutine maxTextColorCorountine = null;

    public bool isMax => count >= maxCount;
    public Transform GetPrisonPos => isMax ? maxPrisonPos : prisonPos;

    void Start()
    {
        countText.text = $"{count:D2}/{maxCount:D2}";
    }

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

    public void OpenArea()
    {
        backWall.gameObject.SetActive(false);
        addArea.gameObject.SetActive(true);
        maxCount = 40;
        countText.text = $"{count:D2}/{maxCount:D2}";
    }
}
