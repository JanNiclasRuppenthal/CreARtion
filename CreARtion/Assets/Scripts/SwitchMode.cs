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


	public GameObject baseObject;

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

				var outline = hit.collider.gameObject.GetComponent<Outline>();

				baseObject = hit.collider.gameObject;

				float r = 1 - baseObject.GetComponent<MeshRenderer>().material.color.r;
				float g = 1 - baseObject.GetComponent<MeshRenderer>().material.color.g;
				float b = 1 - baseObject.GetComponent<MeshRenderer>().material.color.b;


				outline.OutlineMode = Outline.Mode.OutlineAll;
				outline.OutlineColor = new Color(r, g, b, 1);
				outline.OutlineWidth = 5f;



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


				var outline = hit.collider.gameObject.GetComponent<Outline>();

				baseObject = hit.collider.gameObject;

				float r = 255 - baseObject.GetComponent<MeshRenderer>().material.color.r;
				float g = 255 - baseObject.GetComponent<MeshRenderer>().material.color.g;
				float b = 255 - baseObject.GetComponent<MeshRenderer>().material.color.b;


				outline.OutlineMode = Outline.Mode.OutlineAll;
				outline.OutlineColor = new Color(r, g, b, 1);
				outline.OutlineWidth = 5f;

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

		var outline = baseObject.GetComponent<Outline>();


		outline.OutlineMode = Outline.Mode.OutlineAll;
		outline.OutlineColor = new Color(0,0,0,0);
		outline.OutlineWidth = 5f;
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


	public void deleteButton_OnClick()
    {
		Destroy(baseObject);
		switchToSelectionmode();
    }
}
