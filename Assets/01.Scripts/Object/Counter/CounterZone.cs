using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterZone : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("Zone과 연결되어 있는 카운터")][SerializeField] Counter counter;

    [Header("Box")]
    [Tooltip("색상 변경을 위한 영역 테두리")][SerializeField] SpriteRenderer zoneImg;
    [Tooltip("입장 시, 크기 변화를 위한 기본값 설정")][SerializeField] Vector3 nomalScale;
    
    // 중복방지 코루틴
    Coroutine enterZoneCoroutine = null;

    #region Coroutine
    IEnumerator EnterZoneCoroutine(Player player)
    {
        zoneImg.color = Color.green;
        counter.StackItem(player);

        for (float i = 1; i < 1.2f; i += 0.01f)
        {
            transform.localScale = nomalScale * i;
            yield return new WaitForSeconds(0.0001f);
        }
        for (float i = 1.2f; i > 1f; i -= 0.01f)
        {
            transform.localScale = nomalScale * i;
            yield return new WaitForSeconds(0.0001f);
        }

        enterZoneCoroutine = null;
    }

    IEnumerator EnterZoneCoroutine(ServeNPC npc)
    {
        zoneImg.color = Color.green;
        counter.StackItem(npc);

        for (float i = 1; i < 1.2f; i += 0.01f)
        {
            transform.localScale = nomalScale * i;
            yield return new WaitForSeconds(0.0001f);
        }
        for (float i = 1.2f; i > 1f; i -= 0.01f)
        {
            transform.localScale = nomalScale * i;
            yield return new WaitForSeconds(0.0001f);
        }

        enterZoneCoroutine = null;
    }
    #endregion

    #region Trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            var player = other.GetComponent<Player>();

            if (enterZoneCoroutine != null)
            {
                StopCoroutine(enterZoneCoroutine);
                enterZoneCoroutine = null;
            }

            enterZoneCoroutine = StartCoroutine(EnterZoneCoroutine(player));
        }

        if (other.gameObject.layer.Equals(11))
        {
            var npc = other.GetComponent<ServeNPC>();

            if (enterZoneCoroutine != null)
            {
                StopCoroutine(enterZoneCoroutine);
                enterZoneCoroutine = null;
            }

            enterZoneCoroutine = StartCoroutine(EnterZoneCoroutine(npc));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            zoneImg.color = Color.white;
        }

        if (other.gameObject.layer.Equals(11))
        {
            zoneImg.color = Color.white;
        }
    }
    #endregion
    
}
