using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class TakeScreenshot : MonoBehaviour
{
    public int pixelDensity = 4;

    private bool takeHiResShot = false;

    public Text textUI;

    public string ScreenShotName(int width, int height)
    {
        if ( textUI != null ) return string.Format("{0}/{1}_{2}.png",
                             System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                             textUI.text,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

        return string.Format("{0}/screen_{1}.png",
                             System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }
     
    void Start()
    {

    }

    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown(KeyCode.Space);
        
        
        if (takeHiResShot)
        {
            int resWidth = Camera.main.pixelWidth * pixelDensity;
            int resHeight = Camera.main.pixelHeight * pixelDensity;

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            Camera.main.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            Camera.main.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            Camera.main.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
        
    }
}