using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServeNPC : MonoBehaviour
{
    [Tooltip("아이템 리스트가 있는 위치 (아이템 가지러 가기 위함)")][SerializeField] Transform itemTransform;
    [Tooltip("아이템을 내려놓을 위치 (카운터)")][SerializeField] Transform counterTransform;
    [Tooltip("NPC가 들고있는 아이템 오브젝트 리스트")][SerializeField] List<GameObject> itemList;
    [Tooltip("NPC의 NavMeshAgent")][SerializeField] NavMeshAgent nav;
    [Tooltip("NPC가 들고 있는 아이템 수")][SerializeField] int itemCount = 0;

    // 프로퍼티
    public bool isHaveItem => itemCount > 0;

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
