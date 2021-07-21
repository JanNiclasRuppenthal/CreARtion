using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RGBSlider: MonoBehaviour
{
    public Text rvalue; 
    public Text gvalue; 
    public Text bvalue;
    public Text tvalue;
    public Slider red;
    public Slider green;
    public Slider blue;
    public Slider transparent;

    // arraylist of marked objects
    public SwitchMode sw;
    private ArrayList listOfMarkedObjects;

    private void Start()
    {
        listOfMarkedObjects = sw.getListOfMarkedObjects();
    }

    public void OnEdit()
    {
        // Set the counters
        rvalue.text = Mathf.RoundToInt(red.value * 255).ToString();
        gvalue.text = Mathf.RoundToInt(green.value * 255).ToString();
        bvalue.text = Mathf.RoundToInt(blue.value * 255).ToString();
        tvalue.text = Math.Round(transparent.value, 2).ToString();

        foreach (GameObject objects in listOfMarkedObjects)
        {
            Debug.Log(transparent.value);
            objects.GetComponent<Renderer>().material.SetColor("_Color", new Color(red.value, green.value, blue.value, transparent.value));
            
            float r = 1 - objects.GetComponent<MeshRenderer>().material.color.r;
            float g = 1 - objects.GetComponent<MeshRenderer>().material.color.g;
            float b = 1 - objects.GetComponent<MeshRenderer>().material.color.b;
            
            // set new outline
            var outline = objects.GetComponent<Outline>();
            
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = new Color(r, g, b, 1);
            outline.OutlineWidth = 7f;
        }
    }
}
