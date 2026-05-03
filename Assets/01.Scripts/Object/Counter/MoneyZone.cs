using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            var player = other.GetComponent<Player>();
            GameManager.Instance.counter.GetMoney(player);
        }
    }
}
