using FishNet.Object;
using UnityEngine;

namespace BIMOS
{
    public class GripChecker : NetworkBehaviour
    {
        public Hand Hand;

        private bool _isGrabbing;

        private void Update()
        {
            if (!IsOwner) return;
            bool wasGrabbing = _isGrabbing;

            _isGrabbing = Hand.HandInputReader.Grip >= 0.5f;

            if (!wasGrabbing && _isGrabbing)
                Hand.GrabHandler.AttemptGrab();

            if (wasGrabbing && !_isGrabbing)
                Hand.GrabHandler.AttemptRelease();
        }
    }
}