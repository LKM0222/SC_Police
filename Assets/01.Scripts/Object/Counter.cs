using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] List<GameObject> itemList;
    [SerializeField] int nowItemCount = 0;

    [SerializeField] GameObject itemPref;
    [SerializeField] Transform itemParent;

    [SerializeField] GameObject NPC;

    Coroutine stackItemCoroutine = null;

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
    }


    public void StackItem(Player player)
    {
        if (stackItemCoroutine == null) stackItemCoroutine = StartCoroutine(StackItemCoroutine(player));
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
}
