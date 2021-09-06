using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
 * This class contains the logic of the mainmenu
 * All methods change the scene.
 * Methods:
 *      - switch to CreARtion
 *      - switch to Gallery
 *      - open an url link to dropbox
 */
public class MainMenu_Script : MonoBehaviour
{
    // In this link are the images of the ImageTargets, so that everybody can download them.
    private string url = "https://www.dropbox.com/sh/4takb350i80cplm/AADP4ZQDYn7RJqRzyyGyeWnra?dl=0";
    public Text warn;

    /*
     * These functions are for the buttons.
     */

    // If you tap on one button, a new scene will be loaded.
    public void FreeBuilding_Click()
    {
        SceneManager.LoadScene("CreARtion");
    }

    public void Gallery_Click()
    {
        SceneManager.LoadScene("Gallery");
    }


    // open the DropBox URL to downlaod the ImageTargets.
    public void OpenURL_Click()
    {
        // test if user is connected to the internet
        if (Application.internetReachability == 0)
        {
            warn.text = "You need internet connection!";
            return;
        }

        warn.text = "";
        Application.OpenURL(url);
        
    }
}
