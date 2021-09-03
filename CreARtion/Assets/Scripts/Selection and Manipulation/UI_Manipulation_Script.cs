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

    public GameObject[] clonedStages = new GameObject[10];

    // ArrayList of all UI GameObjects
    private ArrayList listUI = new ArrayList();


    //Rotation-UI
    public GameObject uiRotation;
    
    //Color-UI
    public GameObject uiColor;

    // childs of Rotation UI
    public CircularRangeControlX circularX;
    public CircularRangeControlY circularY;
    public CircularRangeControlZ circularZ;

    // Move-UI and bool variable
    public GameObject moveUI;
    private bool movement;


    // arraylist of marked objects
    public SwitchMode sw;
    private HashSet<GameObject> listOfMarkedObjects;

    // hashtable with the parents of the objects and their stages.
    private Dictionary<GameObject, Transform> objectsStages;

    // Variables for stretching
    float initialFingersDistance;
    Vector3 initialScale;
    private float initialScaleDir;
    public GameObject uiListStretchButtons;
    public Button[] stretchButtons = new Button[3];
    private bool stretch_x = true;
    private bool stretch_y = false;
    private bool stretch_z = false;

    // boolean variables control pad
    private bool up_y = false;
    private bool down_y = false;
    private bool right_x = false;
    private bool left_x = false;
    private bool up_z = false;
    private bool down_z = false;
    private bool rotate_right = false;
    private bool rotate_left = false;

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
        // add all UI elemtents
        listUI.AddRange(new List<GameObject>{
            uiRotation,
            controlPad,
            switchMoveControl,
            moveUI,
            uiColor,
            uiListStretchButtons,
            TextContainer
        });

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
        // TODO: better writing
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
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y + speed, pos.z);
            }
        }
        if(down_y)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y - speed, pos.z);
            }
        }
        if (right_x)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x + speed, pos.y, pos.z);
            }
        }
        if (left_x)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x - speed, pos.y, pos.z);
            }
        }
        if (up_z)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y, pos.z + speed);
            }
        }
        if (down_z)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y, pos.z - speed);
            }
        }
        if (rotate_right)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 v = objects.transform.eulerAngles;
                objects.transform.eulerAngles = new Vector3(v.x, v.y + speed*100, v.z);
            }
        }
        if (rotate_left)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 v = objects.transform.eulerAngles;
                objects.transform.eulerAngles = new Vector3(v.x, v.y - speed*100 , v.z);
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


    private void activateGameObjects(GameObject[] array)
    {
        bool skip = false;
        foreach(GameObject ui in listUI)
        {
            for(int i = 0; i < array.Length; i++)
            {
                // found the right UI element?
                if (ui == array[i])
                {
                    skip = true;
                    break;
                }
            }

            // activate, if we found the right element
            if (skip)
            {
                ui.SetActive(true);
                skip = false;
                continue;
            }

            // deactivate the unnecessary UI elements
            ui.SetActive(false);
        }
    }


    // Buttons in the scrollable list

    public void ButtonSelect_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        currentState = manipulationStates.Select;

        highlightIcon();

        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        activateGameObjects(new GameObject[] {TextContainer});
        helpfulInformations.text = "Tap on the objects to select them.";
    }

    public void ButtonMove_Click()
    {
        // set current state of the UI
        currentState = manipulationStates.Move;

        controlPadIsNotActive = false;
        buttonSwitchMoveControl.image.overrideSprite = controlPad_sprite;
        setControlPadBoolsOnFalse();

        highlightIcon();

        activateGameObjects(new GameObject[] { switchMoveControl, moveUI, TextContainer });
        helpfulInformations.text = "Hold the button down and move your device to move your selected objects.";
    }

 

    private void moveObjects()
    {

        if (!movement)
        {
            removeObjectsFromCamera();
            return;
        }
        
        foreach (GameObject objects in listOfMarkedObjects)
        {
            // put the object as a child to camera
            objects.transform.parent.parent = camera.transform;
        }
    }

    public void holdMoveButton(bool move)
    {
        movement = move;
    }

    public void changeMoveControl()
    {
        if (controlPadIsNotActive)
        {
            // Disable controlPad
            controlPad.SetActive(false);
            moveUI.SetActive(true);
            TextContainer.SetActive(true);
            buttonSwitchMoveControl.image.overrideSprite = controlPad_sprite;
        }
        else
        {
            // Enable controlPad
            controlPad.SetActive(true);
            moveUI.SetActive(false);
            TextContainer.SetActive(false);

            // if the user moved the objects around before
            removeObjectsFromCamera();
            movement = false;

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
            objects.transform.parent.parent = objectsStages[objects];
        }
    }



    public void ButtonResize_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();;

        currentState = manipulationStates.Resize; 
        
        highlightIcon();

        activateGameObjects(new GameObject[] {TextContainer});
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
        movement = false;

        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Rotate;
        
        highlightIcon();

        activateGameObjects(new GameObject[] {uiRotation});

        circularX.Reset();
        circularY.Reset();
        circularZ.Reset();
    }


    public void ButtonStretch_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Stretch;
        
        highlightIcon();

        StretchX_Click();

        activateGameObjects(new GameObject[] {uiListStretchButtons});

    }


   
    public void stretchObjects()
    {
        int fingersOnScreen = 0;

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

                        if (stretch_x)
                        {
                            initialScaleDir = objects.transform.parent.localScale.x;
                            currentDir = scaleDir.ScaleX;
                        } 
                        else if (stretch_y) 
                        {
                            initialScaleDir = objects.transform.parent.localScale.y;
                            currentDir = scaleDir.ScaleY;
                        } 
                        else if (stretch_z) 
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

    // methods for the buttons
    public void StretchX_Click()
    {
        stretch_x = true;
        stretch_y = false;
        stretch_z = false;

        // change colour of the button
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        stretchButtons[0].image.color = myColor;
        stretchButtons[1].image.color = Color.white;
        stretchButtons[2].image.color = Color.white;
    }

    public void StretchY_Click()
    {
        stretch_x = false;
        stretch_y = true;
        stretch_z = false;

        // change colour of the button
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        stretchButtons[0].image.color = Color.white;
        stretchButtons[1].image.color = myColor;
        stretchButtons[2].image.color = Color.white;
    }

    public void StretchZ_Click()
    {
        stretch_x = false;
        stretch_y = false;
        stretch_z = true;

        // change colour of the button
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        stretchButtons[0].image.color = Color.white;
        stretchButtons[1].image.color = Color.white;
        stretchButtons[2].image.color = myColor;
    }

    public void Color_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Color;
        
        highlightIcon();


        activateGameObjects(new GameObject[] {uiColor});
    }

    
    public void Copy_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        
        currentState = manipulationStates.Copy;

        highlightIcon();

        activateGameObjects(new GameObject[] {null});

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
            else if (objects.name.Contains("Hemisphere"))
            {
                copied = Instantiate(clonedStages[6]);
            }
            else if (objects.name.Contains("Tube"))
            {
                copied = Instantiate(clonedStages[7]);
            }
            else if (objects.name.Contains("Ring"))
            {
                copied = Instantiate(clonedStages[8]);
            }
            else if (objects.name.Contains("Prism"))
            {
                copied = Instantiate(clonedStages[9]);
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

    public void ButtonRightROTATE_Click(bool right)
    {
        rotate_right = right;
    }

    public void ButtonLeftROTATE_Click(bool left)
    {
        rotate_left = left;
    }
}
