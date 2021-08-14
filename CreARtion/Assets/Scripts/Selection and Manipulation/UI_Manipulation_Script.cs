using System.Collections;
using System.Collections.Generic;
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

    public GameObject[] clonedStages = new GameObject[6];
    

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
    private HashSet<GameObject> listOfMarkedObjects;

    // hashtable with the parents of the objects and their stages.
    private Dictionary<GameObject, Transform> objectsStages;

    float initialFingersDistance;
    Vector3 initialScale;
    private float initialScaleDir;

    // boolean variables control pad
    private bool up_y = false;
    private bool down_y = false;
    private bool right_x = false;
    private bool left_x = false;
    private bool up_z = false;
    private bool down_z = false;

    // control pad
    public GameObject controlPad;
    public GameObject switchMoveControl;
    private bool controlPadIsNotActive = false;
    public Button buttonSwitchMoveControl;
    public Sprite controlPad_sprite;
    public Sprite phone_sprite;

    // speed
    public float speed = 0.01f;

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
        // get the ArrayList
        listOfMarkedObjects = sw.getListOfMarkedObjects();

        // get the Hashtable
        objectsStages = sw.getDictionaryObjectStage();

        // Initial state
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
                if (!controlPadIsNotActive)
                {
                    moveObjects(); 
                }
                break;
            case manipulationStates.Resize:
                resizeObjects();
                break;
            case manipulationStates.Stretch:
                stretchObjects();
                break;
            default: break;
        }

        if (up_y)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.position = new Vector3(pos.x, pos.y + speed, pos.z);
            }
        }
        else if(down_y)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.position = new Vector3(pos.x, pos.y - speed, pos.z);
            }
        }
        else if (right_x)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.position = new Vector3(pos.x + speed, pos.y, pos.z);
            }
        }
        else if (left_x)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.position = new Vector3(pos.x - speed, pos.y, pos.z);
            }
        }
        else if (up_z)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.position = new Vector3(pos.x, pos.y, pos.z + speed);
            }
        }
        else if (down_z)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.position = new Vector3(pos.x, pos.y, pos.z - speed);
            }
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
        // if the user moved the objects around before
        removeObjectsFromCamera();

        currentState = manipulationStates.Select;

        highlightIcon();


        // deactivate the rotation UI 
        uiRotation.SetActive(false);

        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        switchMoveControl.SetActive(false);
        setControlPadBoolsOnFalse();

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

        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        buttonSwitchMoveControl.image.overrideSprite = controlPad_sprite;
        switchMoveControl.SetActive(true);
        setControlPadBoolsOnFalse();

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
            objects.transform.parent.parent = camera.transform;
        }
    }

    public void changeMoveControl()
    {
        if (controlPadIsNotActive)
        {
            // Disable controlPad
            controlPad.SetActive(false);
            TextContainer.SetActive(true);
            buttonSwitchMoveControl.image.overrideSprite = controlPad_sprite;
        }
        else
        {
            // Enable controlPad
            controlPad.SetActive(true);
            TextContainer.SetActive(false);

            // remove Objects from the camera
            removeObjectsFromCamera();

            buttonSwitchMoveControl.image.overrideSprite = phone_sprite;
        }
        
        controlPadIsNotActive = !controlPadIsNotActive;
    }

    public void removeObjectsFromCamera()
    {
        if (currentState != manipulationStates.Move)
        {
            return;
        }

        foreach (GameObject objects in listOfMarkedObjects)
        {
            //objects.transform.parent = null;
            objects.transform.parent.parent = objectsStages[objects];
        }
    }



    public void ButtonResize_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        // deactivate the rotation UI 
        uiRotation.SetActive(false);

        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        switchMoveControl.SetActive(false);
        setControlPadBoolsOnFalse();

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
                        initialScale = objects.transform.parent.localScale;
                    }
                    else
                    {
                        var currentFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        var scaleFactor = currentFingersDistance / initialFingersDistance;
                        objects.transform.parent.localScale = initialScale * scaleFactor;
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

        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        switchMoveControl.SetActive(false);
        setControlPadBoolsOnFalse();

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

        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        switchMoveControl.SetActive(false);
        setControlPadBoolsOnFalse();

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
                            initialScaleDir = objects.transform.parent.localScale.x;
                            currentDir = scaleDir.ScaleX;
                        } else if (min == yAngle || min == yAngleOpossite) 
                        {
                            initialScaleDir = objects.transform.parent.localScale.y;
                            currentDir = scaleDir.ScaleY;
                        } else if (min == zAngle || min == zAngleOpossite) 
                        {
                            initialScaleDir = objects.transform.parent.localScale.z;
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
                                y = objects.transform.parent.localScale.y;
                                z = objects.transform.parent.localScale.z;
                                break;
                            case scaleDir.ScaleY:
                                y = initialScaleDir * stretchFactor;
                                x = objects.transform.parent.localScale.x;
                                z = objects.transform.parent.localScale.z;
                                break;
                            case scaleDir.ScaleZ:
                                z = initialScaleDir * stretchFactor;
                                x = objects.transform.parent.localScale.x;
                                y = objects.transform.parent.localScale.y;
                                break;
                        }

                        objects.transform.parent.localScale = new Vector3(x, y, z);
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
        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        switchMoveControl.SetActive(false);
        setControlPadBoolsOnFalse();

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
        controlPad.SetActive(false);
        controlPadIsNotActive = false;
        switchMoveControl.SetActive(false);
        setControlPadBoolsOnFalse();

        
        currentState = manipulationStates.Copy;

        highlightIcon();

        ArrayList copyObjects = new ArrayList();
        foreach (GameObject objects in listOfMarkedObjects)
        {
            GameObject copied = null;
            if (objects.name.Contains("Cube"))
            {
                copied = Instantiate(clonedStages[0]);
            }
            else if (objects.name.Contains("Cylinder"))
            {
                copied = Instantiate(clonedStages[1]);
            }
            else if (objects.name.Contains("Sphere"))
            {
                copied = Instantiate(clonedStages[2]);
            }
            else if (objects.name.Contains("Capsule"))
            {
                copied = Instantiate(clonedStages[3]);
            }
            else if (objects.name.Contains("Pyramid"))
            {
                copied = Instantiate(clonedStages[4]);
            }
            else if (objects.name.Contains("Cone"))
            {
                copied = Instantiate(clonedStages[5]);
            }


            // copy position
            copied.transform.GetChild(0).position = new Vector3(objects.transform.parent.position.x, 
                                        objects.transform.parent.position.y, objects.transform.parent.position.z);

            // copy rotation
            Vector3 rotation = objects.transform.eulerAngles;
            copied.transform.GetChild(0).GetChild(0).transform.eulerAngles = rotation;


            // copy scale
            copied.transform.GetChild(0).transform.localScale = new Vector3(objects.transform.parent.localScale.x, objects.transform.parent.localScale.y, objects.transform.parent.localScale.z);

            // copy colour
            float r = objects.GetComponent<MeshRenderer>().material.color.r;
            float g = objects.GetComponent<MeshRenderer>().material.color.g;
            float b = objects.GetComponent<MeshRenderer>().material.color.b;
            float a = objects.GetComponent<MeshRenderer>().material.color.a;

            copied.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b, a));


            // get the outline
            var outline = copied.transform.GetChild(0).GetChild(0).GetComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = new Color(1-r, 1-g, 1-b, 1);
            outline.OutlineWidth = 7f;


            copyObjects.Add(copied.transform.GetChild(0).GetChild(0).gameObject);

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
        objectsStages.Clear();

        foreach (GameObject copies in copyObjects)
        {
            listOfMarkedObjects.Add(copies);
            objectsStages.Add(copies, copies.transform.parent.parent);
            copies.transform.parent.parent = camera.transform;
        }
    

        ButtonMove_Click();  
    }

    public void setControlPadBoolsOnFalse()
    {
        up_y = false;
        down_y = false;
        right_x = false;
        left_x = false;
        up_z = false;
        down_z = false;
    }


    // TODO: Can be deleted, since we merged the moving controls
    public void Buttontest_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();

        // deactivate the rotation UI and Color UI
        uiRotation.SetActive(false);
        uiColor.SetActive(false);

        controlPad.SetActive(true);
        switchMoveControl.SetActive(true);
    }

    public void ButtonUP_Click(bool up)
    {
        up_y = up;
        
    }

    public void ButtonDOWN_Click(bool down)
    {
        down_y = down;
    }

    public void ButtonRIGHT_Click(bool right)
    {
        right_x = right;
    }

    public void ButtonLEFT_Click(bool left)
    {
        left_x = left;
    }

    public void ButtonUPZ_Click(bool up)
    {
        up_z = up;
    }

    public void ButtonDOWNZ_Click(bool down)
    {
        down_z = down;
    }
}
