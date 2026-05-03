using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineStackZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            var player = other.GetComponent<Player>();
            GameManager.Instance.machine.GetItme(player);
        }

        if (other.gameObject.layer.Equals(11))
        {
            var npc = other.GetComponent<ServeNPC>();
            GameManager.Instance.machine.GetItem_NPC(npc);
        }
    }
}
