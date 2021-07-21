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
        Select,
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
        currentState = manipulationStates.Select;

        // highlight icon after switching the mode
        highlightIcon();

        TextContainer.SetActive(true);
        helpfulInformations.text = "Tap on the objects to select them.";

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
            case manipulationStates.Select:
                mIcons[0].image.color = myColor;
                break;
            case manipulationStates.Move:
                mIcons[1].image.color = myColor;
                break;
            case manipulationStates.Resize:
                mIcons[2].image.color = myColor;
                break;
            case manipulationStates.Rotate:
                mIcons[3].image.color = myColor;
                break;
            case manipulationStates.Stretch:
                mIcons[4].image.color = myColor;
                break;
            case manipulationStates.Color:
                mIcons[5].image.color = myColor;
                break;
            case manipulationStates.Copy:
                mIcons[6].image.color = myColor;
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

    public void ButtonSelect_Click()
    {
        currentState = manipulationStates.Select;

        highlightIcon();

        // deactivate the rotation UI 
        uiRotation.SetActive(false);

        // deactivate the color UI 
        uiColor.SetActive(false);

        TextContainer.SetActive(true);
        helpfulInformations.text = "Tap on the objects to select them.";
    }

    public void ButtonMove_Click()
    {
        // set current state of the UI
        currentState = manipulationStates.Move;

        // deactivate the rotation UI 
        uiRotation.SetActive(false);
        
        // deactivate the color UI 
        uiColor.SetActive(false);

        highlightIcon();

        TextContainer.SetActive(true);
        helpfulInformations.text = "Move your device to move your selected objects.";
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

        TextContainer.SetActive(true);
        helpfulInformations.text = "Use the two finger gesture to resize the selected objects.";
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

        TextContainer.SetActive(true);
        helpfulInformations.text = "Use the two finger gesture along the x-, y- or z-axis to stretch the selected objects.";

    }


   
    public void stretchObjects()
    {
        int fingersOnScreen = 0;

        float xAngle;
        float yAngle;
        float zAngle;
        float zAngle2;
        float xAngleOpossite;
        float yAngleOpossite;
        float zAngleOpossite;

        TextContainer.SetActive(true);

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
                        Vector2 direction = Input.touches[1].position - Input.touches[0].position;
                        
                        // Get Angle of direction to axis vectors
                        // x
                        xAngle = Vector2.Angle(direction, new Vector2(1, 0));
                        // y
                        yAngle = Vector2.Angle(direction, new Vector2(0, 1));
                        // z
                        zAngle = Vector2.Angle(direction, new Vector2(1, 1));
                        zAngle2 = Vector2.Angle(direction, new Vector2(1, -1));

                        xAngleOpossite = Vector2.Angle(direction, new Vector2(-1, 0));
                        // y
                        yAngleOpossite = Vector2.Angle(direction, new Vector2(0, -1));
                        // z
                        zAngleOpossite = Vector2.Angle(direction, new Vector2(-1, 1));

                        float min = Mathf.Min(Mathf.Min(Mathf.Min(xAngle, yAngle), zAngle), zAngle2);
                        min = Mathf.Min(min, Mathf.Min(Mathf.Min(xAngleOpossite, yAngleOpossite), zAngleOpossite));


                        if (min == xAngle || min == xAngleOpossite)
                        {
                            initialScaleDir = objects.transform.localScale.x;
                            currentDir = scaleDir.ScaleX;
                        } else if (min == yAngle || min == yAngleOpossite) 
                        {
                            initialScaleDir = objects.transform.localScale.y;
                            currentDir = scaleDir.ScaleY;
                        } else if (min == zAngle || min == zAngleOpossite) 
                        {
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

        TextContainer.SetActive(false);
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

        ArrayList copyObjects = new ArrayList();

        foreach (GameObject objects in listOfMarkedObjects)
        {

            GameObject copied = Instantiate(objects, camera.transform);

            // copy position
            copied.transform.position = new Vector3(objects.transform.position.x, objects.transform.position.y, 0.5f);

            // copy rotation
            Vector3 rotation = objects.transform.eulerAngles;
            copied.transform.eulerAngles = rotation;

            Debug.Log(rotation);
            Debug.Log(copied.transform.eulerAngles);

            // copy scale
            copied.transform.localScale = new Vector3(objects.transform.localScale.x, objects.transform.localScale.y, objects.transform.localScale.z);

            // copy colour
            float r = objects.GetComponent<MeshRenderer>().material.color.r;
            float g = objects.GetComponent<MeshRenderer>().material.color.g;
            float b = objects.GetComponent<MeshRenderer>().material.color.b;
            float a = objects.GetComponent<MeshRenderer>().material.color.a;

            copied.GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b, a));


            copyObjects.Add(copied);

        }

        // there is no outline after you enter the selectionmode
        foreach (GameObject item in listOfMarkedObjects)
        {
            var outline = item.GetComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineHidden;
            outline.OutlineColor = new Color(0, 0, 0, 0);
            outline.OutlineWidth = 0f;

        }

        listOfMarkedObjects.Clear();

        foreach (GameObject copies in copyObjects)
        {    
            listOfMarkedObjects.Add(copies);
        }

        ButtonMove_Click();

    }
}
