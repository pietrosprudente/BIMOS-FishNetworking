using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace BIMOS
{
    public class Hand : NetworkBehaviour
    {
        public HandAnimator HandAnimator;
        public readonly SyncVar<Grab> CurrentGrab;
        public HandInputReader HandInputReader;
        public Transform PalmTransform;
        public PhysicsHand PhysicsHand;
        public Transform PhysicsHandTransform;
        public GrabHandler GrabHandler;
        public bool IsLeftHand;
        public Hand otherHand;
        public Collider PhysicsHandCollider;
    }
}
