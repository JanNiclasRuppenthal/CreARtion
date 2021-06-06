using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System.IO;

public class UI_Script : MonoBehaviour
{
    // private variables
    private int number = 1;


    // Video Button
    public Button buttonVideocapture;
    public Sprite stop_sprite;
    public Sprite record_sprite;
    private bool vflag = false;
    
    public Text infos;

    /*
    * Basic Idea is that you disable and enable
    * the mid air stage AND the positioner 
    * of the related object
    */

    // List of stages and positioners
    public GameObject cubeStagePositioner;
    public GameObject cylinderStagePositioner;
    public GameObject sphereStagePositioner;
    public GameObject capsuleStagePositioner;


    // Start is called before the first frame update
    void Start()
    {
        //path = GetAndroidExternalStoragePath();

        // enable cube stage and positioner as default
        cubeStagePositioner.SetActive(true);
        cylinderStagePositioner.SetActive(false);
        sphereStagePositioner.SetActive(false);
        capsuleStagePositioner.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //private string GetAndroidExternalStoragePath()
    //{
    //    if (Application.platform != RuntimePlatform.Android)
    //        return Application.persistentDataPath;

    //    var jc = new AndroidJavaClass("android.os.Environment");
    //    var path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory",
    //        jc.GetStatic<string>("DIRECTORY_DCIM"))
    //        .Call<string>("getAbsolutePath");
    //    return path;
    //}

    public void ButtonScreenshot_Click()
    {
        Debug.Log("Take a Screenshot");
        //string final_ScreenshotText = screenshotText + number + ".png";
        //number++;
        //ScreenCapture.CaptureScreenshot(final_ScreenshotText);
        //Debug.Log(path);
        infos.text = "Screenshot";
    }


    public void ButtonVideocapture_Click()
    {
        vflag = !vflag;
        if(vflag){
            Debug.Log("Capture a Video");
            infos.text = "Video";
            buttonVideocapture.GetComponentInChildren<Text>().text = "";
            buttonVideocapture.image.overrideSprite = record_sprite;
   	    }else{
            Debug.Log("Capturing stopped");
            infos.text = "";
            buttonVideocapture.GetComponentInChildren<Text>().text = "Rec";
            buttonVideocapture.image.overrideSprite = stop_sprite;
	    }
    }


    // Buttons in the scrollable list

    // In this method you enable cube stage and positioner and
    // disable all other stages and positioners
    public void ButtonCube_Click()
    {
        cubeStagePositioner.SetActive(true);
        cylinderStagePositioner.SetActive(false);
        sphereStagePositioner.SetActive(false);
        capsuleStagePositioner.SetActive(false);
    }

    // In this method you enable cylinder stage and positioner and
    // disable all other stages and positioners
    public void ButtonCylinder_Click()
    {
        cubeStagePositioner.SetActive(false);
        cylinderStagePositioner.SetActive(true);
        sphereStagePositioner.SetActive(false);
        capsuleStagePositioner.SetActive(false);
    }

    // In this method you enable sphere stage and positioner and
    // disable all other stages and positioners
    public void ButtonSphere_Click()
    {
        cubeStagePositioner.SetActive(false);
        cylinderStagePositioner.SetActive(false);
        sphereStagePositioner.SetActive(true);
        capsuleStagePositioner.SetActive(false);
    }

    // In this method you enable capsule stage and positioner and
    // disable all other stages and positioners
    public void ButtonCapusle_Click()
    {
        cubeStagePositioner.SetActive(false);
        cylinderStagePositioner.SetActive(false);
        sphereStagePositioner.SetActive(false);
        capsuleStagePositioner.SetActive(true);
    }

}
