using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CircularRange : MonoBehaviour
{
    public Transform Origin; //center of rotation
    public Image ImageSelected; //drag here the image of type filled radial top
    public Text Angle; //value textual feedback
    public Text DetectedValue; //value textual feedback

    public SwitchMode sw;
    protected HashSet<GameObject> listofObjects;

    public int Scale = 360; //value scale to use

    public int CurrentValue;
    public State CircularButtonState = State.NOT_DRAGGING;


    public int getCurrentValue()
    {
        return CurrentValue;
    }

    public void setCurrentValue(int value)
    {
        CurrentValue = value;
    }

    void Start()
    {
        listofObjects = sw.getListOfMarkedObjects();

    }

    public enum State
    {
        NOT_DRAGGING,
        DRAGGING,
    }

    public void DragOnCircularArea(bool isClick)
    {
        //we ignore the click event due to dragging in order to 
        //ignore beyond max set with drag and enable it on click / touch
        if (isClick && CircularButtonState == State.DRAGGING)
        {
            CircularButtonState = State.NOT_DRAGGING;
            return;
        }

        if (isClick)
            CircularButtonState = State.NOT_DRAGGING;
        else
        {
            CircularButtonState = State.DRAGGING;
        }

        float f = Vector3.Angle(Vector3.up, Input.mousePosition - Origin.position);
        bool onTheRight = Input.mousePosition.x > Origin.position.x;
        int detectedValue = onTheRight ? (int)f : 180 + (180 - (int)f);

        if (detectedValue > 350)
            detectedValue = 360;
        else if (CurrentValue == 360 && detectedValue < 10)
            detectedValue = 360;
        else if (CurrentValue == 0 && detectedValue > 350)
            detectedValue = 0;
        else if (detectedValue < 10)
            detectedValue = 0;

        if (!isClick)
        {
            if (detectedValue <= CurrentValue && Mathf.Abs(CurrentValue - detectedValue) > 180)
                detectedValue = CurrentValue;
            else if (CurrentValue == 0 && detectedValue > 270)
                detectedValue = CurrentValue;
        }

        CurrentValue = detectedValue;
        Angle.text = "" + (int)(CurrentValue * Scale / 360f) + "°";
        ImageSelected.fillAmount = CurrentValue / 360f;

        rotate();
    }


    // Rotate Object around Z axis
    public abstract void rotate();

    public void Reset()
    {
        Angle.text = "0°";
        CurrentValue = 0;
        ImageSelected.fillAmount = 0f;
    }
}
