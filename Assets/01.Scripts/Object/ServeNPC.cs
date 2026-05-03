using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class ServeNPC : MonoBehaviour
{
    [SerializeField] NavMeshAgent nav;

    [SerializeField] Transform itemTransform;
    [SerializeField] Transform counterTransform;
    [SerializeField] List<GameObject> itemList;
    [SerializeField] int itemCount = 0;

    bool isHaveItem => itemCount > 0;

    #region Life Cycle
    void Start()
    {
        SetObjList();
    }

    void OnEnable()
    {
        StartCoroutine(ServeNPCCoroutine());
    }
    #endregion

    #region Private Method
    private void SetObjList()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].SetActive(i < itemCount);
        }
    }
    #endregion
    
    #region Public Method
    public void GetItem(int count)
    {
        itemCount = count;

        SetObjList();
    }

    public int ReturnItem()
    {
        int result = itemCount;
        itemCount = 0;
        SetObjList();
        return result;
    }
    #endregion

    #region Coroutine
    IEnumerator ServeNPCCoroutine()
    {
        yield return null;

        while (true)
        {
            if (isHaveItem) // 카운터 가기
            {
                nav.SetDestination(counterTransform.position);
                yield return new WaitUntil(() => nav.remainingDistance <= nav.stoppingDistance);
                yield return new WaitUntil(() => !isHaveItem);
            }
            else // 기계로 가기
            {
                nav.SetDestination(itemTransform.position);
                yield return new WaitUntil(() => nav.remainingDistance <= nav.stoppingDistance);
                yield return new WaitUntil(() => isHaveItem);
            }
        }
    }
    #endregion
}
