using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_WEBGL
using WebXR;
#endif

namespace BIMOS
{
    public class HandInputReader : MonoBehaviour
    {
        public Hand Hand;

#if UNITY_WEBGL
        private WebXRController _controller;
#endif

        public float Trigger, Grip;
        public bool
            TriggerTouched,
            ThumbrestTouched,
            PrimaryTouched,
            PrimaryButton,
            SecondaryTouched,
            SecondaryButton,
            ThumbstickTouched;

        public InputActionReference
            TriggerAction,
            TriggerButtonAction,
            TriggerTouchedAction,
            GripAction,
            ThumbrestTouchedAction,
            PrimaryTouchedAction,
            PrimaryButtonAction,
            SecondaryTouchedAction,
            SecondaryButtonAction,
            ThumbstickTouchedAction;

        private void Awake()
        {
            //Enable all actions
            TriggerAction.action.Enable();
            TriggerTouchedAction.action.Enable();
            TriggerButtonAction.action.Enable();
            GripAction.action.Enable();

            TriggerButtonAction.action.performed += OnTriggerButton;
            TriggerButtonAction.action.canceled += OnTriggerButton;

            TriggerTouchedAction.action.performed += OnTriggerTouched;
            TriggerTouchedAction.action.canceled += OnTriggerTouched;

#if UNITY_WEBGL
            _controller = GetComponent<WebXRController>();
#else
            ThumbrestTouchedAction.action.Enable();
            PrimaryTouchedAction.action.Enable();
            PrimaryButtonAction.action.Enable();
            SecondaryTouchedAction.action.Enable();
            SecondaryButtonAction.action.Enable();
            ThumbstickTouchedAction.action.Enable();

            ThumbrestTouchedAction.action.performed += OnThumbrestTouched;
            ThumbrestTouchedAction.action.canceled += OnThumbrestTouched;

            PrimaryTouchedAction.action.performed += OnPrimaryTouched;
            PrimaryTouchedAction.action.canceled += OnPrimaryTouched;

            PrimaryButtonAction.action.performed += OnPrimaryButton;
            PrimaryButtonAction.action.canceled += OnPrimaryButton;

            SecondaryTouchedAction.action.performed += OnSecondaryTouched;
            SecondaryTouchedAction.action.canceled += OnSecondaryTouched;

            SecondaryButtonAction.action.performed += OnSecondaryButton;
            SecondaryButtonAction.action.canceled += OnSecondaryButton;

            ThumbstickTouchedAction.action.performed += OnThumbstickTouched;
            ThumbstickTouchedAction.action.canceled += OnThumbstickTouched;

            ThumbstickTouchedAction.action.performed += OnThumbstickTouched;
            ThumbstickTouchedAction.action.canceled += OnThumbstickTouched;
#endif
        }

        private void Update()
        {
            //Check all of the inputs
            OnTrigger();
            OnGrip();
#if UNITY_WEBGL
            GetWebInputs();
#endif

            if(Hand.CurrentGrab.Value != null) Hand.CurrentGrab.Value?.GetComponent<Interactable>()?.Tick();
        }

        private void FixedUpdate()
        {
            if (Hand.CurrentGrab.Value != null) Hand.CurrentGrab.Value?.GetComponent<Interactable>()?.PhysicsTick();
        }

#if UNITY_WEBGL
        private void GetWebInputs()
        {
            Trigger = _controller.GetAxis(WebXRController.AxisTypes.Trigger);
            TriggerTouched = _controller.GetButtonTouched(WebXRController.ButtonTypes.Trigger);

            Grip = _controller.GetAxis(WebXRController.AxisTypes.Grip);

            PrimaryButton = _controller.GetButton(WebXRController.ButtonTypes.ButtonA);
            PrimaryTouched = _controller.GetButtonTouched(WebXRController.ButtonTypes.ButtonA);

            SecondaryButton = _controller.GetButton(WebXRController.ButtonTypes.ButtonB);
            SecondaryTouched = _controller.GetButtonTouched(WebXRController.ButtonTypes.ButtonB);

            ThumbrestTouched = _controller.GetButtonTouched(WebXRController.ButtonTypes.Touchpad);
            ThumbstickTouched = _controller.GetButtonTouched(WebXRController.ButtonTypes.Thumbstick);

            Grab currentGrab = Hand.CurrentGrab;

            if (!currentGrab)
                return;

            if (_controller.GetButtonDown(WebXRController.ButtonTypes.ButtonA) || _controller.GetButtonUp(WebXRController.ButtonTypes.ButtonA))
                currentGrab.GetComponent<Interactable>()?.OnPrimary(_controller.GetButtonDown(WebXRController.ButtonTypes.ButtonA));

            if (_controller.GetButtonDown(WebXRController.ButtonTypes.ButtonB) || _controller.GetButtonUp(WebXRController.ButtonTypes.ButtonB))
                currentGrab.GetComponent<Interactable>()?.OnSecondary(_controller.GetButtonDown(WebXRController.ButtonTypes.ButtonB));
        }
#endif

        private void OnTrigger()
        {
            Trigger = TriggerAction.action.ReadValue<float>();
        }

        private void OnTriggerButton(InputAction.CallbackContext callbackContext)
        {
            Grab currentGrab = Hand.CurrentGrab.Value;

            if (!currentGrab || callbackContext.started)
                return;

            bool down = IsButtonDown(callbackContext);
            currentGrab.GetComponent<Interactable>()?.OnTrigger(down);
        }

        private void OnTriggerTouched(InputAction.CallbackContext callbackContext)
        {
            TriggerTouched = callbackContext.performed ? true : false;
        }

        private void OnGrip()
        {
            Grip = GripAction.action.ReadValue<float>();
        }

        private void OnThumbrestTouched(InputAction.CallbackContext callbackContext)
        {
            ThumbrestTouched = callbackContext.performed ? true : false;
        }

        private void OnPrimaryTouched(InputAction.CallbackContext callbackContext)
        {
            PrimaryTouched = callbackContext.performed ? true : false;
        }

        private void OnPrimaryButton(InputAction.CallbackContext callbackContext)
        {
            PrimaryButton = callbackContext.performed;

            Grab currentGrab = Hand.CurrentGrab.Value;

            if (!currentGrab || callbackContext.started)
                return;

            bool down = IsButtonDown(callbackContext);
            currentGrab.GetComponent<Interactable>()?.OnPrimary(down);
        }

        private void OnSecondaryTouched(InputAction.CallbackContext callbackContext)
        {
            SecondaryTouched = callbackContext.performed ? true : false;
        }

        private void OnSecondaryButton(InputAction.CallbackContext callbackContext)
        {
            SecondaryButton = callbackContext.performed ? true : false;

            Grab currentGrab = Hand.CurrentGrab.Value;

            if (!currentGrab || callbackContext.started)
                return;

            bool down = IsButtonDown(callbackContext);
            currentGrab.GetComponent<Interactable>()?.OnSecondary(down);
        }

        private void OnThumbstickTouched(InputAction.CallbackContext callbackContext)
        {
            ThumbstickTouched = callbackContext.performed ? true : false;
        }

        private bool IsButtonDown(InputAction.CallbackContext callbackContext)
        {
            return callbackContext.performed;
        }
    }
}
