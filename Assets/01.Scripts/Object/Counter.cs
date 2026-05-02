using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] List<GameObject> itemList;
    [SerializeField] List<GameObject> moneyList;
    [SerializeField] int nowItemCount = 0;
    [SerializeField] int nowMoneyCount = 0;

    [SerializeField] GameObject itemPref;
    [SerializeField] Transform itemParent;

    [SerializeField] GameObject NPC;

    public int GetItemCount() => nowItemCount;


    Coroutine stackItemCoroutine = null;
    Coroutine getMoneyCoroutine = null;

    private void OnEnable()
    {
        Init();
    }

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


    public void StackItem(Player player)
    {
        if (stackItemCoroutine == null) stackItemCoroutine = StartCoroutine(StackItemCoroutine(player));
    }

    public void StackItem(ServeNPC npc)
    {
        if (stackItemCoroutine == null) stackItemCoroutine = StartCoroutine(StackItemCoroutine(npc));
    }

    IEnumerator StackItemCoroutine(Player player)
    {
        nowItemCount += player.ReturnObj(ObjectType.Item);
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
        int showCount = Mathf.Min(nowItemCount, itemList.Count);
        for (int i = 0; i < showCount; i++)
        {
            itemList[i].SetActive(true);
            yield return new WaitForSeconds(0.01f);
        }
        stackItemCoroutine = null;
    }

    IEnumerator StackItemCoroutine(ServeNPC npc)
    {
        nowItemCount += npc.ReturnItem();
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
        int showCount = Mathf.Min(nowItemCount, itemList.Count);
        for (int i = 0; i < showCount; i++)
        {
            itemList[i].SetActive(true);
            yield return new WaitForSeconds(0.01f);
        }
        stackItemCoroutine = null;
    }

    public void GetItem()
    {
        itemList[nowItemCount-- - 1].SetActive(false);
    }

    public void AddMoney(int count)
    {
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

    IEnumerator GetMoneyCoroutine(Player player)
    {
        for (int i = 0; i < nowMoneyCount; i++)
        {
            player.GetObject(ObjectType.Money);
            yield return new WaitForSeconds(0.01f);
        }

        moneyList.ForEach(x => x.SetActive(false));
        nowMoneyCount = 0;
        getMoneyCoroutine = null;
    }
}