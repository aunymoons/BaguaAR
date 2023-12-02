using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ButtonHandler : MonoBehaviour
{
    public GameObject myPrefab; // Prefab to be set
    public string group;
    public ARObjectPlacer arPlacement; // Reference to ARPlacementInteractable
    public RaycastDetector raycastDetector;

    private void Awake()
    {
        arPlacement = FindObjectOfType<ARObjectPlacer>();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        arPlacement.ActivatePlacement(myPrefab);
        arPlacement.currentGroup = group;
        arPlacement.enabled = true;
        raycastDetector.isEditing = true;
    }

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}