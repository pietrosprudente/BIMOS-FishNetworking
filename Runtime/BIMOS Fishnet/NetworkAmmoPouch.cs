using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace BIMOS
{
    public class NetworkAmmoPouch : AmmoPouch
    {
        public NetworkObject networkObject;
        private List<NetworkObject> spawnedMagazines = new List<NetworkObject>();

        public override void OnGrab(Hand hand) //Triggered when player grabs the grab
        {
            if (MagazinePrefab == null)
                return;

            OnRelease(hand, true);
            var magazine = Instantiate(MagazinePrefab).GetComponent<NetworkObject>();
            networkObject.Spawn(magazine, networkObject.LocalConnection);
            magazine.transform.SetPositionAndRotation(hand.PhysicsHandTransform.position, hand.PhysicsHandTransform.rotation);

            foreach (Grab grab in magazine.GetComponentsInChildren<SnapGrab>())
                if (grab.IsLeftHanded && hand.IsLeftHand || grab.IsRightHanded && !hand.IsLeftHand)
                {
                    grab.OnGrab(hand);
                    break;
                }

            spawnedMagazines.Add(magazine);
            if (spawnedMagazines.Count > 5)
                foreach (var spawnedMagazine in spawnedMagazines)
                    if (!spawnedMagazine.GetComponentInChildren<Attacher>()?.Socket)
                    {
                        spawnedMagazines.Remove(spawnedMagazine);
                        networkObject.Despawn(spawnedMagazine);
                        break;
                    }
        }
    }
}
