using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{
    [Header("Customer")]
    [Tooltip("손님이 아이템을 구매하기 위해 대기하는 줄")][SerializeField] List<Transform> lineList;
    [Tooltip("감옥 손님 리스트")][SerializeField] List<Customer> prisonList;
    [Tooltip("구매 대기 손님 리스트")][SerializeField] List<Customer> waitList;

    [Header("Prefab")]
    [Tooltip("손님 프리팹")][SerializeField] GameObject customerPrefab;
    [Tooltip("손님이 소환되는 위치")][SerializeField] Transform startPos;
    
    [Header("Customer MT")]
    [Tooltip("구매 완료한 손님의 색상")][SerializeField] Material buyFinishMT; // 구매 완료 후, MT 변경

    // 프로퍼티
    public Customer nowCustomer => waitList[0]; // 현재 차례의 손님은 waitList의 0번 인덱스 손님과 같음

    #region Life Cycle
    private void Start()
    {
        Init();
    }

    #endregion

    #region Public Method
    public int GetMyTime(Customer nowCustomer)
    {
        int nowIdx = waitList.FindIndex(x => x.Equals(nowCustomer));
        return nowIdx;
    }

    public void NextCustomer()
    {
        nowCustomer.SetDestination(GameManager.Instance.prison.GetPrisonPos);
        GameManager.Instance.prison.AddCount();
        nowCustomer.meshRenderer.material = buyFinishMT;
        var count = nowCustomer.needCount;

        prisonList.Add(nowCustomer);
        waitList.Remove(nowCustomer);

        SpawnCustomer();
        SetCustomerLine();
        GameManager.Instance.counter.AddMoney(count);
    }
    #endregion

    #region Private Method
    private void Init()
    {
        // 일단 게임 시작때, 손님 4명 대기
        for (int i = 0; i < 4; i++)
        {
            SpawnCustomer();
        }
        SetCustomerLine();
    }

    private void SpawnCustomer()
    {
        var customer = Instantiate(customerPrefab).GetComponent<Customer>();
        customer.gameObject.transform.position = startPos.position;
        customer.Init(Random.Range(3, 6));
        waitList.Add(customer);
    }

    // 손님이 대기하는 순서대로, 포지션 할당 후, 손님 움직임
    private void SetCustomerLine()
    {
        for (int i = 0; i < waitList.Count; i++)
        {
            waitList[i].SetDestination(lineList[i]);
        }
    }
    #endregion
}
