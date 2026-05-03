using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] Transform targetTransform; // 목적지
    [SerializeField] NavMeshAgent nav;
    public int needCount;
    [SerializeField] int enoughCount; // 구매한 아이템 갯수
    [SerializeField] float buySpeed; // 구매 속도
    public MeshRenderer meshRenderer; // 매쉬 랜더러

    public bool isTurn => CustomerManager.Instance.GetMyTime(this) == 0; // 0번쨰 차례가 내 차례
    public float itemRatio => (float)enoughCount / needCount;
    public bool isBuyFinish => needCount == enoughCount;
    public int bubbleCount => needCount - enoughCount;


    Coroutine getItemCoroutine = null;

    private void Update()
    {
        if (isTurn && getItemCoroutine == null)
        {
            getItemCoroutine = StartCoroutine(GetItemCoroutine());
        }
    }

    public void SetDestination(Transform target)
    {
        targetTransform = target;

        nav.SetDestination(targetTransform.position);
    }

    public void Init(int needCount)
    {
        this.needCount = needCount;
    }

    // 아이템 수령하는곳
    IEnumerator GetItemCoroutine()
    {
        yield return new WaitUntil(() => nav.remainingDistance < 0.01f);
        yield return new WaitUntil(() => CustomerManager.Instance.counter.GetItemCount() > 0);
        // yield return new WaitForSeconds(1f); // 잠깐 기다린 후, 구매 시작

        for (int i = 0; i < needCount; i++)
        {
            if (isBuyFinish) break;
            yield return new WaitUntil(() => CustomerManager.Instance.counter.GetItemCount() > 0);
            yield return new WaitForSeconds(buySpeed);

            CustomerManager.Instance.counter.GetItem();
            enoughCount++;

            SoundManager.Instance.PlaySound(SoundType.GetItem);
        }

        // 구매 완료하면, 캐릭터 색상 변경 후, 감옥으로 이동, wait에서 삭제 후, 다음 손님 스폰
        CustomerManager.Instance.NextCustomer();
    }
}
