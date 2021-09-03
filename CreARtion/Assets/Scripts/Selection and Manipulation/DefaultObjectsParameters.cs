using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DefaultObjectsParameters : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        // copied objects appear on the screen
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<Collider>().enabled = true;
    }

}
