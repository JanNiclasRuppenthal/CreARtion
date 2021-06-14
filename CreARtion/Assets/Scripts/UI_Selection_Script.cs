using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System.IO;
using System;

public class UI_Selection_Script : MonoBehaviour
{
    // variables for screenshots
    private int number = 1;
    private int screenshotTimer = 0;
    private int videocapturingTimerSeconds = 0;
    private int videocapturingTimerMinutes = 0;


    // Video Button
    public Button buttonVideocapture;
    public Sprite stop_sprite;
    public Sprite record_sprite;
    private bool vflag = false;

    // Switch Interaction mode
    private bool selectMode = true;

    public Text helpfulInformations;

    /*
    * Basic Idea is that you disable and enable
    * the mid air stage AND the positioner 
    * of the related object
    */

    // List of stages and positioners
    public int size = 6;
    public GameObject[] stageAndPositioners = new GameObject[6];

    // List of Buttons and Lists of different modes
    // Selection mode
    public GameObject scrollableListForms;

    // Manipulation mode
    public GameObject deleteButton;
    public GameObject XButton;
    public GameObject scrollableListManipulation;


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



    // Start is called before the first frame update
    void Start()
    {

        // enable cube stage and positioner as default
        setStageAndPositioner(0);

	    //Select Select mode at beginning
	    scrollableListForms.SetActive(true);
	    deleteButton.SetActive(false);
	    XButton.SetActive(false);
	    scrollableListManipulation.SetActive(false);

        enumObjects = Objects.Cube;
    }


    public void Update()
    {
        
    }

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
        helpfulInformations.text = "Screenshot taken in " + (3 - screenshotTimer);
        Invoke("takeScreenshot", 1);
    }

    private void takeScreenshot()
    {
        screenshotTimer++;

        if (screenshotTimer <= 3)
        {
            helpfulInformations.text = "Screenshot taken in " + (3 - screenshotTimer);
            Invoke("takeScreenshot", 1);

        }
        else
        {
            string final_ScreenshotText = "CreARtion " + DateTime.Now.ToString("MM/dd/yyyy") + " " + number + ".png";
            number++;
            ScreenCapture.CaptureScreenshot(final_ScreenshotText);

            helpfulInformations.text = "Screenshot was taken.";

            screenshotTimer = 0;

            Invoke("changeInformationOnText", 1);
        }
    }


    public void ButtonVideocapture_Click()
    {
        vflag = !vflag;
        if(vflag){
            // video is stopped
            buttonVideocapture.image.overrideSprite = record_sprite;

            helpfulInformations.text = "Recording: 00:00";

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
            helpfulInformations.text = "Recording is stopped.";

            // default values
            videocapturingTimerMinutes = 0;
            videocapturingTimerSeconds = 0;

            Invoke("changeInformationOnText", 1);
            return;
        }

        videocapturingTimerSeconds++;

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
    }

    // In this method you enable cylinder stage and positioner and
    // disable all other stages and positioners
    public void ButtonCylinder_Click()
    {
        setStageAndPositioner(1);

        enumObjects = Objects.Cylinder;
        changeInformationOnText();
    }

    // In this method you enable sphere stage and positioner and
    // disable all other stages and positioners
    public void ButtonSphere_Click()
    {
        setStageAndPositioner(2);

        enumObjects = Objects.Sphere;
        changeInformationOnText();
    }

    // In this method you enable capsule stage and positioner and
    // disable all other stages and positioners
    public void ButtonCapusle_Click()
    {
        setStageAndPositioner(3);

        enumObjects = Objects.Capsule;
        changeInformationOnText();
    }

    // In this method you enable pyramid stage and positioner and
    // disable all other stages and positioners
    public void ButtonPyramid_Click()
    {
        setStageAndPositioner(4);

        enumObjects = Objects.Pyramid;
        changeInformationOnText();
    }

    // In this method you enable cone stage and positioner and
    // disable all other stages and positioners
    public void ButtonCone_Click()
    {
        setStageAndPositioner(5);

        enumObjects = Objects.Cone;
        changeInformationOnText();
    }

    // this method change the text of information
    // this method will be called in each click() method
    private void changeInformationOnText()
    {
        helpfulInformations.text = "Tap to place " + enumObjects.ToString() + ".\nOr tap on object to manipulate.";
    }
}
