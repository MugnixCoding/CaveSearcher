using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetective : MonoBehaviour
{
    [SerializeField]
    private Collider AttackRangeCollider;
    [SerializeField]
    private Collider SightCollider;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
    private void OnTriggerStay(Collider other)
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
