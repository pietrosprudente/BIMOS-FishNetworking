using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabs/Interactable")]
    public class Interactable : NetworkBehaviour
    {
        public UnityEvent
            TriggerDownEvent,
            TriggerUpEvent,
            PrimaryDownEvent,
            PrimaryUpEvent,
            SecondaryDownEvent,
            SecondaryUpEvent,
            GrabEvent,
            ReleaseEvent;
        public TickEvent OnTick;
        public TickEvent OnPhysicsTick;

        private Grab _grab;

        private void Awake()
        {
            _grab = GetComponent<Grab>();
            _grab.ReleaseEvent += OnRelease;
        }
        private void CheckInputs(out float trigger, out bool primary, out bool secondary)
        {
            float leftTrigger = 0f;
            bool leftPrimary = false;
            bool leftSecondary = false;
            if (_grab.LeftHand)
            {
                leftTrigger = _grab.LeftHand.HandInputReader.Trigger;
                leftPrimary = _grab.LeftHand.HandInputReader.PrimaryButton;
                leftSecondary = _grab.LeftHand.HandInputReader.SecondaryButton;
            }

            float rightTrigger = 0f;
            bool rightPrimary = false;
            bool rightSecondary = false;
            if (_grab.RightHand)
            {
                rightTrigger = _grab.RightHand.HandInputReader.Trigger;
                rightPrimary = _grab.RightHand.HandInputReader.PrimaryButton;
                rightSecondary = _grab.RightHand.HandInputReader.SecondaryButton;
            }

            trigger = Mathf.Max(leftTrigger, rightTrigger);
            primary = leftPrimary || rightPrimary;
            secondary = leftSecondary || rightSecondary;
        }
        public void Tick()
        {
            float trigger;
            bool primary, secondary;
            CheckInputs(out trigger, out primary, out secondary);
            OnTick.Invoke(trigger, primary, secondary);
        }
        public void PhysicsTick()
        {
            float trigger;
            bool primary, secondary;
            CheckInputs(out trigger, out primary, out secondary);
            OnPhysicsTick.Invoke(trigger, primary, secondary);
        }
        [ServerRpc(RequireOwnership = false)]
        public void OnTrigger(bool isButtonDown)
        {
            if (isButtonDown)
                TriggerDownEvent.Invoke();
            else
                TriggerUpEvent.Invoke();
        }
        [ServerRpc(RequireOwnership = false)]
        public void OnPrimary(bool isButtonDown)
        {
            if (isButtonDown)
                PrimaryDownEvent.Invoke();
            else
                PrimaryUpEvent.Invoke();
        }
        [ServerRpc(RequireOwnership = false)]
        public void OnSecondary(bool isButtonDown)
        {
            if (isButtonDown)
                SecondaryDownEvent.Invoke();
            else
                SecondaryUpEvent.Invoke();
        }
        [ServerRpc(RequireOwnership = false)]
        public void OnGrab()
        {
            GrabEvent.Invoke();
        }
        [ServerRpc(RequireOwnership = false)]
        public void OnRelease()
        {
            ReleaseEvent.Invoke();
        }

        [System.Serializable]
        public class TickEvent : UnityEvent<float, bool, bool> { }
    }
}