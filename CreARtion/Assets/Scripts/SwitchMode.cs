using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vuforia;

public class SwitchMode : MonoBehaviour
{

	public GameObject listStagesPositioners;
	public GameObject uiSelectionmode;
	public GameObject uiManipulationmode;
	public GameObject uiRotation;
	public GameObject uiControlPad;
	public GameObject switchMoveControl;
	public UI_Manipulation_Script ui_Manipulation_Script;

	
	private GameObject baseObject;

	private HashSet<GameObject> listOfMarkedObjects = new HashSet<GameObject>();

	private Dictionary<GameObject, Transform> dictObjectStage = new Dictionary<GameObject, Transform>();

	//private float cooldown = 0.15f;

	//[SerializeField]
	//private float timeDelay = 0.1f;

	// Update is called once per frame
	void Update()
	{

		if (ui_Manipulation_Script.currentState != UI_Manipulation_Script.manipulationStates.Select)
        {
			// do NOT detect a gameobject in the scene
        }


		// Touching Objects
		else if (Input.GetMouseButton(0) /*&& Time.time > cooldown*/)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// behind the list
			// Check if the mouse was clicked over a UI element
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}

			//cooldown = Time.time + timeDelay;

			markObjects(ray);
		}
		else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began /*&& Time.time > cooldown*/)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

			// behind the list
			// Check if the mouse was clicked over a UI element
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}

			//cooldown = Time.time + timeDelay;

			markObjects(ray);
		}
	}


	/*
	 * This method is called several times because of the update() function.
	 * It will be called every frame.
	 * That is the reason why objects are copied several times. 
	 * The ArrayList had several instances of the same object.
	 */
	private void markObjects(Ray ray)
    {

		// look for Hit
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit))
		{

			// Deactivate all Stages and Positioners
			// Your are not able to place an object anymore
			switchToManipulationmode();


			var outline = hit.collider.gameObject.GetComponent<Outline>();

			// save the object
			baseObject = hit.collider.gameObject;

			// if the object is already in the list, do not add it to the ArrayList
			foreach (GameObject markedObjects in listOfMarkedObjects)
            {
				if (markedObjects == baseObject)
                {
					//listOfMarkedObjects.Remove(baseObject);
					//Debug.Log(listOfMarkedObjects.Count+1);

					//var outlineBaseObject = baseObject.GetComponent<Outline>();

					//outlineBaseObject.OutlineMode = Outline.Mode.OutlineHidden;
					//outlineBaseObject.OutlineColor = new Color(0, 0, 0, 0);
					//outlineBaseObject.OutlineWidth = 0f;

					return;
                }
            }


			// get the complementary colour as outline of this object
			float r = 1 - baseObject.GetComponent<MeshRenderer>().material.color.r;
			float g = 1 - baseObject.GetComponent<MeshRenderer>().material.color.g;
			float b = 1 - baseObject.GetComponent<MeshRenderer>().material.color.b;

			// set the new outline
			outline.OutlineMode = Outline.Mode.OutlineAll;
			outline.OutlineColor = new Color(r, g, b, 1);
			outline.OutlineWidth = 7f;

			// save the object in HashSet
			listOfMarkedObjects.Add(baseObject);

			// save object and stage in the dictionary
			try
			{
				dictObjectStage.Add(baseObject, baseObject.transform.parent.parent);
			}
			catch
            {
				//ignore
            }
		}
	}


	// the x-button in the manipulation mode calls this function
	public void switchToSelectionmode()
    {
		// activate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(true);
		uiSelectionmode.SetActive(true);

		// disable the manipulationmode and the UIs of the manipulationstools
		uiManipulationmode.SetActive(false);
		uiRotation.SetActive(false);
		uiControlPad.SetActive(false);
		switchMoveControl.SetActive(false);


		// there is no outline after you enter the selectionmode
		foreach(GameObject item in listOfMarkedObjects)
        {
			var outline = item.GetComponent<Outline>();

			outline.OutlineMode = Outline.Mode.OutlineHidden;
			outline.OutlineColor = new Color(0, 0, 0, 0);
			outline.OutlineWidth = 0f;

		}


		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();
		
		// reset Icon Highlighting
		ui_Manipulation_Script.resetIconHighlighting();

		listOfMarkedObjects.Clear();
		dictObjectStage.Clear();

		// no current state of a manipualtion mode
		ui_Manipulation_Script.currentState = UI_Manipulation_Script.manipulationStates.Select;

	}

	// There is no button which call this function
	// Thats why it is private
	private void switchToManipulationmode()
	{
		// deactivate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(false);
		uiSelectionmode.SetActive(false);
		uiControlPad.SetActive(false);
		ui_Manipulation_Script.setControlPadBoolsOnFalse();

		// enable the manipulationmode
		uiManipulationmode.SetActive(true);

		ui_Manipulation_Script.highlightIcon();

	}


	public void deleteButton_OnClick()
    {

		// destroy object

		ArrayList listIndex = new ArrayList();
		foreach (GameObject item in listOfMarkedObjects)
		{
			GameObject temp = item.transform.parent.parent.gameObject;
			Destroy(temp);

			listIndex.Add(item);

		}

		foreach (GameObject item in listIndex)
        {
			listOfMarkedObjects.Remove(item);
        }

		listIndex.Clear();

		// reset Icon Highlighting
		ui_Manipulation_Script.resetIconHighlighting();
		
		// switch to the selectionmode
		switchToSelectionmode();
    }

	// Getter
	public HashSet<GameObject> getListOfMarkedObjects(){
		return listOfMarkedObjects;
	}

	public Dictionary<GameObject, Transform> getDictionaryObjectStage()
    {
		return dictObjectStage;
    }
}
