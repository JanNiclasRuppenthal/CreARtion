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
            objects.transform.eulerAngles = new Vector3(cX.getCurrentValue(), cY.getCurrentValue(), CurrentValue);
        }
    }
}