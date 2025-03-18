using System.Reflection;
using BIMOS;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedInputReader : NetworkBehaviour
{
    public InputReader inputReader;

    public Vector2 MoveVector;
    public float CrouchInput;
    public float TurnInput;

    void Update()
    {
        if (!inputReader) inputReader = GetComponent<InputReader>();
        if (IsOwner)
        {
            if (!inputReader) return;
            inputReader.broadcastInput = true;
            UpdateValues(inputReader.MoveVector, inputReader.CrouchInput, inputReader.TurnInput);
        }
        else
        {
            inputReader.broadcastInput = false;
            inputReader.MoveVector = MoveVector;
            inputReader.CrouchInput = CrouchInput;
            inputReader.TurnInput = TurnInput;
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void Run()
    {
        if (!IsOwner)
            BroadcastMessage("Run");
    }

    [ServerRpc(RequireOwnership = true)]
    public void AnticipateJump()
    {
        if (!IsOwner)
            BroadcastMessage("AnticipateJump");
    }

    [ServerRpc(RequireOwnership = true)]
    public void Jump()
    {
        if (!IsOwner)
            BroadcastMessage("Jump");
    }

    [ServerRpc]
    private void UpdateValues(Vector2 newMove, float newCrouch, float newTurn)
    {
        MoveVector = newMove;
        CrouchInput = newCrouch;
        TurnInput = newTurn;
    }
}
