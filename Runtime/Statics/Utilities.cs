using UnityEngine;

namespace BIMOS
{
    public static class Utilities
    {
        public static AudioClip RandomAudioClip(AudioClip[] audioClips)
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
        
        public static Transform GetBody(Transform current, out Rigidbody rigidbody, out ArticulationBody articulationBody)
        {
            rigidbody = null;
            articulationBody = null;
            Transform body = null;
            while (!body)
            {
                rigidbody = current.GetComponent<Rigidbody>();
                articulationBody = current.GetComponent<ArticulationBody>();

                if (rigidbody || articulationBody)
                    return current;

                current = current.parent;
            }
            return null;
        }
    }
}