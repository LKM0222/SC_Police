using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// 튜토리얼에서 사용하는 화살표 오브젝트 종류
[Serializable] public enum TutorialType { Mine, Machine, MachineStack, Counter, CounterMoney }

// 현재 튜토리얼 단게
[Serializable] public enum TutorialProgressType { NONE, MineStart, MineFinish, MachineOreStack, GetItem, PutDownCounter, WaitCreateCounterMoney, GetCounterMoney }

// 튜토리얼 화살표의 정보를 저장하기 위한 클래스
[Serializable]
public class TutorialInfo
{
    public TutorialType type;
    public TutorialArrow arrow;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Object")]
    [Tooltip("튜토리얼 화살표 리스트")][SerializeField] List<TutorialInfo> tutorialArrows;

    [Header("Player")]
    [Tooltip("플레이어 주위를 회전하는 화살표")][SerializeField] GameObject playerArrow;

    [Header("시작 커서")]
    [Tooltip("무한대 이미지")][SerializeField] GameObject infinityImg;
    [Tooltip("커서 이미지")][SerializeField] GameObject cursorImg;
    [Tooltip("무한대를 그리는 시작점")][SerializeField] Vector3 cursorStartPos;
    [Tooltip("무한대를 그리는 속도")][SerializeField] float speed = 1f;
    [Tooltip("무한대 경로 X크기")][SerializeField] float sizeX = 1f;
    [Tooltip("무한대 경로 y크기")][SerializeField] float sizeY = 1f;

    [Header("Data")]
    [Tooltip("현재 진행중인 튜토리얼")][SerializeField] TutorialProgressType nowType;
    [Tooltip("플레이어 주변 화살표가 가리킬 오브젝트 타입")][SerializeField] TutorialType followType;

    Coroutine tutorialArrowCoroutine = null; // 중복방지 코루틴

    #region Life Cycle
    private void OnEnable()
    {
        tutorialArrows.ForEach(x => x.arrow.gameObject.SetActive(false));

        StartCoroutine(TutorialProgressCoroutine());
        StartCoroutine(DrawInfinity());
    }
    #endregion


    #region Public method
    // 오브젝트 위의 화살표 활성화
    public void SetTutorialArrow(TutorialType type, bool active)
    {
        tutorialArrows.Find(x => x.type.Equals(type)).arrow.gameObject.SetActive(active);
    }

    // 플레이어에게 경로를 알려주는 화살표 코루틴 실행 (중복 방지)
    public void SetPlayerTutorialArrow()
    {
        if (tutorialArrowCoroutine != null)
        {
            StopCoroutine(tutorialArrowCoroutine);
            tutorialArrowCoroutine = null;
        }

        tutorialArrowCoroutine = StartCoroutine(PlayerTutorialArrowCoroutine());
    }
    #endregion

    #region Private Method
    // 현재 진행중인 튜토리얼이 종료되었는지 확인
    private bool IsTutorialFinish(TutorialProgressType type)
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
    #endregion

    #region Coroutine
    // 플레이어 주변을 회전하는 화살표 코루틴
    IEnumerator PlayerTutorialArrowCoroutine()
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

    // 튜토리얼 메인 진행 코루틴
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
        playerArrow.SetActive(false);

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
        // 첫 번째 UpgradeZone 활성화
        UpgradeManager.Instance.SetUpgradeZone(UpgradeType.Weapon2, true);
    }

    // 게임 시작 시, 터치 감지하는 코루틴
    IEnumerator DrawInfinity()
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
    #endregion
}
