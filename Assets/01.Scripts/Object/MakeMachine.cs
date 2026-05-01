using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MakeMachine : MonoBehaviour
{
    [SerializeField] int tempCount;
    [SerializeField] int itemCount;
    [SerializeField] int oreCount;

    [SerializeField] float makeTime;
    [SerializeField] List<GameObject> itemList;
    [SerializeField] List<GameObject> oreList;
    [SerializeField] GameObject makeOreObj; // 생선중인 ore(애니메이션용)


    Coroutine makeCoroutine = null;
    Coroutine addCoroutine = null;
    Coroutine returnItemCoroutine = null;

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (oreCount > 0 && makeCoroutine == null) makeCoroutine = StartCoroutine(MakeCoroutine());
    }

    private void Init()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].SetActive(i < itemCount);
        }
        for (int i = 0; i < oreList.Count; i++)
        {
            oreList[i].SetActive(i < oreCount);
        }
    }

    public void AddOre(int count) // 이제 이걸 외부에서 호출해서 카운트 더해주는걸 해야됨 (발판 만들고, 발판위에 ORE배치하기)
    {
        tempCount += count;
        if (addCoroutine == null) addCoroutine = StartCoroutine(AddCoroutine());
    }

    IEnumerator MakeCoroutine()
    {
        yield return new WaitForSeconds(makeTime);
        while (oreCount > 0)
        {
            yield return new WaitUntil(() => itemCount < itemList.Count);
            if (oreCount == 0) continue;

            oreList[oreCount - 1].SetActive(false);
            oreCount--;

            var makeOrePos = makeOreObj.transform.localPosition;
            for (float i = 0.65f; i > -0.6f; i -= 0.01f)
            {
                makeOreObj.transform.localPosition = new Vector3(0f, makeOrePos.y, i);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(0.01f);

            itemList[itemCount].SetActive(true);
            itemCount++;
        }
        makeCoroutine = null;
    }

    IEnumerator AddCoroutine()
    {
        while (tempCount > 0)
        {
            oreList[oreCount].SetActive(true);
            oreCount++;
            tempCount--;
            yield return new WaitForSeconds(0.01f);
        }
        addCoroutine = null;
    }

    IEnumerator ReturnItemCoroutine(Player player)
    {
        while (itemCount > 0)
        {
            if (!player.GetItem()) break; // 만약, 더이상 들 수 없다면 함수 종료

            itemList[(itemCount--) - 1].SetActive(false);
            yield return new WaitForSeconds(0.01f);
        }
        
        returnItemCoroutine = null;
    }

    // 플레이어가 아이템 들기
    public void GetItme(Player player)
    {
        if (returnItemCoroutine == null) returnItemCoroutine = StartCoroutine(ReturnItemCoroutine(player));
    }
}
