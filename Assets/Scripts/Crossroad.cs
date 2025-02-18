using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossroad : MonoBehaviour
{
    [SerializeField] Player player;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            Debug.Log("Collision");
            player.atCrossroad = true;
            player.movementSpeed = 0;
        }
    }
    
}
