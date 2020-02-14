using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField]
    private float _seconds = 1f;
    void Start()
    {
        Destroy(gameObject, _seconds);
    }
}
