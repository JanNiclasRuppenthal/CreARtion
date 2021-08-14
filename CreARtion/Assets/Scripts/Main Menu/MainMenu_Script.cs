using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /*
     * These functions are for the buttons
     * If you tap on one button, a new scene will be loaded.
     */

    public void FreeBuilding_Click()
    {
        SceneManager.LoadScene("CreARtion");
    }

    public void LegoMode_Click()
    {
        SceneManager.LoadScene("Lego-Mode");
    }

    public void Gallery_Click()
    {
        SceneManager.LoadScene("Gallery");
    }

    public void Options_Click()
    {
        // a new UI appears
    }
}
