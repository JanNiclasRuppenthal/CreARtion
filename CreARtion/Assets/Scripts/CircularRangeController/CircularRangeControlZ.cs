using System;
using UnityEngine;


public class CircularRangeControlZ : CircularRange
{

    public CircularRangeControlX cX;
    public CircularRangeControlY cY;

    public override void rotate()
    {
        foreach (GameObject objects in listofObjects)
        {
            Vector3 v = objects.transform.localRotation.eulerAngles;
            objects.transform.localEulerAngles = new Vector3(cX.getCurrentValue(), cY.getCurrentValue(), CurrentValue);
        }
    }
}