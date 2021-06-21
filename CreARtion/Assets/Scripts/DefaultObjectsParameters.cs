using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultObjectsParameters : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // position?

        this.GetComponent<Transform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
        this.GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);

	    // TODO: if you set the color like this, the object looses his shadow
        //this.GetComponent<MeshRenderer>().material.color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
