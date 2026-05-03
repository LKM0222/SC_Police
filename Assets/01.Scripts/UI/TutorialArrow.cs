using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [Tooltip("화살표가 위 아래로 움직이는 속도")][SerializeField] float speed;

    Coroutine upDownCoroutine = null;


    #region Life Cycle
    // 코루틴 중복실행 방지
    private void OnEnable()
    {
        if (upDownCoroutine != null)
        {
            StopCoroutine(upDownCoroutine);
            upDownCoroutine = null;
        }
        upDownCoroutine = StartCoroutine(UpDownCoroutine());
    }
    #endregion

    #region Coroutine
    // 튜토리얼 화살표를 위 아래로 움직이게 하는 코루틴
    IEnumerator UpDownCoroutine()
    {
        while (true)
        {
            for (float y = 0; y < 0.5f; y += speed)
            {
                transform.localPosition += Vector3.up * speed;
                yield return new WaitForSeconds(0.01f);
            }

            for (float y = 0.5f; y > 0f; y -= speed)
            {
                transform.localPosition -= Vector3.up * speed;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    #endregion
}
