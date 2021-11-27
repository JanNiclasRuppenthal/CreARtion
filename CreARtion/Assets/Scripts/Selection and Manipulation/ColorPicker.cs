using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPicker : MonoBehaviour
{
    // the texts near the slider
    //public Text rvalue;
    //public Text gvalue;
    //public Text bvalue;

    // the slider
    //public Slider red;
    //public Slider green;
    //public Slider blue;

    // get the HashSet of marked objects
    public SwitchMode sw;
    private HashSet<GameObject> listOfMarkedObjects;

    public FlexibleColorPicker colorPicker;
       
    private void Start()
    {
        listOfMarkedObjects = sw.getListOfMarkedObjects();
        
    }

    private void Update()
    {
        Debug.Log("Außerhalb der Schleife");
        // Set the counters
        //rvalue.text = Mathf.RoundToInt(red.value * 255).ToString();
        //gvalue.text = Mathf.RoundToInt(green.value * 255).ToString();
        //bvalue.text = Mathf.RoundToInt(blue.value * 255).ToString();

        foreach (GameObject objects in listOfMarkedObjects)
        {
            Debug.Log("Hier in der Schleife");

            objects.GetComponent<Renderer>().material.color = colorPicker.color;

            // the complementary of the current colour
            float r = 1 - objects.GetComponent<MeshRenderer>().material.color.r;
            float g = 1 - objects.GetComponent<MeshRenderer>().material.color.g;
            float b = 1 - objects.GetComponent<MeshRenderer>().material.color.b;


            // set new outline with the complent
            var outline = objects.GetComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = new Color(r, g, b, 1);
            outline.OutlineWidth = 7f;
        }
    }
}
