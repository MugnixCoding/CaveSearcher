using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{

    [SerializeField] private Transform interactPoint;
    [SerializeField] private float interactRange = 4;
    [SerializeField] private LayerMask interactMask;

    public event EventHandler OnDetectInterectable;
    public event EventHandler OnNoInterectable;

    private InteractableObject currentIntectable;
    private Ray ray;
    
    void Start()
    {
        InputManager.Instance.OnInteractAction += InputManager_OnInteractAction;
        ray = new Ray();
        currentIntectable = null;
    }
    void Update()
    {
        ray.origin = interactPoint.position;
        ray.direction = interactPoint.forward;
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * interactRange);
        if (Physics.Raycast(ray,out hitInfo,interactRange,interactMask))
        {
            InteractableObject newInteractable = null;
            if (hitInfo.collider.TryGetComponent<InteractableObject>(out newInteractable))
            {
                if (currentIntectable != null && newInteractable != currentIntectable)
                {
                    DisableCurrentInteractable();
                }
                if (currentIntectable != newInteractable)
                {
                    if (newInteractable.enabled)
                    {
                        SetNewCurrentInteractable(newInteractable);
                    }
                    else
                    {
                        DisableCurrentInteractable();
                    }
                }
            }
            else
            {
                DisableCurrentInteractable();
            }
        }
        else
        {
            DisableCurrentInteractable();
        }


    }

    private void InputManager_OnInteractAction(object sender, System.EventArgs e)
    {
        Interact();
    }
    private void Interact()
    {
        if (currentIntectable !=null)
        {
            currentIntectable.Interacte(this);
        }
    }
    private void SetNewCurrentInteractable(InteractableObject newInteractable)
    {
        currentIntectable = newInteractable;
        currentIntectable.EnableOutline();
        OnDetectInterectable?.Invoke(this,EventArgs.Empty);
    }
    private void DisableCurrentInteractable()
    {
        if (currentIntectable!=null)
        {
            currentIntectable.DisableOutline();
            currentIntectable = null;
            OnNoInterectable?.Invoke(this,EventArgs.Empty);
        }
    }
}
