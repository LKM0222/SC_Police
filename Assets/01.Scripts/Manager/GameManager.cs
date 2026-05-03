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

    [Header("Refrence")]
    [Tooltip("플레이어 오브젝트")]public Player player;
    [Tooltip("제작 기계 오브젝트")]public MakeMachine machine;
    [Tooltip("카운터 오브젝트")]public Counter counter;
    [Tooltip("감옥 오브젝트")] public Prison prison;


    [Header("Data")]
    [Tooltip("플레이어가 가지고 있는 돈")]public int money; // 가지고있는 돈
}
