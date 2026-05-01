using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{
    [SerializeField] List<Transform> lineList; // 손님이 아이템을 구매하기 위해 대기하는 줄
    [SerializeField] Transform startPos; // 손님이 소환되는 위치
    [SerializeField] List<Customer> prisonList; // 감옥 손님 리스트
    [SerializeField] List<Customer> waitList; // 구매 대기 손님 리스트
    
    
    [SerializeField] Material buyFinishMT; // 구매 완료 후, MT 변경

    public Counter counter;

    public Customer nowCustomer => waitList[0];



    [SerializeField] GameObject customerPrefab; // 손님 프리팹

    private void Start()
    {
        Init();
    }

    void Init()
    {
        // 일단 게임 시작때, 손님 4명 대기
        for (int i = 0; i < 4; i++)
        {
            SpawnCustomer();
        }
        SetCustomerLine();
    }

    void SpawnCustomer()
    {
        var customer = Instantiate(customerPrefab).GetComponent<Customer>();
        customer.gameObject.transform.position = startPos.position;
        customer.Init(Random.Range(3, 6));
        waitList.Add(customer);
    }

    // 손님이 대기하는 순서대로, 포지션 할당 후, 손님 움직임
    void SetCustomerLine()
    {
        for (int i = 0; i < waitList.Count; i++)
        {
            waitList[i].SetDestination(lineList[i]);
        }
    }

    public int GetMyTime(Customer nowCustomer)
    {
        int nowIdx = waitList.FindIndex(x => x.Equals(nowCustomer));
        return nowIdx;
    }

    public void NextCustomer()
    {
        nowCustomer.SetDestination(counter.prisonPos);
        nowCustomer.meshRenderer.material = buyFinishMT;
        
        prisonList.Add(nowCustomer);
        waitList.Remove(nowCustomer);

        SpawnCustomer();
        SetCustomerLine();
        
    }
}
