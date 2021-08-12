using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DefaultObjectsParameters : MonoBehaviour
{
    //public GameObject baseObject;

    private bool marked = false;


    // Start is called before the first frame update
    void Start()
    {

        //baseObject.transform.position = new Vector3(0f, baseObject.transform.position.y);

        //this.GetComponent<Transform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //this.GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);
        //this.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(255.0f, 255.0f, 255.0f));
        //this.GetComponent<Renderer>().material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;

        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<Collider>().enabled = true;


    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public bool getMarked()
    {
        return marked;
    }

    public void setMarked(bool marked)
    {
        this.marked = marked;
    }
}
