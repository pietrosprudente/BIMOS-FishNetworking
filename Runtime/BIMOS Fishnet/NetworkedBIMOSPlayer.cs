using UnityEngine;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using System;
using FishNet.Component.Transforming;
using BIMOS;
using UnityEngine.UI;

[ExecuteAlways]
public class NetworkedBIMOSPlayer : NetworkBehaviour
{
    public static NetworkedBIMOSPlayer LocalPlayer { get; private set; }
    public Player Player;
    
    [SerializeField] private List<Camera> BIMOSCameras;

    [Header("Checklist")]
    public GameObject UIRig;
    public GameObject EventSystem;
    public ModifyTransform Neck;
    public GameObject thirdPersonCamera;

    [Header("Client Authority")]
    public NetworkTransform ControllerRig;
    public NetworkTransform Headset;
    public NetworkTransform LeftController;
    public NetworkTransform RightController;

    [Header("Input")]
    public NetworkedInputReader networkedInputReader;
    public NetworkedHandInputReader LeftNetworkedHandInput;
    public NetworkedHandInputReader RightNetworkedHandInput;


    [Header("Server Authority")]
    [Header("<size=75%>(Make sure to disable Client Auth\n and turn on Rigidbody in Component Config if a rigidbody is connected!)")]
    [Header("<size=85%>*(If you are on fishnet 4\n enable prediction and put the locomotion sphere as the graphical object)")]
    public NetworkTransform LocomotionSphere;
    public NetworkTransform Fender;
    public NetworkTransform Pelvis;
    public NetworkTransform Head;
    public NetworkTransform HeadCollider;
    public NetworkTransform LeftHand;
    public NetworkTransform RightHand;
    public NetworkTransform LeftFootAnchor;
    public NetworkTransform RightFootAnchor;

    private void OnEnable()
    {
        Setup();
    }

    private void Setup()
    {
        Player = GetComponent<Player>();
        BIMOSCameras = GetComponentsInChildren<Camera>().ToList();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Setup();
        if (IsOwner)
        {
            LocalPlayer = this;
        }
        else
        {
            Destroy(UIRig);
            Destroy(EventSystem);
            Destroy(thirdPersonCamera);
            Neck.ResetLocalScale();
            try
            {
                foreach (Camera camera in BIMOSCameras)
                {
                    Destroy(camera.GetComponent<AudioListener>());
                    Destroy(camera);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 1)
                {
                    Debug.Log("Terminated Camera probably did not have a Audio Listener");
                }
            }
        }
    }
}
