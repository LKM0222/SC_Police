using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackObject : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("오브젝트가 커졌다 작아지는 속도")][SerializeField] float speed;
    [Tooltip("코루틴 반복 대기 시간(작을수록 빠름)")][SerializeField] float time;
    [Tooltip("기본 Scale")][SerializeField] Vector3 nomalScale;

    // 중복 방지 코루틴
    Coroutine sizePopupCoroutine = null;

    #region Life Cycle
    void OnEnable()
    {
        if (sizePopupCoroutine != null)
        {
            StopCoroutine(sizePopupCoroutine);
            sizePopupCoroutine = null;
        }
        sizePopupCoroutine = StartCoroutine(SizePop());
    }
    #endregion

    #region Coroutine
    IEnumerator SizePop()
    {
        yield return null;
        for (float i = 1; i < 1.2f; i += speed)
        {
            transform.localScale = nomalScale * i;
            yield return new WaitForSeconds(time);
        }
        for (float i = 1.2f; i > 1f; i -= speed)
        {
            transform.localScale = nomalScale * i;
            yield return new WaitForSeconds(time);
        }
        transform.localScale = nomalScale;
    }
    #endregion
}
