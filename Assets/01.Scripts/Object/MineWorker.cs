using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MineWorker : MonoBehaviour
{
    [SerializeField] NavMeshAgent nav;
    [SerializeField] float atk;
    [SerializeField] float atkSpd = 0.5f;
    [SerializeField] Queue<Ore> targetList = new Queue<Ore>();
    [SerializeField] Ore nowTarget;

    void OnEnable()
    {
        StartCoroutine(MineWorkerMining());
    }

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
            
            nowTarget.WorkersMined(atk);
            if (nowTarget.isDestory) nowTarget = null;
            yield return new WaitForSeconds(Random.Range(atkSpd, atkSpd + 0.5f));
        }
    }

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
}
