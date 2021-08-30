using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class AnimatorController : MonoBehaviour
{
    public GameObject modelledObject;
    //public VirtualButtonBehaviour vb;
    public GameObject camera;
    public float minimalDistance;

    private Animator anim;
    //private bool rewind = false;
    private bool details = false;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        //vb.RegisterOnButtonPressed(onButtonPressed);

        anim = modelledObject.GetComponent<Animator>();
    
    }

    public void Update()
    {
        distance = Vector3.Distance(camera.transform.position, modelledObject.transform.position);
        //Debug.Log("Distance: " + distance);

        if (!details && distance < minimalDistance)
        {
            anim.Play("Base Layer.Forwards");
            details = true;
        }
        else if (distance > minimalDistance)
        {
            anim.Play("Base Layer.Rewind");
            details = false;
        }
    }

    //public void onButtonPressed(VirtualButtonBehaviour vb)
    //{
    //    if(rewind)
    //    {
            
    //        anim.Play("Base Layer.Rewind");
    //        rewind = !rewind;
            
    //    }
    //    else
    //    {
    //        anim.Play("Base Layer.Forwards");
    //        rewind = !rewind;
    //    } 
    //}

}
