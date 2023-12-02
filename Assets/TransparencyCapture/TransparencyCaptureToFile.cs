using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransparencyCaptureToFile:MonoBehaviour
{
    public RawImage targetImage;
    public IEnumerator capture()
    {

        yield return new WaitForEndOfFrame();
        //After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
        //Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
        targetImage.texture = zzTransparencyCapture.captureScreenshot();
    }

    public void CaptureAR()
    {
            StartCoroutine(capture());
    }
}