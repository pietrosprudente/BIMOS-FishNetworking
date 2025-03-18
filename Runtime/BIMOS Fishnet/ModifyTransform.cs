using UnityEngine;

public class ModifyTransform : MonoBehaviour
{
    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }
    public void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }
    public void ResetLocalScale()
    {
        transform.localScale = Vector3.one;
    }
}
