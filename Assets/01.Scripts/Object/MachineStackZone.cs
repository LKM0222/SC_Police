using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineStackZone : MonoBehaviour
{
    [SerializeField] MakeMachine machine;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            var player = other.GetComponent<Player>();
            machine.GetItme(player);
        }

        if (other.gameObject.layer.Equals(11))
        {
            var npc = other.GetComponent<ServeNPC>();
            machine.GetItem_NPC(npc);
        }
    }
}
