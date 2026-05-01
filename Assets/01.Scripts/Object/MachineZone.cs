using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineZone : MonoBehaviour
{
    [SerializeField] MakeMachine machine; // 제작기계
    [SerializeField] SpriteRenderer zoneImg;
    Coroutine enterZoneCoroutine = null;

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

    IEnumerator EnterZoneCoroutine(Player player)
    {
        zoneImg.color = Color.green;
        machine.AddOre(player.ReturnObj(ObjectType.Ore));

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
}