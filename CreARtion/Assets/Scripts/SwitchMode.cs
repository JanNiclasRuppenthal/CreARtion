using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMode : MonoBehaviour
{

	public GameObject listStagesPositioners;
	public GameObject uiSelectionmode;


	public GameObject deleteButton;
	public GameObject XButton;
	public GameObject scrollableListManipulation;

	// Update is called once per frame
	void Update()
	{
		// Touching the Objects
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// look for Hit
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit))
			{

				// Deactivate all Stages and Positioners
				// Your are not able to place an object anymore
				switchToManipulationmode();

				hit.collider.GetComponent<Transform>().localScale = new Vector3(0.15f, 0.15f, 0.15f);

			}
		}
		else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

			// look for Hit
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit))
			{

				// Deactivate all Stages and Positioners
				// Your are not able to place an object anymore
				switchToManipulationmode();

				// Reaction of the phone
				// So that I know if it works
				Handheld.Vibrate();

				hit.collider.GetComponent<Transform>().localScale = new Vector3(0.15f, 0.15f, 0.15f);
			}
		}
	}


	// the x-button in the manipulation mode calls this function
	public void switchToSelectionmode()
    {
		listStagesPositioners.SetActive(true);
		uiSelectionmode.SetActive(true);
		deleteButton.SetActive(false);
		XButton.SetActive(false);
		scrollableListManipulation.SetActive(false);
    }

	// There is no button which call this function
	// Thats why it is private
	private void switchToManipulationmode()
	{
		listStagesPositioners.SetActive(false);
		uiSelectionmode.SetActive(false);
		deleteButton.SetActive(true);
		XButton.SetActive(true);
		scrollableListManipulation.SetActive(true);
	}
}
