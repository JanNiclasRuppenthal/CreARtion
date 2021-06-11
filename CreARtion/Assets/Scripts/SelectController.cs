using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
	// Touching the Objects
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
		Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

		// look for Hit
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(ray, out hit)){
			hit.transform.gameObject.SendMessage("OnClick");
			Debug.Log("touch event is called");
		}
	}
    }
}
