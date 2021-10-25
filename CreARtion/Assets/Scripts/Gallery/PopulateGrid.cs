using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateGrid : MonoBehaviour
{
    public UI_Gallery UIGallery;
    public GameObject prefab;

    private ArrayList buttons = new ArrayList();

    void Start()
    {
        if (UIGallery.getFiles().Length > 0)
        {
            Populate();
        }
    }

    public void Populate()
    {
        GameObject newObj;

        for (int i = 0; i < UIGallery.getFiles().Length; i++)
        {
            int icopy = 0;
            newObj = (GameObject) Instantiate(prefab, transform);
            string pathToFile = UIGallery.getFiles()[i];
            icopy = i;
            Texture2D texture = GetScreenshotImage(pathToFile);
            Sprite sp = Sprite.Create(texture, new Rect((texture.width-texture.height)/2, 0, texture.height, texture.height),
                new Vector2(0.5f, 0.5f));
            newObj.GetComponent<Image>().sprite = sp;
            newObj.GetComponent<Button>().onClick.AddListener(() => UIGallery.ClickImage(icopy));

            // add every button in an ArrayList
            buttons.Add(newObj);
        }
    }
    
    // return the picture with the path
    private Texture2D GetScreenshotImage(string filePath)
    {
        Texture2D texture = null;
        byte[] fileBytes;
        if (File.Exists(filePath))
        {
            fileBytes = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.LoadImage(fileBytes);
        }
        return texture;
    }

    public ArrayList getButtons()
    {
        return buttons;
    }
}
