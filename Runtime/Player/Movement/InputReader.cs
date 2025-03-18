using UnityEngine;
using UnityEngine.InputSystem;

namespace BIMOS
{
    public class InputReader : MonoBehaviour
    {
        [Header("Outputs")]
        public bool broadcastInput = true;
        public Vector2 MoveVector;
        public float CrouchInput, TurnInput;

        public InputActionReference
            MoveAction,
            RunAction,
            CrouchAction,
            JumpAction,
            TurnAction;

        private void Awake()
        {
            //Enable all actions
            broadcastInput = true;
            MoveAction.action.Enable();
            RunAction.action.Enable();
            CrouchAction.action.Enable();
            JumpAction.action.Enable();
            TurnAction.action.Enable();

            //Bind actions to their respective procedures
            RunAction.action.performed += OnRun;
            JumpAction.action.performed += OnJumpAnticipate;
            JumpAction.action.canceled += OnJump;
        }

        private void OnDestroy()
        {
            //Unsubscribe
            broadcastInput = false;
            RunAction.action.performed -= OnRun;
            JumpAction.action.performed -= OnJumpAnticipate;
            JumpAction.action.canceled -= OnJump;
        }

        private void Update()
        {
            OnMove();
            OnCrouchChanged();
            OnTurn();
        }

        //Input actions
        private void OnMove()
        {
            MoveVector = Vector2.ClampMagnitude(MoveAction.action.ReadValue<Vector2>(), 1); //Gets the move vector and clamps it to 1
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            if (!broadcastInput) return;
            BroadcastMessage("Run");
        }

        private void OnCrouchChanged()
        {
            CrouchInput = CrouchAction.action.ReadValue<Vector2>().y;
        }

        private void OnJumpAnticipate(InputAction.CallbackContext context)
        {
            if (!broadcastInput) return;
            BroadcastMessage("AnticipateJump");
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!broadcastInput) return;
            BroadcastMessage("Jump");
        }

        private void OnTurn()
        {
            TurnInput = TurnAction.action.ReadValue<Vector2>().x;
        }
    }
}