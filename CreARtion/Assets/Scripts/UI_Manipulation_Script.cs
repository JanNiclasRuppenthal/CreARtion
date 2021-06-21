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
    }

    public void ButtonStretch_Click()
    {
        
    }
}
