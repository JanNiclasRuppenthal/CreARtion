using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vuforia;
using Object = UnityEngine.Object;

public class SwitchMode : MonoBehaviour
{

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

	
	private GameObject baseObject;

	private HashSet<GameObject> listOfMarkedObjects = new HashSet<GameObject>();

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
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began /*&& Time.time > cooldown*/)
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


        // Touching Objects
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) /*&& Time.time > cooldown*/)
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
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Hier");
        if (ui_Manipulation_Script.currentState != UI_Manipulation_Script.manipulationStates.Select)
        {
            // do NOT detect a gameobject in the scene
        }

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


	public void activateGameObjects(GameObject gameObject)
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

		// disable the manipulationmode and the UIs of the manipulationstools
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
		activateGameObjects(uiManipulationmode);

		ui_Manipulation_Script.setControlPadBoolsOnFalse();

		ui_Manipulation_Script.highlightIcon();

		ui_Manipulation_Script.ButtonSelect_Click();

	}


	public void deleteButton_OnClick()
    {
		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();

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
	
	public void clearAllButton_OnClick()
	{
		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();

		

		// destroy all placed objects
		foreach (GameObject item in GameObject.FindGameObjectsWithTag("MidAirStage"))
		{
			//GameObject temp = item.transform.parent.parent.gameObject;
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
