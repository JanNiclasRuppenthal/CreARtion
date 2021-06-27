using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System;

public class UI_Manipulation_Script : MonoBehaviour
{
    public GameObject uiManipulationmode;
    public GameObject TextContainer;
    public Text helpfulInformations;
    public GameObject scrollableListManipulations;


    //Rotation-UI
    public GameObject uiRotation;

    // childs of Rotation UI
    public CircularRangeControlX circularX;
    public CircularRangeControlY circularY;
    public CircularRangeControlZ circularZ;


    // arraylist of marked objects
    public SwitchMode sw;
    private ArrayList listOfMarkedObjects;

    float initialFingersDistance;
    Vector3 initialScale;


    // camera
    public Camera camera;

    public enum manipulationStates
    {
        None,
        Move,
        Resize,
        Rotate,
        Stretch
    }

    public manipulationStates currentState;



    // Start is called before the first frame update
    void Start()
    {
        listOfMarkedObjects = sw.getListOfMarkedObjects();
        currentState = manipulationStates.None;
       
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case manipulationStates.Move: 
                moveObjects(); 
                break;
            case manipulationStates.Resize:
                resizeObjects();
                break;
            default: break;
        }
    }



    // Buttons in the scrollable list

    public void ButtonMove_Click()
    {
        // set current state of the UI
        currentState = manipulationStates.Move;

        // deactivate the rotation UI 
        uiRotation.SetActive(false);
    }

 

    private void moveObjects()
    {
        
        foreach (GameObject objects in listOfMarkedObjects)
        {
            // put the object as a child to camera
            objects.transform.parent = camera.transform;
        }
    }

    public void removeObjectsFromCamera()
    {
        if (currentState != manipulationStates.Move)
        {
            return;
        }

        foreach (GameObject objects in listOfMarkedObjects)
        {
            objects.transform.parent = null;
        }
    }



    public void ButtonResize_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        // deactivate the rotation UI 
        uiRotation.SetActive(false);

        currentState = manipulationStates.Resize; 
    }

    private void resizeObjects()
    {

        int fingersOnScreen = 0;

        foreach (Touch touch in Input.touches)
        {
            fingersOnScreen++; //Count fingers (or rather touches) on screen as you iterate through all screen touches.

            //You need two fingers on screen to pinch.
            if (fingersOnScreen == 2)
            {
                foreach (GameObject objects in listOfMarkedObjects)
                {

                    //First set the initial distance between fingers so you can compare.
                    if (touch.phase == TouchPhase.Began)
                    {
                        initialFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        initialScale = objects.transform.localScale;
                    }
                    else
                    {
                        var currentFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        var scaleFactor = currentFingersDistance / initialFingersDistance;
                        objects.transform.localScale = initialScale * scaleFactor;
                    }
                }
            }
        }
    }

    public void ButtonRotate_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        currentState = manipulationStates.Rotate;

        TextContainer.SetActive(false);
        uiRotation.SetActive(true);

        // TODO: one superclass
        circularX.Reset();
        circularY.Reset();
        circularZ.Reset();
    }

    public void ButtonStretch_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        // deactivate the rotation UI 
        uiRotation.SetActive(false);

        currentState = manipulationStates.Stretch;
    }
   
}
