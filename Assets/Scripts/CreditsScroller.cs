using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private float ScrollStartY = -2.0f;
    [SerializeField] private float ScrollStartZ = -6.0f;
    [SerializeField] private float ScrollSpeed = 1.0f;
    [SerializeField] private float ScrollEnd = 1000.0f;

    [SerializeField] private RectTransform subject;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("RunCrawl");
    }

    public IEnumerator RunCrawl()
    {
        Vector3 pos = subject.localPosition;

        for ( float y = ScrollStartY, z = ScrollStartZ; y < ScrollEnd; y += ScrollSpeed * Time.deltaTime, z += ScrollSpeed * Time.deltaTime ) {
            pos.y = y;
            pos.z = z;
            
            subject.localPosition = pos;
            yield return null;
        }

        // TODO: Call on finish here
        yield return null;
    }
}
