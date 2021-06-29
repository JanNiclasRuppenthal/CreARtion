using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System;
using Image = Vuforia.Image;

public class UI_Manipulation_Script : MonoBehaviour
{
    public GameObject uiManipulationmode;
    public GameObject TextContainer;
    public Text helpfulInformations;
    public GameObject scrollableListManipulations;


    //Rotation-UI
    public GameObject uiRotation;
    
    //Color-UI
    public GameObject uiColor;

    // childs of Rotation UI
    public CircularRangeControlX circularX;
    public CircularRangeControlY circularY;
    public CircularRangeControlZ circularZ;


    // arraylist of marked objects
    public SwitchMode sw;
    private ArrayList listOfMarkedObjects;

    float initialFingersDistance;
    Vector3 initialScale;
    private float initialScaleDir;

    // camera
    public Camera camera;

    public enum manipulationStates
    {
        None,
        Move,
        Resize,
        Rotate,
        Stretch,
        Color,
        Copy
    }

    public manipulationStates currentState;
    
    public enum scaleDir
    {
        ScaleX,
        ScaleY,
        ScaleZ
    }

    public scaleDir currentDir;
    
    // Icons
    public Button[] mIcons = new Button[6];
    

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
            case manipulationStates.Stretch:
                stretchObjects();
                break;
            default: break;
        }
    }

    
    // Highlight selected Icon
    public void highlightIcon()
    {
        resetIconHighlighting();
        
        // Select a Color
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);
        
        switch (currentState)
        {
            case manipulationStates.Move:
                mIcons[0].image.color = myColor;
                break;
            case manipulationStates.Resize:
                mIcons[1].image.color = myColor;
                break;
            case manipulationStates.Rotate:
                mIcons[2].image.color = myColor;
                break;
            case manipulationStates.Stretch:
                mIcons[3].image.color = myColor;
                break;
            case manipulationStates.Color:
                mIcons[4].image.color = myColor;
                break;
            case manipulationStates.Copy:
                mIcons[5].image.color = myColor;
                break;
            default: break;
        }
    }

    public void resetIconHighlighting()
    {
        foreach (Button i in mIcons)
        {
            i.image.color = Color.white;
        }
    }


    // Buttons in the scrollable list

    public void ButtonMove_Click()
    {
        // set current state of the UI
        currentState = manipulationStates.Move;

        // deactivate the rotation UI 
        uiRotation.SetActive(false);
        
        // deactivate the color UI 
        uiColor.SetActive(false);

        highlightIcon();
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
        
        // deactivate the color UI 
        uiColor.SetActive(false);

        currentState = manipulationStates.Resize; 
        
        highlightIcon();
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
        
        // deactivate the color UI 
        uiColor.SetActive(false);

        currentState = manipulationStates.Rotate;
        
        highlightIcon();

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
        
        // deactivate the color UI 
        uiColor.SetActive(false);

        currentState = manipulationStates.Stretch;
        
        highlightIcon();
    }

    public void stretchObjects()
    {
        int fingersOnScreen = 0;

        float xAngle;
        float yAngle;
        float zAngle;

        foreach (Touch touch in Input.touches)
        {
            fingersOnScreen++; //Count fingers (or rather touches) on screen as you iterate through all screen touches.

            //You need two fingers on screen to stretch.
            if (fingersOnScreen == 2)
            {
                foreach (GameObject objects in listOfMarkedObjects)
                {

                    //First set the initial distance between fingers so you can compare.
                    if (touch.phase == TouchPhase.Began)
                    {
                        initialFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        Vector2 direction = new Vector2(Input.touches[0].position.x, Input.touches[1].position.y);
                        
                        // Get Angle of direction to axis vectors
                        // x
                        xAngle = Vector2.Angle(direction, new Vector2(0, 1));
                        // y
                        yAngle = Vector2.Angle(direction, new Vector2(1, 0));
                        // z
                        zAngle = Vector2.Angle(direction, new Vector2(1, 1));

                        float min = Mathf.Min(Mathf.Min(xAngle, yAngle), zAngle);
                        
                        if (min == xAngle)
                        {
                            initialScaleDir = objects.transform.localScale.x;
                            currentDir = scaleDir.ScaleX;
                        } else if (min == yAngle) {
                            initialScaleDir = objects.transform.localScale.y;
                            currentDir = scaleDir.ScaleY;
                        } else {
                            initialScaleDir = objects.transform.localScale.z;
                            currentDir = scaleDir.ScaleZ;
                        }
                    }
                    else
                    {
                        var currentFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        var stretchFactor = currentFingersDistance / initialFingersDistance;

                        float x = 0;
                        float y = 0;
                        float z = 0;

                        switch (currentDir)
                        {
                            case scaleDir.ScaleX:
                                x = initialScaleDir * stretchFactor;
                                y = objects.transform.localScale.y;
                                z = objects.transform.localScale.z;
                                break;
                            case scaleDir.ScaleY:
                                y = initialScaleDir * stretchFactor;
                                x = objects.transform.localScale.x;
                                z = objects.transform.localScale.z;
                                break;
                            case scaleDir.ScaleZ:
                                z = initialScaleDir * stretchFactor;
                                x = objects.transform.localScale.x;
                                y = objects.transform.localScale.y;
                                break;
                        }
                        
                        objects.transform.localScale = new Vector3(x,y,z);
                    }
                }
            }
        }
    }
    
    public void Color_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        // deactivate the rotation UI 
        uiRotation.SetActive(false);
        
        // activate color UI
        uiColor.SetActive(true);

        currentState = manipulationStates.Color;
        
        highlightIcon();
    }
    
    public void Copy_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        // deactivate the rotation UI and Color UI
        uiRotation.SetActive(false);
        uiColor.SetActive(false);

        currentState = manipulationStates.Copy;
        
        highlightIcon();
    }
}
