using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 오브젝트를 관리하는 스크립트
public class Player : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("플레이어의 이동 속도")]
    [SerializeField] float speed = 5f; // 플레이어의 이동속도 (초당 단위)
    [Tooltip("플레이어가 바라보는 각도")]
    [SerializeField] float angle = 0f; // 플레이어가 현재 바라보는 각도 (확인용)

    [Header("Turn Object")]
    [Tooltip("플레이어 회전 시 실제로 회전하는 오브젝트")]
    [SerializeField] GameObject mainCharacterObj; // 실제로 회전하는 오브젝트


    #region Life Cycle
    private void Update()
    {

    }
    #endregion


    #region Method
    // 플레이어의 이동을 담당하는 함수
    public void Move(Vector2 vec)
    {
        //atan2 -> 백터의 방향(각도) 구함, rad2dig -> 라디안을 도(degree)로 변환환
        angle = Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg;

        this.transform.position += new Vector3(vec.x, 0f, vec.y) * speed * Time.deltaTime;
        mainCharacterObj.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    #endregion
}
