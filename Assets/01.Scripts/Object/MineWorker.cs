using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MineWorker : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("광부의 공격력 (광물 체력은 항상 2)")][SerializeField] float atk;
    [Tooltip("광부의 채광 속도")][SerializeField] float atkSpd = 0.5f;
    [Tooltip("현재 광부가 채광하고 있는 오브젝트")][SerializeField] Ore nowTarget;
    [Tooltip("광부의 NavMeshAgent")][SerializeField] NavMeshAgent nav;

    [Tooltip("광물의 타겟 범위에 들어온 큐")][SerializeField] Queue<Ore> targetList = new Queue<Ore>();

    #region Life Cycle
    void OnEnable()
    {
        StartCoroutine(MineWorkerMining());
    }
    #endregion

    #region Coroutine   
    IEnumerator MineWorkerMining()
    {
        while (true)
        {
            yield return new WaitUntil(() => targetList.Count > 0);

            while (nowTarget == null)
            {
                var nextTarget = targetList.Dequeue();
                if (nextTarget.own == null)
                {
                    nextTarget.own = this.gameObject;
                    nowTarget = nextTarget;
                }
            }

            if (nowTarget != null)
            {
                nav.SetDestination(nowTarget.transform.position);
            }

            yield return new WaitUntil(() => nav.remainingDistance <= nav.stoppingDistance);

            nowTarget.Mind_Workers(atk);
            if (nowTarget.isDestory) nowTarget = null;
            yield return new WaitForSeconds(Random.Range(atkSpd, atkSpd + 0.5f));
        }
    }
    #endregion

    #region Trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(6))
        {
            var ore = other.GetComponent<Ore>();
            if (!targetList.Contains(ore))
            {
                targetList.Enqueue(ore);
            }
        }
    }
    #endregion
}
