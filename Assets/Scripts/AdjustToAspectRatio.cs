using UnityEngine;

public class AdjustToAspectRatio : MonoBehaviour
{
    [SerializeField] private float _16By9CameraSize;
    [SerializeField] private float _4By3CameraSize;

    private const float RATIO_16_9 = 16f / 9f;
    private const float RATIO_4_3 = 4f / 3f;

    private float _aspectRatio;
    private Camera _camera;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
        _aspectRatio = (float) Screen.width / Screen.height;
        var t = (_aspectRatio - RATIO_4_3) / (RATIO_16_9 - RATIO_4_3);
        var size = Mathf.Lerp(_4By3CameraSize, _16By9CameraSize, t);
        _camera.orthographicSize = size;
        
        Debug.LogFormat("Setting camera size to {0}", size);
    }
}