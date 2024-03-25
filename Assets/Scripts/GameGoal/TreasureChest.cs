
using UnityEngine;

public class TreasureChest : InteractableObject
{
    [SerializeField]
    private GameObject chestModel;
    [SerializeField]
    private AudioClip FoundTreasure;
    [SerializeField]
    private AudioSource hintAudioSource;

    private AudioSource InteractAudioSource;

    protected override void Awake()
    {
        base.Awake();
        InteractAudioSource = GetComponent<AudioSource>();
    }
    protected override void Start()
    {
        base.Start();
        DisableOutline();
        chestModel.SetActive(true);
    }
    public override void Interacte(Interactor interactor)
    {
        Inventory interactor_inventory = interactor.gameObject.GetComponent<Inventory>();
        if (interactor_inventory!=null)
        {
            interactor_inventory.GetTresureYet = true;
            transform.GetComponent<Collider>().enabled = false;
            InteractAudioSource.PlayOneShot(FoundTreasure);
            hintAudioSource.Stop();
            chestModel.SetActive(false);
            interactor.GetComponent<PlayerUIController>()?.TalkHintMessage(message);
        }

    }
}
