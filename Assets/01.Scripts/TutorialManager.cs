using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public enum TutorialType { Mine, Machine, MachineStack, Counter, CounterMoney }

[Serializable]
public enum TutorialProgressType { NONE, MineStart, MineFinish, MachineOreStack, GetItem, PutDownCounter, WaitCreateCounterMoney, GetCounterMoney }

[Serializable]
public class TutorialInfo
{
    public TutorialType type;
    public TutorialArrow arrow;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<TutorialInfo> tutorialArrows;


    [SerializeField] GameObject playerArrow;

    Coroutine tutorialArrowCoroutine = null;

    [SerializeField] TutorialProgressType nowType;
    [SerializeField] TutorialType followType;


    [Header("시작 커서")]
    [SerializeField] GameObject infinityImg;
    [SerializeField] GameObject cursorImg;
    [SerializeField] Vector3 cursorStartPos;

    [SerializeField] float speed = 1f;
    [SerializeField] float sizeX = 1f;
    [SerializeField] float sizeY = 1f;
    Coroutine drawInfinity = null;

    private void OnEnable()
    {
        tutorialArrows.ForEach(x => x.arrow.gameObject.SetActive(false));

        StartCoroutine(TutorialProgressCoroutine());
        StartCoroutine(DrawInfinity());

    }


    public void SetTutorialArrow(TutorialType type, bool active)
    {
        tutorialArrows.Find(x => x.type.Equals(type)).arrow.gameObject.SetActive(active);
    }

    public void SetPlayerTutorialArrow()
    {
        if (tutorialArrowCoroutine != null)
        {
            StopCoroutine(tutorialArrowCoroutine);
            tutorialArrowCoroutine = null;
        }

        tutorialArrowCoroutine = StartCoroutine(TutorialArrowCoroutine());
    }



    bool IsTutorialFinish(TutorialProgressType type)
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return false;

        switch (type)
        {
            case TutorialProgressType.MineStart:
                {
                    return GameManager.Instance.player.OreHaveCount > 0;
                }

            case TutorialProgressType.MineFinish:
                {
                    return !GameManager.Instance.player.CanStackOre;
                }

            case TutorialProgressType.MachineOreStack:
                {
                    return GameManager.Instance.machine.OreCount > 0;
                }

            case TutorialProgressType.GetItem:
                {
                    return GameManager.Instance.player.ItemHaveCount > 0;
                }

            case TutorialProgressType.PutDownCounter:
                {
                    return GameManager.Instance.player.ItemHaveCount == 0;
                }

            case TutorialProgressType.WaitCreateCounterMoney:
                {
                    return GameManager.Instance.counter.NowMoneyCount > 0;
                }

            case TutorialProgressType.GetCounterMoney:
                {
                    return GameManager.Instance.money > 0;
                }

            default:
                return false;
        }
    }

    IEnumerator TutorialArrowCoroutine()
    {
        Transform nowTarget = tutorialArrows.Find(x => x.type.Equals(followType)).arrow.transform;
        playerArrow.SetActive(true);

        while (!IsTutorialFinish(nowType))
        {
            playerArrow.transform.LookAt(nowTarget);

            Vector3 euler = playerArrow.transform.eulerAngles;
            playerArrow.transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

            yield return new WaitForSeconds(0.01f);
        }

        playerArrow.SetActive(false);
        tutorialArrowCoroutine = null;
    }
    IEnumerator TutorialProgressCoroutine()
    {
        // 광산
        nowType = TutorialProgressType.MineStart;
        followType = TutorialType.Mine;
        SetTutorialArrow(TutorialType.Mine, true);
        SetPlayerTutorialArrow();
        yield return new WaitUntil(() => IsTutorialFinish(nowType));
        SetTutorialArrow(TutorialType.Mine, false);

        // 광산에서 최대치까지 캠
        // 기계에 광물 놓기
        yield return new WaitUntil(() => IsTutorialFinish(TutorialProgressType.MineFinish));
        Debug.Log($" 광물 가득 참 ");
        nowType = TutorialProgressType.MachineOreStack;
        followType = TutorialType.Machine;
        SetTutorialArrow(TutorialType.Machine, true);
        SetPlayerTutorialArrow();
        yield return new WaitUntil(() => IsTutorialFinish(nowType));

        Debug.Log($"광물 내려놓음");
        SetTutorialArrow(TutorialType.Machine, false);

        nowType = TutorialProgressType.GetItem;
        followType = TutorialType.MachineStack;
        SetTutorialArrow(TutorialType.MachineStack, true);
        SetPlayerTutorialArrow();
        yield return new WaitUntil(() => IsTutorialFinish(nowType));

        Debug.Log($"아이템 습득");
        SetTutorialArrow(TutorialType.MachineStack, false);
        yield return new WaitUntil(() => IsTutorialFinish(TutorialProgressType.GetItem));

        nowType = TutorialProgressType.PutDownCounter;
        followType = TutorialType.Counter;
        SetTutorialArrow(TutorialType.Counter, true);
        yield return new WaitUntil(() => IsTutorialFinish(nowType));

        Debug.Log($"아이템 내려놓음");
        SetTutorialArrow(TutorialType.Counter, false);
        yield return new WaitUntil(() => IsTutorialFinish(TutorialProgressType.WaitCreateCounterMoney));

        Debug.Log($"돈 생성됨");
        SetTutorialArrow(TutorialType.CounterMoney, true);
        yield return new WaitUntil(() => IsTutorialFinish(TutorialProgressType.GetCounterMoney));

        Debug.Log($"돈 습득");
        SetTutorialArrow(TutorialType.CounterMoney, false);
    }

    private IEnumerator DrawInfinity()
    {
        float time = 0f;

        yield return new WaitUntil(() => GameManager.Instance != null);

        while (!GameManager.Instance.isInput)
        {
            time += Time.deltaTime * speed;

            float x = Mathf.Sin(time);
            float y = Mathf.Sin(2f * time);

            cursorImg.transform.localPosition = cursorStartPos + new Vector3(x * sizeX, y * sizeY, 0);
            yield return new WaitForSeconds(0.01f);
        }

        infinityImg.SetActive(false);
    }
}
