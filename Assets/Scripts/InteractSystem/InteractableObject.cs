
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline),typeof(Collider))]
public class InteractableObject : MonoBehaviour,IInteractable
{
    protected Outline outline;
    [SerializeField]
    protected string message;
    protected virtual void Awake()
    {
        outline = GetComponent<Outline>();
        gameObject.layer = 8;
    }
    protected virtual void Start()
    {
        DisableOutline();
    }

    public virtual void Interacte(Interactor interactor)
    {
        Debug.Log("Interact: "+message);
    }
    public void DisableOutline()
    {
        outline.enabled = false;
    }
    public void EnableOutline()
    {
        outline.enabled = true;
    }
}
