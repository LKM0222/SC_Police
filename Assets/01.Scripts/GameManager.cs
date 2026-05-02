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


    public Player player;
    public MakeMachine machine;


    [Header("Data")]
    public int money; // 가지고있는 돈
}
