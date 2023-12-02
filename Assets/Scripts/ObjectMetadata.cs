using UnityEngine;

public class ObjectMetadata : MonoBehaviour
{
    public string objectName;
    public string description;
    public string category;
    public Sprite image;

    private void OnEnable()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.RegisterObject(this);
        }
    }

    private void OnDisable()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.DeregisterObject(this);
        }
    }
}
