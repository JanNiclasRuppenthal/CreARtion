using System;
using UnityEngine;


public class CircularRangeControlX : CircularRange
{
   
    public CircularRangeControlY cY;
    public CircularRangeControlZ cZ;


    public override void rotate()
    {
        foreach (GameObject objects in listofObjects)
        {
            objects.transform.localEulerAngles = new Vector3(CurrentValue, cY.getCurrentValue(), cZ.getCurrentValue());
        }
    }
}