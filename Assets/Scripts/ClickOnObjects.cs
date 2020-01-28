using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if(hit.transform.tag == "PLANET") {
                Debug.Log("JUST HIT A PLANET");
            }
        }
    }
}
