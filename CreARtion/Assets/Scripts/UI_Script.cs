using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UI_Script : MonoBehaviour
{
    private string screenshotText = "Screenshot";
    private int number = 1;
    private string path;

    //Video
    public Button mybutton;
    public Sprite stop_sprite;
    public Sprite record_sprite;
    private bool vflag = false;
    
    public Text infos;

    // Start is called before the first frame update
    void Start()
    {
        //path = GetAndroidExternalStoragePath();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
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
	      mybutton.image.overrideSprite = record_sprite;
   	}else{
	      Debug.Log("Capturing stopped");
	      infos.text = "";
	      mybutton.image.overrideSprite = stop_sprite;
	}
    }
}
