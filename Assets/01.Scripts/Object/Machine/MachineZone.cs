using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineZone : MonoBehaviour
{
    [Header("Obj")]
    [Tooltip("플레이어 입장 시, 색상 변경을 위한 SpriteRendere")][SerializeField] SpriteRenderer zoneImg;

    // 중복 방지 코루틴
    Coroutine enterZoneCoroutine = null;

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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            zoneImg.color = Color.white;
        }
    }
    #endregion

    #region Coroutine
    IEnumerator EnterZoneCoroutine(Player player)
    {
        zoneImg.color = Color.green;
        int playerOreCount = player.ReturnObj(ObjectType.Ore);
        GameManager.Instance.machine.AddOre(playerOreCount);

        if (playerOreCount > 0) SoundManager.Instance.PlaySound(SoundType.OreStacking);

        for (float i = 1; i < 1.2f; i += 0.01f)
        {
            transform.localScale = Vector3.one * i;
            yield return new WaitForSeconds(0.0001f);
        }
        for (float i = 1.2f; i > 1f; i -= 0.01f)
        {
            transform.localScale = Vector3.one * i;
            yield return new WaitForSeconds(0.0001f);
        }

        enterZoneCoroutine = null;
    }
    #endregion
}