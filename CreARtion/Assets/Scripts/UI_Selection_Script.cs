using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System.IO;
using System;


public class UI_Selection_Script : MonoBehaviour
{

    // two different uis
    public GameObject uiSelectionmode;
    public GameObject uiManipulationmode;

    // variables for screenshots
    private int number;
    private int screenshotTimer = 0;
    private int videocapturingTimerSeconds = 0;
    private int videocapturingTimerMinutes = 0;


    // Video Button
    public Button buttonVideocapture;
    public Sprite stop_sprite;
    public Sprite record_sprite;
    private bool vflag = false;



    public Text helpfulInformations;

    /*
    * Basic Idea is that you disable and enable
    * the mid air stage AND the positioner 
    * of the related object
    */

    // List of stages and positioners
    public int size = 6;
    public GameObject[] stageAndPositioners = new GameObject[6];
    public AnchorBehaviour[] stages = new AnchorBehaviour[6];
    public ContentPositioningBehaviour[] positioner = new ContentPositioningBehaviour[6];

    // List of Buttons
    public Button[] formIcons = new Button[6];


    // enumeration of all modes
    private enum Objects
    {
        Cube,
        Cylinder,
        Sphere,
        Capsule,
        Pyramid,
        Cone
    }

    private Objects enumObjects;


    private void Update()
    {

        // Touching the Objects
        if (Input.GetMouseButton(0))
        {
            setStage();
        }
        else if((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            setStage();
        }
    }

    private void setStage()
    {
       
        switch (enumObjects)
        {
            case Objects.Cube:
                positioner[0].AnchorStage = stages[0];
                break;
            case Objects.Cylinder:
                positioner[1].AnchorStage = stages[1];
                break;
            case Objects.Sphere:
                positioner[2].AnchorStage = stages[2];
                break;
            case Objects.Capsule:
                positioner[3].AnchorStage = stages[3];
                break;
            case Objects.Pyramid:
                positioner[4].AnchorStage = stages[4];
                break;
            case Objects.Cone:
                positioner[5].AnchorStage = stages[5];
                break;

            default: break;
        }
    }
    
    // Highlight selected Icon
    public void highlightIcon()
    {
        resetIconHighlighting();
        
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);
        
        switch (enumObjects)
        {
            case Objects.Cube:
                formIcons[0].image.color = myColor;
                break;
            case Objects.Cylinder:
                formIcons[1].image.color = myColor;
                break;
            case Objects.Sphere:
                formIcons[2].image.color = myColor;
                break;
            case Objects.Capsule:
                formIcons[3].image.color = myColor;
                break;
            case Objects.Pyramid:
                formIcons[4].image.color = myColor;
                break;
            case Objects.Cone:
                formIcons[5].image.color = myColor;
                break;
            default: break;
        }
    }

    public void resetIconHighlighting()
    {
        foreach (Button i in formIcons)
        {
            i.image.color = Color.white;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        // enable cube stage and positioner as default
        setStageAndPositioner(0);

        //Select Select mode at beginning
        uiSelectionmode.SetActive(true);
        // and disable ui of manipulationmode
        uiManipulationmode.SetActive(false);

        enumObjects = Objects.Cube;

        // save the number of shots
        if (!PlayerPrefs.HasKey("numberOfShots"))
        {
            PlayerPrefs.SetInt("numberOfShots", 0);
        }
        
        // Highlight Cube Icon
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);
        formIcons[0].image.color = myColor;
    }

    // set one stage and positioner as active
    // and disable all other stages and positioners
    private void setStageAndPositioner(int index)
    {
        for (int i = 0; i < stageAndPositioners.Length; i++)
        {
            if (i == index )
            {
                stageAndPositioners[i].SetActive(true);
            }
            else
            {
                stageAndPositioners[i].SetActive(false);
            }
        }
    }


    public void ButtonScreenshot_Click()
    {
        // center the position of the text
        //helpfulInformations.transform.position = new Vector2(672f, 31.0f);

        // new text
        helpfulInformations.text = "Screenshot taken in " + (3 - screenshotTimer);

        // call function after 1 second
        Invoke("takeScreenshot", 1);
    }

