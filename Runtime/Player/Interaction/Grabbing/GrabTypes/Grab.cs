using FishNet.Component.Transforming;
using FishNet.Object;
using UnityEngine;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabs/Grab (Basic)")]
    public class Grab : NetworkBehaviour
    {
        [HideInInspector]
        public Hand LeftHand, RightHand;

        public HandPose HandPose;
        public delegate void Release();
        public event Release ReleaseEvent;
        public bool IsLeftHanded = true, IsRightHanded = true;
        public Grab[] EnableGrabs, DisableGrabs;

        private Rigidbody _rigidBody;
        private ArticulationBody _articulationBody;
        private Transform _body;

        [HideInInspector]
        public Collider Collider;

        private void OnEnable()
        {
            _body = Utilities.GetBody(transform, out _rigidBody, out _articulationBody);
            if (!_body)
            {
                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                _rigidBody = rigidbody;
                _body = _rigidBody.transform;
            }

            Collider = GetComponent<Collider>();
            if (Collider)
                return;

            CreateCollider();
        }

        public virtual void CreateCollider()
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.01f;
            Collider = collider;
        }

        public virtual float CalculateRank(Transform handTransform) //Returned when in player grab range
        {
            if (Collider is MeshCollider)
                return 1f / 1000f;

            return 1f / Vector3.Distance(handTransform.position, Collider.ClosestPoint(handTransform.position)); //Reciprocal of distance from hand to grab
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void OnGrab(Hand hand) //Triggered when player grabs the grab
        {
            hand.CurrentGrab.Value = this;

            if (hand.IsLeftHand)
                LeftHand = hand;
            else
                RightHand = hand;

            hand.GrabHandler.ApplyGrabPose(HandPose); //Use the hand pose attached

            AlignHand(hand);
            CreateGrabJoint(hand);

            IgnoreCollision(hand, true);

            foreach (Grab grab in EnableGrabs)
            {
                if (grab)
                    grab.enabled = true;
            }
            foreach (Grab grab in DisableGrabs)
            {
                if (grab)
                    grab.enabled = false;
            }

            GetComponent<Interactable>()?.OnGrab();
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void IgnoreCollision(Hand hand, bool ignore)
        {
            foreach (Collider collider in _body.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(collider, hand.PhysicsHandCollider, ignore);
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void AlignHand(Hand hand) { }

        [ServerRpc(RequireOwnership = false)]
        private void CreateGrabJoint(Hand hand)
        {
            FixedJoint grabJoint = hand.PhysicsHandTransform.gameObject.AddComponent<FixedJoint>();
            grabJoint.enableCollision = true;
            if (_rigidBody)
                grabJoint.connectedBody = _rigidBody;
            if (_articulationBody)
                grabJoint.connectedArticulationBody = _articulationBody;
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnRelease(Hand hand, bool toggleGrabs) //Triggered when player releases the grab
        {
            if (!hand)
                return;

            DestroyGrabJoint(hand);

            if (toggleGrabs)
            {
                foreach (Grab grab in EnableGrabs)
                {
                    if (grab)
                        grab.enabled = false;
                }
                foreach (Grab grab in DisableGrabs)
                {
                    if (grab)
                        grab.enabled = true;
                }
            }

            GetComponent<Interactable>()?.OnRelease();

            hand.CurrentGrab.Value = null;

            if (hand.IsLeftHand)
                LeftHand = null;
            else
                RightHand = null;

            ReleaseEvent?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void DestroyGrabJoint(Hand hand)
        {
            if (!hand)
                return;

            FixedJoint grabJoint = hand.PhysicsHandTransform.GetComponent<FixedJoint>(); //Gets the grab joint
            Destroy(grabJoint); //Deletes the joint, letting it go

            IgnoreCollision(hand, false);

            if (!gameObject.activeSelf)
                return;
        }

        private void OnDisable()
        {
            OnRelease(LeftHand, false);
            OnRelease(RightHand, false);
        }
    }
}