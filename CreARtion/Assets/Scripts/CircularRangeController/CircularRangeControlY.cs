using System;
using UnityEngine;


public class CircularRangeControlY : CircularRange
{
    public CircularRangeControlX cX;
    public CircularRangeControlZ cZ;


    public override void rotate()
    {
        foreach (GameObject objects in listofObjects)
        {
            Vector3 v = objects.transform.localRotation.eulerAngles;
            objects.transform.eulerAngles = new Vector3(cX.getCurrentValue(), CurrentValue, cZ.getCurrentValue());
        }
    }
}