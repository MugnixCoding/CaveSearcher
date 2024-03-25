using System;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public event EventHandler OnSuccess;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!=null)
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory!=null)
            {
                if (inventory.GetTresureYet)
                {
                    OnSuccess?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
