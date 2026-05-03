using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MakeMachine : MonoBehaviour
{
    [SerializeField] int tempCount;
    [SerializeField] int itemCount;
    [SerializeField] int oreCount;

    [SerializeField] float makeTerm;
    [SerializeField] float makeTime; 
    [SerializeField] List<GameObject> itemList;
    [SerializeField] List<GameObject> oreList;
    [SerializeField] GameObject makeOreObj; // 생선중인 ore(애니메이션용)

    [SerializeField] GameObject orePref;
    [SerializeField] Transform oreListObj; // 광물 parent

    public int OreCount => oreCount;
    public int ItemCount => itemCount;
    Vector3 leftOrePos => oreList[0].transform.localPosition;
    Vector3 rightOrePos => oreList[1].transform.localPosition;


    Coroutine makeCoroutine = null;
    Coroutine addCoroutine = null;
    Coroutine returnItemCoroutine = null;
    Coroutine npcWaitCoroutine = null;



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
        yield return new WaitForSeconds(makeTerm);
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
                yield return new WaitForSeconds(makeTime);
            }

            yield return new WaitForSeconds(0.01f);

            itemList[itemCount].SetActive(true);
            itemCount++;
            SoundManager.Instance.PlaySound(SoundType.ItemMaking);
        }
        makeCoroutine = null;
    }

    IEnumerator AddCoroutine()
    {
        while (tempCount > 0)
        {
            // 리스트의 갯수가 모자르다면, 광물 생성
            if (oreCount >= oreList.Count)
            {
                var ore = Instantiate(orePref, oreListObj);

                float yPos = oreList.Count / 2 * 0.5f;
                float zPos = oreList.Count % 2 == 0 ? leftOrePos.z : rightOrePos.z;

                ore.transform.localPosition = new Vector3(0, yPos, zPos);
                oreList.Add(ore);
            }

            oreList[oreCount].SetActive(true);
            oreCount++;
            tempCount--;
            yield return new WaitForSeconds(0.01f);
        }
        addCoroutine = null;
    }

    IEnumerator ReturnItemCoroutine(Player player)
    {
        int temp = itemCount;

        while (itemCount > 0)
        {
            if (!player.GetItem()) break; // 만약, 더이상 들 수 없다면 함수 종료
            SoundManager.Instance.PlaySound(SoundType.GetItem);
            itemList[(itemCount--) - 1].SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        // if(itemCount > 0) SoundManager.Instance.PlaySound(SoundType.OreStacking); // 아이템이 있는 경우에만 사운드 출력
        returnItemCoroutine = null;
    }

    // 플레이어가 아이템 들기
    public void GetItme(Player player)
    {
        if (returnItemCoroutine == null) returnItemCoroutine = StartCoroutine(ReturnItemCoroutine(player));
    }


    // npc가 아이템 들기
    public void GetItem_NPC(ServeNPC npc)
    {
        if (npcWaitCoroutine == null) npcWaitCoroutine = StartCoroutine(NPCWaitCoroutine(npc));
    }

    IEnumerator NPCWaitCoroutine(ServeNPC npc)
    {
        yield return new WaitUntil(() => itemCount > 0);
        npc.GetItem(itemCount);
        itemList[(itemCount--) - 1].SetActive(false);
        while (itemCount > 0)
        {
            itemList[(itemCount--) - 1].SetActive(false);
            yield return new WaitForSeconds(0.01f);
        }
        
        npcWaitCoroutine = null;
    }
}
