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
    }
}
