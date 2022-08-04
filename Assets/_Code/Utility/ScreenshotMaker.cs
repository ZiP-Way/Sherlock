using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private string fileName;

    #region "Fields"

    private int index = 0;

    #endregion
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ScreenCapture.CaptureScreenshot(fileName + index.ToString() + ".png");
            index++;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Time.timeScale = 1;
        }
    }
#endif
}