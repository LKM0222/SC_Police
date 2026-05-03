using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MakeMachine : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("코루틴에서 순차적으로 계산하기 위해, 받은 값을 저장해두는 변수")][SerializeField] int tempCount;
    [Tooltip("현재 기계에 있는 아이템 수")][SerializeField] int itemCount;
    [Tooltip("현재 기계에 있는 광물의 수")][SerializeField] int oreCount;
    [Tooltip("다음 제자까지 대기하는 시간")][SerializeField] float makeTerm;
    [Tooltip("아이템 제작에 걸리는 시간")][SerializeField] float makeTime;

    [Tooltip("현재 오브젝트에 존재하는 아이템 오브젝트 리스트")][SerializeField] List<GameObject> itemList;
    [Tooltip("현재 오브젝트에 존재하는 광물 오브젝트 리스트")][SerializeField] List<GameObject> oreList;
    [Tooltip("생산중인 광물 오브젝트(애니메이션 용도)")][SerializeField] GameObject makeOreObj;

    [Header("Ore")]
    [Tooltip("광물 Prefab")][SerializeField] GameObject orePref;
    [Tooltip("추가적으로 쌓아둘 광물을 보관할 Parent")][SerializeField] Transform oreListObj;

    // 프로퍼티
    public int OreCount => oreCount;
    public int ItemCount => itemCount;
    Vector3 leftOrePos => oreList[0].transform.localPosition;
    Vector3 rightOrePos => oreList[1].transform.localPosition;

    // 중복 실행 방지 코루틴
    Coroutine makeCoroutine = null;
    Coroutine addCoroutine = null;
    Coroutine returnItemCoroutine = null;
    Coroutine npcWaitCoroutine = null;


    #region Life Cycle
    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (oreCount > 0 && makeCoroutine == null) makeCoroutine = StartCoroutine(MakeCoroutine());
    }
    #endregion

    #region Public Method
    public void AddOre(int count) // 이제 이걸 외부에서 호출해서 카운트 더해주는걸 해야됨 (발판 만들고, 발판위에 ORE배치하기)
    {
        tempCount += count;
        if (addCoroutine == null) addCoroutine = StartCoroutine(AddCoroutine());
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

    #endregion

    #region Private Method
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
    #endregion

    #region Coroutine
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
    #endregion
}
