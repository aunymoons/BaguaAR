using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;
using UnityEngine.Networking;
using TMPro;
using System.Net.Http;
using System.Text;
using BestHTTP;

public class Capture : MonoBehaviour
{
    public ARCameraManager m_CameraManager;
    public ARCameraBackground m_CameraBackground;
    public Camera mainCamera;
    public Camera maskCamera;
    public Shader whiteShader;
    public RenderTexture renderTexture;
    public CanvasGroup canvas;

    public RawImage rawimage1;
    public RawImage rawimage2;
    public RawImage rawimage3;
    public RenderTexture maskRenderTexture;
    public TMP_InputField promptText;
    public TMP_InputField urlTarget;
    private void Start()
    {

        // Create a new RenderTexture
        maskRenderTexture = new RenderTexture(UnityEngine.Screen.width, UnityEngine.Screen.height, 24);

        // Assign it to the camera
        maskCamera.targetTexture = maskRenderTexture;

        // Assign it to the RawImage
        rawimage3.texture = maskRenderTexture;
    }

    [SerializeField] RawImage outputRawImage; // Assign this in the editor


    public async void MakeAPICall(string prompt, Texture2D texture1, Texture2D texture2)
    {
        if (texture1.width != texture2.width || texture1.height != texture2.height)
        {
            throw new Exception("Textures are not of the same size.");
        }

        Texture2D resized1 = texture1; //ResizeTexture(texture1, 512, 512);
        Texture2D resized2 = texture2;//ResizeTexture(texture2, 512, 512);

        string base64_1 = Convert.ToBase64String(resized1.EncodeToPNG());
        string base64_2 = Convert.ToBase64String(resized2.EncodeToPNG());

        var controlNetArgsList = new List<ControlNetArg>
        {
            new ControlNetArg
            {
                module = "depth_midas",
                model = "control_v11f1p_sd15_depth [cfd03158]"
            }
        };

        var requestData = new RequestDataImg2Img
        {
            prompt = prompt,
            negative_prompt = "",
            steps = 100,
            cfg_scale = 7,
            sampler_name = "DPM++ 2M SDE Heun Karras",
            width = 768,
            height = 768,
            restore_faces = true,
            denoising_strength = 0.75f,
            init_images = new List<string> { base64_1 },
            mask = base64_2,
            inpainting_fill = 0,
            inpaint_full_res = true,
            sampler_index = "Euler",
            save_images = true,
            script_name = "",
            alwayson_scripts = new AlwaysonScripts
            {
                controlnet = new ControlNet
                {
                    args = controlNetArgsList
                }
            }
        };

        string json = JsonUtility.ToJson(requestData);

        HTTPRequest request = new HTTPRequest(new Uri($"{urlTarget.text}/sdapi/v1/img2img"), HTTPMethods.Post, (req, resp) =>
        {
            if (resp.IsSuccess)
            {
                string responseText = resp.DataAsText;
                var responseObj = JsonUtility.FromJson<MyResponse>(responseText);

                byte[] imageBytes = Convert.FromBase64String(responseObj.images[0]);
                Texture2D receivedTexture = new Texture2D(resized1.width, resized1.height);
                receivedTexture.LoadImage(imageBytes);

                outputRawImage.texture = receivedTexture;
            }
            else
            {
                Debug.LogError($"Error: {resp.Message}, Status Code: {resp.StatusCode}");
            }
        });
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.RawData = Encoding.UTF8.GetBytes(json);

        request.Send();
    }

    [Serializable]
    private class MyResponse
    {
        public List<string> images;
        public Dictionary<string, object> parameters;
        public string info;
    }

    private Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        float aspectRatio = (float)source.width / source.height;
        if (aspectRatio > 1f)
        {
            targetHeight = Mathf.RoundToInt(targetWidth / aspectRatio);
        }
        else
        {
            targetWidth = Mathf.RoundToInt(targetHeight * aspectRatio);
        }

        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        rt.Release();

