using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

[ExecuteInEditMode]
public class NetworkScriptManager : MonoBehaviour
{
    public NetworkObject networkObject;

    public MonoBehaviour[] serverOnlyScripts;
    public MonoBehaviour[] ownerOnlyScripts;
    public MonoBehaviour[] otherOnlyScripts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        networkObject = GetComponentInParent<NetworkObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying || !networkObject.IsClientInitialized) return;
        ManageScripts(networkObject.IsOwner, ownerOnlyScripts);
        ManageScripts(!networkObject.IsOwner, otherOnlyScripts);
        ManageScripts(networkObject.IsServer, serverOnlyScripts);
    }

    public static void ManageScripts(bool canUse, MonoBehaviour[] scriptList)
    {
        foreach (MonoBehaviour script in scriptList)
        {
            if(!canUse) script.enabled = false;
        }
    }
}
