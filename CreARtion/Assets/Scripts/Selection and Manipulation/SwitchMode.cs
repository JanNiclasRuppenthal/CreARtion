using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This class implements the function to switch between
 * manipulationmode and selectionmode.
 * Methods:
 *		- switch to selectionmode
 *		- switch to manipulationmode
 *		- mark objects
 *		- delete several or all stages
 *		- acitvate one certain UI element
 */
public class SwitchMode : MonoBehaviour
{
	// all UI GameObjects
	public GameObject listStagesPositioners;
	public GameObject uiSelectionmode;
	public GameObject uiManipulationmode;
	public GameObject uiRotation;
	public GameObject uiControlPad;
	public GameObject switchMoveControl;
	public GameObject uiColourSlider;
	public GameObject uiStretchButtons;
	public GameObject moveUI;

	// ArrayList of all UI GameObjects
	private ArrayList listUI = new ArrayList();

	public UI_Manipulation_Script ui_Manipulation_Script;

	// a variable to save the current marked object
	private GameObject baseObject;

	private HashSet<GameObject> listOfMarkedObjects = new HashSet<GameObject>();

	// a dictionary of a marked object and their Mid Air Stage
	private Dictionary<GameObject, Transform> dictObjectStage = new Dictionary<GameObject, Transform>();


	void Start()
    {
		// add every UI GameObject to the ArrayList
		listUI.AddRange(new List<GameObject>
		{
			uiSelectionmode,
			uiManipulationmode,
			uiRotation,
			uiControlPad,
			switchMoveControl,
			uiColourSlider,
			uiStretchButtons,
			moveUI
		});
    }


    // Update is called once per frame
    void Update()
    {

        if (ui_Manipulation_Script.currentState != UI_Manipulation_Script.manipulationStates.Select)
        {
			// do NOT detect a gameobject in the scene
			return;
        }

		// Tuching Objects
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            // behind the list
            // Check if the mouse was clicked over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            markObjects(ray);
        }


        // Touching Objects
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // behind the list
            // Check if the mouse was clicked over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            markObjects(ray);
        }
#endif
    }

	/*
     *  A method to mark the objects with a Raycast.
     *  save the object and stage in the HashSet and Dictionary.
     *  Every marked object gets an outline
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

			// save the object
			baseObject = hit.collider.gameObject;

			var outline = baseObject.GetComponent<Outline>();


			// if the object is already in the list, do not add it to the HashSet again
			foreach (GameObject markedObjects in listOfMarkedObjects)
            {
				if (markedObjects == baseObject)
				{ 
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

	// deactivate all UI elements except the parameter
	private void activateGameObjects(GameObject gameObject)
    {
		foreach (GameObject ui in listUI)
		{
			if (ui == gameObject)
			{
				ui.SetActive(true);
				continue;
			}

			ui.SetActive(false);
		}
	}


	// the x-button in the manipulation mode calls this function
	public void switchToSelectionmode()
    {
		// activate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(true);

		// disable the manipulationmode and the UIs of the manipulationtools
		activateGameObjects(uiSelectionmode);


		// there is no outline after you enter the selectionmode
		foreach (GameObject item in listOfMarkedObjects)
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

		// clear the HashSet and the Dictionary
		listOfMarkedObjects.Clear();
		dictObjectStage.Clear();

		// no current state of a manipualtion mode
		ui_Manipulation_Script.currentState = UI_Manipulation_Script.manipulationStates.Select;
	}

	// A method to switch to the manipulation mode
	private void switchToManipulationmode()
	{
		// deactivate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(false);
		activateGameObjects(uiManipulationmode);

		// setup the manipulationmode
		ui_Manipulation_Script.setControlPadBoolsOnFalse();
		ui_Manipulation_Script.highlightIcon();
		ui_Manipulation_Script.ButtonSelect_Click();
	}

	// a method for the delete button
	// It destroys the stages from the HashSet listOfMarkedObjects
	public void deleteButton_OnClick()
    {
		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();

		// a teporary HashSet
		HashSet<GameObject> listIndex = new HashSet<GameObject>();
		foreach (GameObject item in listOfMarkedObjects)
		{
			GameObject temp = item.transform.parent.parent.gameObject;
			// destroy Mid Air Stage
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

	// a method for the delete ALL button
	// It destroys the stages with a tag
	public void clearAllButton_OnClick()
	{
		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();

		// destroy all placed objects and their Mid Air Stages
		// Every Stage has a Tag "MidAirStage"
		foreach (GameObject item in GameObject.FindGameObjectsWithTag("MidAirStage"))
		{
			Destroy(item);
		}

		// clear the listOfMarkedObjects
		listOfMarkedObjects.Clear();

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
