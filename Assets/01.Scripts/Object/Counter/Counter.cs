using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("카운터에 쌓아 두는 아이템 오브젝트 리스트")][SerializeField] List<GameObject> itemList;
    [Tooltip("카운터에 생성되는 돈 오브젝트 리스트")][SerializeField] List<GameObject> moneyList;

    [Tooltip("현재 카운터에 생성된 아이템 카운트")][SerializeField] int nowItemCount = 0;
    [Tooltip("현재 카운터에 생성된 돈 카운트")][SerializeField] int nowMoneyCount = 0;

    [Header("Prefab")]
    [Tooltip("추가적으로 생성할 아이템 오브젝트")][SerializeField] GameObject itemPref;
    [Tooltip("아이템 오브젝트를 생성할 Transform (parent)")][SerializeField] Transform itemParent;

    [Header("NPC")]
    [Tooltip("카운터에서 아이템을 운반할 NPC (퀘스트 완료 시 순차개방)")][SerializeField] GameObject NPC;

    // 프로퍼티
    public int GetItemCount() => nowItemCount;
    public int NowMoneyCount => nowMoneyCount;

    // 중복 방지 코루틴
    Coroutine stackItemCoroutine = null;
    Coroutine getMoneyCoroutine = null;

    #region Life Cycle
    private void OnEnable()
    {
        Init();
    }
    #endregion

    #region Public Method
    public void StackItem(Player player)
    {
        int count = player.ReturnObj(ObjectType.Item);

        if (count <= 0) return;
        if (count > 0) SoundManager.Instance.PlaySound(SoundType.OreStacking);
            
        if (stackItemCoroutine == null) stackItemCoroutine = StartCoroutine(StackItemCoroutine(count));
    }

    public void StackItem(ServeNPC npc)
    {   
        int count = npc.ReturnItem();
        if (count <= 0) return;
        if (stackItemCoroutine == null) stackItemCoroutine = StartCoroutine(StackItemCoroutine(count));
    }

    public void GetItem()
    {
        for (int i = itemList.Count - 1; i >= nowItemCount ; i--)
        {
            itemList[i].SetActive(false);
        }

        itemList[(nowItemCount--) - 1].SetActive(false);
    }

    public void AddMoney(int count)
    {
        SoundManager.Instance.PlaySound(SoundType.PayMoney);
        for (int i = nowMoneyCount; i < nowMoneyCount + count; i++)
        {
            moneyList[i].SetActive(true);
        }

        nowMoneyCount += count;
    }

    public void GetMoney(Player player)
    {
        
        if (getMoneyCoroutine == null) getMoneyCoroutine = StartCoroutine(GetMoneyCoroutine(player));
    }
    #endregion
    
    #region Private Method
    private void Init()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].SetActive(i < nowItemCount);
        }
        for (int i = 0; i < moneyList.Count; i++)
        {
            moneyList[i].SetActive(i < nowMoneyCount);
        }
    }
    #endregion

    #region Coroutine
    IEnumerator StackItemCoroutine(int itemCount)
    {
        nowItemCount += itemCount;
        if (nowItemCount > itemList.Count)
        {
            int missing = nowItemCount - itemList.Count;
            for (int i = 0; i < missing; i++)
            {
                var newItem = Instantiate(itemPref, itemParent);
                newItem.transform.localPosition = itemList[itemList.Count - 1].transform.localPosition + (Vector3.up * 0.5f);
                itemList.Add(newItem);
            }
        }

        itemList.ForEach(x => x.gameObject.SetActive(false));

        int showCount = Mathf.Min(nowItemCount, itemList.Count);
        for (int i = 0; i < showCount; i++)
        {
            itemList[i].SetActive(i < showCount);
            yield return new WaitForSeconds(0.01f);
        }
        stackItemCoroutine = null;
    }

    IEnumerator GetMoneyCoroutine(Player player)
    {
        for (int i = 0; i < nowMoneyCount; i++)
        {
            player.GetObject(ObjectType.Money);
            yield return new WaitForSeconds(0.01f);
        }

        // 가져갈 돈이 있는 경우에만 사운드 출력
        if (nowMoneyCount > 0) SoundManager.Instance.PlaySound(SoundType.GetMoney);
        
        moneyList.ForEach(x => x.SetActive(false));
        nowMoneyCount = 0;
        getMoneyCoroutine = null;
    }
    #endregion
}