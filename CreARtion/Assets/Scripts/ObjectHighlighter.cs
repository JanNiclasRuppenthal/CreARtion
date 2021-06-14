using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
	private bool isHighlighted = false;
	private Material originalMaterial;
	private Material blueMaterial;
	private MeshRenderer meshRenderer;

    private GameObject baseObject;

    // Start is called before the first frame update
    void Start () 
	{

    }

	public void setBaseObject(GameObject obj)
    {
		baseObject = obj;

	}
	
    // Update is called once per frame
    void Update () {
	
    }
	
    void OnClick()
	{
		isHighlighted = !isHighlighted;
		
		if( isHighlighted == true )
		{	
			HighlightBlue();
		}
		if ( isHighlighted==false )
		{		
			RemoveHighlight();
		}
    }
	
    public void HighlightBlue()
	{
		//meshRenderer.material = blueMaterial;
		Debug.Log("IT SHOULD BE BLUE");

		meshRenderer = baseObject.GetComponent<MeshRenderer>();
		//originalMaterial = meshRenderer.material;

		Color blue = new Color(240.0f, 0.0f, 0.0f, 1.0f);
		//blueMaterial = new Material(Shader.Find("Transparent/Parallax Specular"));
		meshRenderer.material.color = blue;

		Debug.Log(baseObject.GetComponent<MeshRenderer>().material.color);
	}
	
    public void RemoveHighlight()
	{
		meshRenderer.material = originalMaterial;
    }
}
