using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class SwitchMode : MonoBehaviour
{

	public GameObject listStagesPositioners;
	public GameObject uiSelectionmode;
	public GameObject uiManipulationmode;
	public GameObject uiRotation;

	
	private GameObject baseObject;

	private ArrayList listOfMarkedObjects = new ArrayList();


	// Update is called once per frame
	void Update()
	{


		// Touching Objects
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			markObjects(ray);
		}
		else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			markObjects(ray);
		}
	}


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


			// get the complementary colour as outline of this object
			float r = 1 - baseObject.GetComponent<MeshRenderer>().material.color.r;
			float g = 1 - baseObject.GetComponent<MeshRenderer>().material.color.g;
			float b = 1 - baseObject.GetComponent<MeshRenderer>().material.color.b;

			// set the new outline
			outline.OutlineMode = Outline.Mode.OutlineAll;
			outline.OutlineColor = new Color(r, g, b, 1);
			outline.OutlineWidth = 5f;

			// save the object in ArrayList
			listOfMarkedObjects.Add(baseObject);

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


		// there is no outline after you enter the selectionmode
		foreach(GameObject item in listOfMarkedObjects)
        {
			var outline = item.GetComponent<Outline>();

			outline.OutlineMode = Outline.Mode.OutlineAll;
			outline.OutlineColor = new Color(0, 0, 0, 0);
			outline.OutlineWidth = 5f;
		}

		listOfMarkedObjects.Clear();

	}

	// There is no button which call this function
	// Thats why it is private
	private void switchToManipulationmode()
	{
		// deactivate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(false);
		uiSelectionmode.SetActive(false);

		// enable the manipulationmode
		uiManipulationmode.SetActive(true);
	}


	public void deleteButton_OnClick()
    {

		// destroy object

		ArrayList listIndex = new ArrayList();
		foreach (GameObject item in listOfMarkedObjects)
		{
			GameObject temp = item;
			Destroy(temp);

			listIndex.Add(item);

		}

		foreach (GameObject item in listIndex)
        {
			listOfMarkedObjects.Remove(item);
        }

		listIndex.Clear();


		// switch to the selectionmode
		switchToSelectionmode();
    }

	// Getter
	public ArrayList getListOfMarkedObjects(){
		return listOfMarkedObjects;
	}
}
