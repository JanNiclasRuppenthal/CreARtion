using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    bool isHighlighted = false;
    Material originalMaterial;
    Material blueMaterial;
    MeshRenderer meshRenderer;

    GameObject baseObject;
    string obj_name;

    // Start is called before the first frame update
    void Start () {
		obj_name 		= this.gameObject.name;
		baseObject 		= GameObject.Find( obj_name );
		meshRenderer 		= baseObject.GetComponent<MeshRenderer>();
		originalMaterial 	= meshRenderer.material;
		
		Color blue 		= new Color(240.0f,248.0f,255.0f,0.5f);
		blueMaterial  		= new Material(Shader.Find("Transparent/Parallax Specular"));
		blueMaterial.color 	= blue;
    }
	
    // Update is called once per frame
    void Update () {
	
    }
	
    void OnClick(){
	Debug.Log("OMD "+obj_name);
	isHighlighted = !isHighlighted;

         // TODO: change Mode with UI_Script Method
		
	if( isHighlighted == true ){	
		HighlightBlue();
	}
	if ( isHighlighted==false ){		
		RemoveHighlight();
	}
    }
	
    void HighlightBlue(){
	meshRenderer.material = blueMaterial;
	Debug.Log("IT SHOULD BE BLUE");
    }
	
    void RemoveHighlight(){
	meshRenderer.material = originalMaterial;
    }
}
