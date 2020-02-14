using UnityEngine;

public class DoNotRotateWithParent : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