    private void takeScreenshot()
    {
        // one second later
        screenshotTimer++;

        // after 3 seconds, it should take a screenshot
        if (screenshotTimer <= 3)
        {
            // new text
            helpfulInformations.text = "Screenshot taken in " + (3 - screenshotTimer);

            // call function after 1 second
            Invoke("takeScreenshot", 1);

        }
        else
        {
            // number of screenshots should always be three digits
            string numberUnderTenOrHundred = "";
            numberUnderTenOrHundred = (number < 100 && number > 10 ? "0" : "00");

            // file name
            string final_ScreenshotText = "CreARtion " +  " Screenshot " + numberUnderTenOrHundred + PlayerPrefs.GetInt("numberOfShots") + ".png";

            // save the new number of shots
            PlayerPrefs.SetInt("numberOfShots", ++number);

            // disable the uiSelectionmode
            // capture a screenshot
            // enable the uiSelectionmode
            uiSelectionmode.SetActive(false);
            ScreenCapture.CaptureScreenshot(final_ScreenshotText);
            Invoke("enableSelectionmode", 0.1f);

            // new text
            helpfulInformations.text = "Screenshot was taken.";

            // reset variable
            screenshotTimer = 0;

            // call function after 1 second
            Invoke("changeInformationOnText", 1);
        }
    }

    // enable the selectionmode ui
    private void enableSelectionmode()
    {
        uiSelectionmode.SetActive(true);
    }


    public void ButtonVideocapture_Click()
    {
        vflag = !vflag;
        if(vflag){
            // video is stopped
            buttonVideocapture.image.overrideSprite = record_sprite;

            // set a new text for taking a screenshot
            helpfulInformations.text = "Recording: 00:00";
            // center the text
            //helpfulInformations.transform.position = new Vector2(697.7f, 31.0f);


            // call function after 1 second
            Invoke("videoTimer", 1);

        }
        else
        {
            // video is capturing
            buttonVideocapture.image.overrideSprite = stop_sprite;
        }
    }

    private void videoTimer()
    {
        // video is stopped
        if (!vflag)
        {
            //helpfulInformations.transform.position = new Vector2(675f, 31.0f);
            helpfulInformations.text = "Recording is stopped.";

            // default values
            videocapturingTimerMinutes = 0;
            videocapturingTimerSeconds = 0;

            Invoke("changeInformationOnText", 1);
            return;
        }

        videocapturingTimerSeconds++;

        // show the right time
        if (videocapturingTimerMinutes < 10)
        {

            if (videocapturingTimerSeconds < 10)
            {
                helpfulInformations.text = "Recording: 0" + videocapturingTimerMinutes + ":0" + videocapturingTimerSeconds;
            }
            else if (videocapturingTimerSeconds < 60)
            {
                helpfulInformations.text = "Recording: 0" + videocapturingTimerMinutes + ":" + videocapturingTimerSeconds;
            }
            else
            {
                videocapturingTimerMinutes++;
                videocapturingTimerSeconds = 0;
            }
        }
        else
        {
            if (videocapturingTimerSeconds < 10)
            {
                helpfulInformations.text = "Recording: " + videocapturingTimerMinutes + ":0" + videocapturingTimerSeconds;
            }
            else if (videocapturingTimerSeconds < 60)
            {
                helpfulInformations.text = "Recording: " + videocapturingTimerMinutes + ":" + videocapturingTimerSeconds;
            }
            else
            {
                videocapturingTimerMinutes++;
                videocapturingTimerSeconds = 0;
            }
        }

        // call function after 1 second
        Invoke("videoTimer", 1);
    }


    // Buttons in the scrollable list

    // In this method you enable cube stage and positioner and
    // disable all other stages and positioners
    public void ButtonCube_Click()
    {
        setStageAndPositioner(0);

        enumObjects = Objects.Cube;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable cylinder stage and positioner and
    // disable all other stages and positioners
    public void ButtonCylinder_Click()
    {
        setStageAndPositioner(1);

        enumObjects = Objects.Cylinder;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable sphere stage and positioner and
    // disable all other stages and positioners
    public void ButtonSphere_Click()
    {
        setStageAndPositioner(2);

        enumObjects = Objects.Sphere;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable capsule stage and positioner and
    // disable all other stages and positioners
    public void ButtonCapusle_Click()
    {
        setStageAndPositioner(3);

        enumObjects = Objects.Capsule;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable pyramid stage and positioner and
    // disable all other stages and positioners
    public void ButtonPyramid_Click()
    {
        setStageAndPositioner(4);

        enumObjects = Objects.Pyramid;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable cone stage and positioner and
    // disable all other stages and positioners
    public void ButtonCone_Click()
    {
        setStageAndPositioner(5);

        enumObjects = Objects.Cone;
        changeInformationOnText();
        
        highlightIcon();
    }

    // this method change the text of information
    // this method will be called in each click() method
    private void changeInformationOnText()
    {
        // set the old position
        //helpfulInformations.transform.position = new Vector2(640.5f, 44.1f);

        // change the text to the information how to place objects
        helpfulInformations.text = "Tap to place " + enumObjects.ToString() + ".\nOr tap on object to manipulate.";
    }
}