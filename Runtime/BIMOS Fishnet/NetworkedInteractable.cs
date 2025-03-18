using UnityEngine;
using BIMOS;
using FishNet.Object;
using UnityEngine.Events;
using System;
public class NetworkedInteractable : NetworkBehaviour
{
    public Interactable interactable;

    private UnityEvent
            TriggerDownEvent,
            TriggerUpEvent,
            PrimaryDownEvent,
            PrimaryUpEvent,
            SecondaryDownEvent,
            SecondaryUpEvent,
            GrabEvent,
            ReleaseEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        interactable = GetComponentInParent<Interactable>();
        SetupAllEvents();
        DeleteOldEvents();
        AddRPCEvents();
    }

    private void DeleteOldEvents()
    {
        interactable.TriggerDownEvent.RemoveAllListeners();
        interactable.TriggerUpEvent.RemoveAllListeners();
        interactable.PrimaryDownEvent.RemoveAllListeners();
        interactable.PrimaryUpEvent.RemoveAllListeners();
        interactable.SecondaryDownEvent.RemoveAllListeners();
        interactable.SecondaryUpEvent.RemoveAllListeners();
        interactable.GrabEvent.RemoveAllListeners();
        interactable.ReleaseEvent.RemoveAllListeners();
    }

    private void AddRPCEvents()
    {
        interactable.TriggerDownEvent.AddListener(TriggerDownEventRpc);
        interactable.TriggerUpEvent.AddListener(TriggerUpEventRpc);
        interactable.PrimaryDownEvent.AddListener(PrimaryDownEventRpc);
        interactable.PrimaryUpEvent.AddListener(PrimaryUpEventRpc);
        interactable.SecondaryDownEvent.AddListener(SecondaryDownEventRpc);
        interactable.SecondaryUpEvent.AddListener(SecondaryUpEventRpc);
        interactable.GrabEvent.AddListener(GrabEventRpc);
        interactable.ReleaseEvent.AddListener(ReleaseEventRpc);
    }

    private void SetupAllEvents()
    {
        TriggerDownEvent = interactable.TriggerDownEvent;
        TriggerUpEvent = interactable.TriggerUpEvent;
        PrimaryDownEvent = interactable.PrimaryDownEvent;
        PrimaryUpEvent = interactable.PrimaryUpEvent;
        SecondaryDownEvent = interactable.SecondaryDownEvent;
        SecondaryUpEvent = interactable.SecondaryUpEvent;
        GrabEvent = interactable.GrabEvent;
        ReleaseEvent = interactable.ReleaseEvent;
    }

    void OnDisable()
    {
        interactable = GetComponentInParent<Interactable>();
    }


    [ServerRpc(RequireOwnership = false)]
    private void TriggerDownEventRpc()
    {
        TriggerDownEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void TriggerUpEventRpc()
    {
        TriggerUpEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void PrimaryDownEventRpc()
    {
        PrimaryDownEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void PrimaryUpEventRpc()
    {
        PrimaryUpEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void SecondaryDownEventRpc()
    {
        SecondaryDownEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void SecondaryUpEventRpc()
    {
        SecondaryUpEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void GrabEventRpc()
    {
        GrabEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }
    [ServerRpc(RequireOwnership = false)]
    private void ReleaseEventRpc()
    {
        ReleaseEvent.Invoke();
        Debug.Log("Invoke Interactable");
    }

}
