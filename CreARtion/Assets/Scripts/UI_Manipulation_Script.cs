using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System.IO;
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



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Buttons in the scrollable list

    public void ButtonMove_Click()
    {
        
    }

    public void ButtonResize_Click()
    {
        
    }

    public void ButtonRotate_Click()
    {
        TextContainer.SetActive(false);
        uiRotation.SetActive(true);

        // TODO: one superclass
        circularX.Reset();
        circularY.Reset();
        circularZ.Reset();
    }

    public void ButtonStretch_Click()
    {
        
    }
}
