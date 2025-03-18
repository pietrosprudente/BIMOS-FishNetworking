using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace BIMOS
{
    public class GrabHandler : NetworkBehaviour
    {
        [SerializeField]
        private Hand _hand;

        [SerializeField]
        private Transform _grabBounds;

        [SerializeField]
        private HandPose _hoverHandPose, _defaultGrabHandPose;

        [SerializeField]
        private AudioClip[] _grabSounds, _releaseSounds;

        [SerializeField]
        private bool ForceGrab = true;

        public readonly SyncVar<Grab> _chosenGrab;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (_hand.CurrentGrab.Value) //If the hand isn't holding something
                return;

            _chosenGrab.Value = GetChosenGrab(); //Get the grab the player is hovering over

            bool grabInRange = _chosenGrab.Value && _hand.HandInputReader.Grip < 0.5f;
            _hand.HandAnimator.HandPose = grabInRange ? _hoverHandPose : _hand.HandAnimator.DefaultHandPose;
        }

        public void ApplyGrabPose(HandPose handPose)
        {
            if (!handPose)
                handPose = _defaultGrabHandPose;

            _hand.HandAnimator.HandPose = handPose;
        }

        private Grab GetChosenGrab()
        {
            List<Collider> grabColliders = Physics.OverlapBox(_grabBounds.position, _grabBounds.localScale / 2, _grabBounds.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide).ToList();
            bool hasHit = Physics.SphereCast(_grabBounds.position, 0.3f, _grabBounds.eulerAngles, out RaycastHit hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Collide);
            if (hasHit && ForceGrab && _hand.HandInputReader.Trigger > 0.5f && hit.collider.GetComponent<SnapGrab>())
            {
                grabColliders.Add(hit.collider);
            }
            //Get all grabs in the grab bounds
            float highestRank = 0;
            Grab highestRankGrab = null;

            foreach (Collider grabCollider in grabColliders) //Loop through found grab colliders to find grab with highest rank
            {
                Grab grab = grabCollider.GetComponent<Grab>();

                if (!grab)
                    grab = grabCollider.GetComponentInParent<Grab>();

                if (!grab)
                    continue;

                if (!grab.enabled)
                    continue;

                if (!grab || !(grab.IsLeftHanded && _hand.IsLeftHand || grab.IsRightHanded && !_hand.IsLeftHand)) //If grab exists and is for the appropriate hand
                    continue;

                float grabRank = grab.CalculateRank(_hand.PalmTransform);

                if (grabRank <= highestRank || grabRank <= 0f)
                    continue;

                highestRank = grabRank;
                highestRankGrab = grab;
            }

            return highestRankGrab; //Return the grab with the highest rank
        }


        [ServerRpc]
        public void AttemptGrab()
        {
            if (!_chosenGrab.Value)
                return;

            _chosenGrab.Value.OnGrab(_hand);
            _audioSource.PlayOneShot(Utilities.RandomAudioClip(_grabSounds));
        }

        [ServerRpc]
        public void AttemptRelease()
        {
            if (!_hand.CurrentGrab.Value)
                return;

            _hand.CurrentGrab.Value.OnRelease(_hand, true);
            _audioSource.PlayOneShot(Utilities.RandomAudioClip(_releaseSounds));
        }

        private void OnDisable()
        {
            AttemptRelease();
        }
    }
}