using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("아이템 요구 카운트")] public int needCount;
    [Tooltip("구매한 아이템 갯수")][SerializeField] int enoughCount;
    [Tooltip("구매 속도")][SerializeField] float buySpeed;

    [Header("NavMesh")]
    [Tooltip("손님에 적용된 NavMeshAgent")][SerializeField] NavMeshAgent nav;
    [Tooltip("구매 완료 후, 손님이 향하는 목적지")][SerializeField] Transform targetTransform; // 목적지
    [Tooltip("구매 완료 시, 손님 색상 변경위해 사용하는 MeshRenderer")] public MeshRenderer meshRenderer;

    // 프로퍼티
    public bool isTurn => CustomerManager.Instance.GetMyTime(this) == 0; // 0번쨰 차례가 내 차례
    public float itemRatio => (float)enoughCount / needCount;
    public bool isBuyFinish => needCount == enoughCount;
    public int bubbleCount => needCount - enoughCount;

    // 중복방지 코루틴
    Coroutine getItemCoroutine = null;

    #region Life Cycle
    private void Update()
    {
        if (isTurn && getItemCoroutine == null)
        {
            getItemCoroutine = StartCoroutine(GetItemCoroutine());
        }
    }
    #endregion

    #region Public Method
    public void SetDestination(Transform target)
    {
        targetTransform = target;

        nav.SetDestination(targetTransform.position);
    }

    public void Init(int needCount)
    {
        this.needCount = needCount;
    }
    #endregion

    #region Coroutine
    // 아이템 수령하는곳
    IEnumerator GetItemCoroutine()
    {
        yield return new WaitUntil(() => nav.remainingDistance < 0.01f);
        yield return new WaitUntil(() => GameManager.Instance.counter.GetItemCount() > 0);
        // yield return new WaitForSeconds(1f); // 잠깐 기다린 후, 구매 시작

        for (int i = 0; i < needCount; i++)
        {
            if (isBuyFinish) break;
            yield return new WaitUntil(() => GameManager.Instance.counter.GetItemCount() > 0);
            yield return new WaitForSeconds(buySpeed);

            GameManager.Instance.counter.GetItem();
            enoughCount++;

            SoundManager.Instance.PlaySound(SoundType.GetItem);
        }

        // 구매 완료하면, 캐릭터 색상 변경 후, 감옥으로 이동, wait에서 삭제 후, 다음 손님 스폰
        CustomerManager.Instance.NextCustomer();
    }
    #endregion
}
