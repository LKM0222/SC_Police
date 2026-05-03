using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public enum ObjectType { Ore, Money, Item }

// 플레이어 오브젝트를 관리하는 스크립트
public class Player : MonoBehaviour
{

    [Header("Turn Object")]
    [Tooltip("플레이어 회전 시 실제로 회전하는 오브젝트")][SerializeField] GameObject mainCharacterObj; // 실제로 회전하는 오브젝트

    [Header("UI")]
    [Tooltip("플레이어가 광물을 가득 들었을 때 나타내는 UI")][SerializeField] GameObject maxText;

    [Tooltip("플레이어가 들고있는 돈 오브젝트 프리팹")][SerializeField] GameObject moneyPref;
    [Tooltip("돈 프리펩 리스트 오브젝트")][SerializeField] GameObject moneyListTransform;

    [Header("Data")]
    [Tooltip("플레이어의 이동 속도")][SerializeField] float speed = 5f; // 플레이어의 이동속도 (초당 단위)
    [Tooltip("플레이어가 바라보는 각도")][SerializeField] float angle = 0f; // 플레이어가 현재 바라보는 각도 (확인용)
    [Tooltip("카메라 회전 각도를 이동 방향에 적용하기 위해, 카메라의 rotation을 사용")][SerializeField] Transform playerCam;
    [Tooltip("플레이어의 공격 사거리")][SerializeField] float attackRange = 1;
    [Tooltip("광물 감지 범위 콜라이더")][SerializeField] BoxCollider attackRangeCollider;
    [Tooltip("플레이어의 공격범위에 들어온 광물")][SerializeField] List<Ore> targetOreList = new List<Ore>();
    [Tooltip("공격력 (광물의 체력은 2)")] float atk = 2;

    [Header("Items")]
    [Tooltip("광물 오브젝트 리스트")][SerializeField] List<GameObject> oreObjList = new List<GameObject>();
    [Tooltip("돈 오브젝트 리스트")][SerializeField] List<GameObject> moneyObjList = new List<GameObject>();
    [Tooltip("아이템 오브젝트 리스트")][SerializeField] List<GameObject> itemObjList = new List<GameObject>();

    [Header("Have Item Data")]
    [Tooltip("들고있는 광물 갯수")][SerializeField] int oreHaveCount;
    [Tooltip("들고있는 아이템 갯수")][SerializeField] int itemHaveCount;
    [Tooltip("최대로 들 수 있는 광물의 갯수")][SerializeField] int maxOreCount = 10;

    /// 프로퍼티
    public bool CanStackOre => oreHaveCount < maxOreCount; // 광물을 캘 수 있는지 여부
    public int GetMoneyCount => GameManager.Instance.money; // GameManager에 저장된 Money값
    float attackCooltime => UpgradeManager.Instance.mineLevel == 1 ? 0.5f : 0.01f; // 공격 쿨타임
    int canAttackTargetCount => UpgradeManager.Instance.mineLevel * 2 - 1; // 공격 범위
    public int OreHaveCount => oreHaveCount;
    public int ItemHaveCount => itemHaveCount;

    /// 코루틴
    Coroutine maxTextFloatingCoroutine = null; 


    #region Life Cycle
    private void Start()
    {
        Init();
    }
    #endregion


    #region Private Method
    private void Init()
    {
        // 모든 리스트, 현재 들고있는 오브젝트 수 만큼 초기화
        foreach (ObjectType category in Enum.GetValues(typeof(ObjectType))) SetListObj(category);

        // 최대 텍스트는 비활성화
        maxText.SetActive(false);

        // 감지 범위 설정
        attackRangeCollider.size = new Vector3(canAttackTargetCount * 0.1f, 1, attackRange);

        // 곡괭이질 시작
        StartCoroutine(Mining());
    }
    #endregion


    #region Behaviour Method
    // 플레이어의 이동을 담당하는 함수
    public void Move(Vector2 vec)
    {
        // 여기서, 카메라 각도 보정값 적용해야됨.
        float refYaw = playerCam.eulerAngles.y;
        Vector3 inputDir = new Vector3(vec.x, 0f, vec.y);
        Vector3 moveDir = Quaternion.Euler(0f, refYaw, 0f) * inputDir;

        //atan2 -> 백터의 방향(각도) 구함, rad2dig -> 라디안을 도(degree)로 변환
        angle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

        this.transform.position += moveDir * speed * Time.deltaTime;
        mainCharacterObj.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    #endregion

    #region Priave Method
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer.Equals(6))
        {
            Ore ore = other.gameObject.GetComponent<Ore>();
            if (ore != null && !targetOreList.Contains(ore))
            {
                if (!ore.isDestory)
                {
                    targetOreList.Add(ore);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(6))
        {
            Ore ore = other.gameObject.GetComponent<Ore>();
            DestroyOre(ore);
        }
    }
    #endregion

    #region Public Method
    // 아이템을 얻었을 때, 리스트 업데이트 위한 함수
    public void GetObject(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Ore:
                {
                    // 더이상 캘 수 없다면 max텍스트 띄우고 함수 종료
                    if (!CanStackOre)
                    {
                        if (maxTextFloatingCoroutine != null)
                        {
                            StopCoroutine(maxTextFloatingCoroutine);
                            maxTextFloatingCoroutine = null;
                        }
                        maxTextFloatingCoroutine = StartCoroutine(MaxTextFloatCoroutine());
                        return;
                    }

                    oreHaveCount = Math.Clamp(oreHaveCount + 1, 0, oreObjList.Count);
                }
                break;
            case ObjectType.Money: GameManager.Instance.money++; break;
            case ObjectType.Item: itemHaveCount = Math.Clamp(itemHaveCount + 1, 0, itemObjList.Count); break;
        }

        SetListObj(type);
    }

