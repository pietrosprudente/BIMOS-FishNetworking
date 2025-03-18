using System;
using BIMOS;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class NetworkedHandInputReader : NetworkBehaviour
{
    public HandInputReader handInputReader;

    public float Trigger;
    public float Grip;

    public bool
            TriggerTouched,
            ThumbrestTouched,
            PrimaryTouched,
            PrimaryButton,
            SecondaryTouched,
            SecondaryButton,
            ThumbstickTouched;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handInputReader = GetComponent<HandInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsClientInitialized) return;
        if (!IsOwner)
        {/*
            handInputReader.TriggerAction.action.Disable();
            handInputReader.TriggerTouchedAction.action.Disable();
            handInputReader.TriggerButtonAction.action.Disable();
            handInputReader.GripAction.action.Disable();
            handInputReader.ThumbrestTouchedAction.action.Disable();
            handInputReader.PrimaryTouchedAction.action.Disable();
            handInputReader.PrimaryButtonAction.action.Disable();
            handInputReader.SecondaryTouchedAction.action.Disable();
            handInputReader.SecondaryButtonAction.action.Disable();
            handInputReader.ThumbstickTouchedAction.action.Disable();
            */

            handInputReader.Trigger = Trigger;
            handInputReader.Grip = Grip;
            handInputReader.PrimaryButton = PrimaryButton;
            handInputReader.SecondaryButton = SecondaryButton;
        }
        else
        {
            UpdateFloatValues(handInputReader.Trigger, handInputReader.Grip);
            UpdateBoolValues(handInputReader.TriggerTouched, handInputReader.ThumbrestTouched, handInputReader.PrimaryTouched, handInputReader.PrimaryButton, handInputReader.SecondaryTouched, handInputReader.SecondaryButton, handInputReader.ThumbstickTouched);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdateBoolValues(bool triggerTouched, bool thumbrestTouched, bool primaryTouched, bool primaryButton, bool secondaryTouched, bool secondaryButton, bool thumbstickTouched)
    {
        TriggerTouched = triggerTouched;
        ThumbrestTouched = thumbrestTouched;
        PrimaryTouched = primaryTouched;
        PrimaryButton = primaryButton;
        SecondaryTouched = secondaryTouched;
        SecondaryButton = secondaryButton;
        ThumbstickTouched = thumbstickTouched;
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdateFloatValues(float value1, float value2)
    {
        Trigger = value1;
        Grip = value2;
    }

}