        return result;
    }

    public void CaptureWithMask()
    {
        
        CaptureScreenshot();
        
        //CaptureMask();
    }

    private void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {

        //m_CameraBackground.enabled = false;

        //int oldMask = mainCamera.cullingMask;

        //mainCamera.cullingMask = (1 << LayerMask.NameToLayer("Object"));
        canvas.alpha = 0;
        canvas.interactable = false;

        yield return new WaitForEndOfFrame();

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

        //mainCamera.cullingMask = oldMask;
        canvas.alpha = 1;
        canvas.interactable = true;

        //maskCamera.Render();

        //Texture2D tex2 = ScreenCapture.CaptureScreenshotAsTexture();

        SaveToGallery(tex,"Screenshot.png", "Controlnet", rawimage1);
        SaveToGallery(RenderTextureToTexture2D(maskRenderTexture), "whatever", "whatever", rawimage3);

        
        MakeAPICall(promptText.text, tex, RenderTextureToTexture2D(maskRenderTexture));

        //SaveToGallery(tex2, "Screenshot.png", "Controlnet", rawimage3);

        //canvas.enabled = true;
        //m_CameraBackground.enabled = true;
        //CaptureRawCameraFeed();
    }

    Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        // Create a new Texture2D object with the dimensions of the RenderTexture
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Set the active RenderTexture to the one you want to read from
        RenderTexture.active = renderTexture;

        // Read the active RenderTexture into the Texture2D
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        // Apply changes (this is crucial, don't forget!)
        texture2D.Apply();

        // Reset the active RenderTexture
        RenderTexture.active = null;

        return texture2D;
    }

    private void CaptureRawCameraFeed()
    {
        GetImageAsync();
    }

    public byte[] RenderTextureToByteArray(RenderTexture renderTexture)
    {
        RenderTexture.active = renderTexture;
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;

        byte[] byteArray = texture2D.EncodeToPNG();
        return byteArray;
    }

    private void CaptureMask()
    {
        int width = UnityEngine.Screen.width;
        int height = UnityEngine.Screen.height;

        // Mask capture
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        maskCamera.targetTexture = renderTexture;
        maskCamera.Render();

        SaveToGallery(RenderTextureToTexture2D(renderTexture), "Mask.png", "Controlnet", rawimage3);

        maskCamera.ResetReplacementShader();
        maskCamera.targetTexture = null;
    }

    void AsynchronousConversion()
    {
        // Acquire an XRCpuImage
        if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // If successful, launch an asynchronous conversion coroutine
            StartCoroutine(ConvertImageAsync(image));

            // It is safe to dispose the image before the async operation completes
            image.Dispose();
        }
    }

    public void GetImageAsync()
    {
        // Acquire an XRCpuImage
        if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // Perform async conversion
            image.ConvertAsync(new XRCpuImage.ConversionParams
            {
                // Get the full image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Downsample by 2
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                // Color image format
                outputFormat = TextureFormat.RGB24,

                // Flip across the Y axis
                transformation = XRCpuImage.Transformation.MirrorY

                // Call ProcessImage when the async operation completes
            }, ProcessImage);

            // It is safe to dispose the image before the async operation completes
            image.Dispose();
        }
    }

    void ProcessImage(
        XRCpuImage.AsyncConversionStatus status,
        XRCpuImage.ConversionParams conversionParams,
        NativeArray<byte> data)
    {
        if (status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Async request failed with status {0}", status);
            return;
        }

        // Copy to a Texture2D, pass to a computer vision algorithm, etc
        DoSomethingWithImageData(data, conversionParams);

        // Data is destroyed upon return. No need to dispose
    }

    IEnumerator ConvertImageAsync(XRCpuImage image)
    {
        // Create the async conversion request
        var request = image.ConvertAsync(new XRCpuImage.ConversionParams
        {
            // Use the full image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Optionally downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Output an RGB color image format
            outputFormat = TextureFormat.RGB24,

            // Flip across the Y axis
            transformation = XRCpuImage.Transformation.MirrorY
        });

        // Wait for the conversion to complete
        while (!request.status.IsDone())
            yield return null;

        // Check status to see if the conversion completed successfully
        if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            // Something went wrong
            Debug.LogErrorFormat("Request failed with status {0}", request.status);

            // Dispose even if there is an error
            request.Dispose();
            yield break;
        }

        // Image data is ready. Let's apply it to a Texture2D
        var rawData = request.GetData<byte>();

        // Create a texture
        var texture = new Texture2D(
            request.conversionParams.outputDimensions.x,
            request.conversionParams.outputDimensions.y,
            request.conversionParams.outputFormat,
            false);

        // Copy the image data into the texture
        texture.LoadRawTextureData(rawData);
        texture.Apply();

        // Dispose the request including raw data
        request.Dispose();
    }

    void DoSomethingWithImageData(NativeArray<byte> data, ConversionParams conversionParams)
    {
        int width = conversionParams.outputDimensions.x;
        int height = conversionParams.outputDimensions.y;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.LoadRawTextureData(data);
        tex.Apply();

        // Save to Android gallery
        SaveToGallery(tex, "RawImage.png", "Controlnet", rawimage2);
    }

    void SaveToTexture(Texture2D bytes, int width, int height, RawImage rawImage)
    {
        NativeToolkit.SaveImage(bytes, DateTime.Now.ToString(), "png");
        rawImage.texture = bytes;
    }

    void SaveToGallery(Texture2D bytes, string filename, string album, RawImage rawImage)
    {
        int width = UnityEngine.Screen.width;
        int height = UnityEngine.Screen.height;
        SaveToTexture(bytes, width, height, rawImage);
        return;
    }

}
