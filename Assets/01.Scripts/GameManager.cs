using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 범용적으로 사용하는 플래그와 변수를 관리하는 스크립트입니다.
public class GameManager : MonoSingleton<GameManager>
{
    [Header("Flag")]
    [Tooltip("현재 입력을 받는 중인지")]
    public bool isInput; // input이 있는지

    [Tooltip("현재 움직일 수 있는 상황인지 (카메라 전환중이 아닌지)")]
    public bool canInput = true; // 움직일 수 있는지
    
    [Tooltip("테이블이 개방되어있는 상태인지")]
    public bool openTable; // 테이블 개방 플래그

    // [Header("Data")]
    // [Tooltip("플레이어가 가지고 있는 돈")]
    public int money; // 가지고있는 돈
    // [Tooltip("크로와상 한개의 가격")]
    // public int croassantPrice; // 크로와상 가격 (7원)

}
