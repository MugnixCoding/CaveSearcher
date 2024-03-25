using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField]
    private Collider blockingWall;
    [SerializeField]
    private string message;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                if (inventory.GetTresureYet)
                {
                    blockingWall.enabled = false;
                }
                else
                {
                    player.GetComponent<PlayerUIController>().TalkHintMessage(message);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
    }
}