    // 플레이어가 가지고 있는 리스트 오브젝트를 업데이트하는 함수
    public void SetListObj(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Ore:
                {
                    for (int i = 0; i < oreObjList.Count; i++)
                    {
                        oreObjList[i].SetActive(i < oreHaveCount);
                    }
                }
                break;

            case ObjectType.Money:
                {
                    if (GameManager.Instance.money > moneyObjList.Count)
                    {
                        int missing = GameManager.Instance.money - moneyObjList.Count;
                        for (int i = 0; i < missing; i++)
                        {
                            var newItem = Instantiate(moneyPref, moneyListTransform.transform);
                            newItem.transform.localPosition = moneyObjList[moneyObjList.Count - 1].transform.localPosition + (Vector3.up * 0.25f);
                            moneyObjList.Add(newItem);
                        }
                    }

                    for (int i = 0; i < moneyObjList.Count; i++)
                    {
                        moneyObjList[i].SetActive(i < GameManager.Instance.money);
                    }
                }
                break;

            case ObjectType.Item:
                {
                    for (int i = 0; i < itemObjList.Count; i++)
                    {
                        itemObjList[i].SetActive(i < itemHaveCount);
                    }
                }
                break;
        }

        // 돈을 들고 있을땐, 유저에게 가까이 붙임.
        moneyListTransform.transform.localPosition = oreHaveCount <= 0 ? Vector3.forward * -0.8f : Vector3.forward * -1.5f;
    }

    public int ReturnObj(ObjectType type)
    {
        int result = 0;

        switch (type)
        {
            case ObjectType.Ore:
                {
                    oreObjList.ForEach(x => x.SetActive(false));
                    result = oreHaveCount;
                    oreHaveCount = 0;
                }
                break;
            case ObjectType.Item:
                {
                    itemObjList.ForEach(x => x.SetActive(false));
                    result = itemHaveCount;
                    itemHaveCount = 0;
                }
                break;
        }

        SetListObj(type);

        return result;
    }

    // 아이템 획득 (갯수 제한이 있어서, 최대로 들 수 있는 갯수라면 못가져오도록)
    public bool GetItem()
    {
        if (itemHaveCount < itemObjList.Count)
        {
            GetObject(ObjectType.Item);
            return true;
        }
        else return false;
    }

    public void PayMoney(int cost)
    {
        moneyObjList.ForEach(x => x.SetActive(false));
        GameManager.Instance.money -= cost;
        SetListObj(ObjectType.Money);
    }

    public void SetFindRange(int level)
    {
        attackRangeCollider.size = new Vector3(level * 1.5f, 1, 0.5f);
        maxOreCount += level * 10; 
    }

    public void DestroyOre(Ore ore)
    {
        targetOreList.Remove(ore);
    }
    #endregion

    #region Coroutine
    // MaxText를 띄우는 코루틴
    IEnumerator MaxTextFloatCoroutine()
    {
        yield return null;
        // 원점으로 복귀
        maxText.transform.localPosition = Vector3.zero;
        maxText.gameObject.SetActive(true);

        for (float y = 0f; y <= 1.5f; y += 0.1f)
        {
            // y 값 서서히 위로
            maxText.transform.localPosition = Vector3.up * y;
            yield return new WaitForSeconds(0.01f);
        }

        // 잠깐 대기 후, 텍스트 비활성화
        yield return new WaitForSeconds(0.1f);
        maxText.gameObject.SetActive(false);
    }

    IEnumerator Mining()
    {
        while (true)
        {
            yield return null;
            // 광물이 없을땐 뛰어넘기
            if (targetOreList.Count < 0) continue;

            // 광물캐기
            int nowCount = 0;
            for (int i = 0; i < targetOreList.Count; i++)
            {
                if (nowCount >= canAttackTargetCount) break;

                // 광물이 파괴되지 않았고, 광물을 캐고 있는 사람이 없을때만 플레이어가 광물을 캘 수 있음
                if (!targetOreList[i].isDestory && targetOreList[i].own == null)
                {
                    targetOreList[i].own = this.gameObject;
                    targetOreList[i].Mined(atk);
                    nowCount++; // 캔 광물 수 더해줌
                }
            }

            // 캐고나서 공격 쿨타임 대기
            yield return new WaitForSeconds(attackCooltime);
        }
    }
    #endregion
}
