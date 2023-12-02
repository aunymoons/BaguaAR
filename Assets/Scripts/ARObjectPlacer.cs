using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using Unity.XR.CoreUtils;

public class ARObjectPlacer : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public RaycastDetector raycastDetector;
    public Camera arCamera;
    public GameObject objectToPlacePrefab;
    private GameObject previewObject;
    private bool placementActive = false;
    public string currentGroup;
    public Material transparentMaterial;


    bool TouchOrClickReceived()
    {
        // Check for touch input
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed && !IsPointerOverUIObject(touch.position.ReadValue()))
                {
                    return true;
                }
            }
        }

        // Check for mouse input
        if (Mouse.current != null && Mouse.current.leftButton.isPressed && !IsPointerOverUIObject(Mouse.current.position.ReadValue()))
        {
            return true;
        }

        return false;
    }

    // Helper method to check if the touch or click is on UI element
    bool IsPointerOverUIObject(Vector2 position)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(position.x, position.y)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
    void Update()
    {
        if (placementActive)
        {
            UpdatePlacementPose();

            // New Input System handling for touch or click
            if (TouchOrClickReceived())
            {
                placementActive = false;
                PlaceObject();
            }
        }
    }
    public void ActivatePlacement(GameObject prefab)
    {
        objectToPlacePrefab = prefab;
        CreatePreviewObject();
        placementActive = true;
    }

    void CreatePreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = Instantiate(objectToPlacePrefab);
        SetMaterialToTransparent(previewObject);
        // Optionally disable any non-visual components that shouldn't be active in the preview
    }

    void SetMaterialToTransparent(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>(true); // true for including inactive children
        foreach (var renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = transparentMaterial;
            }
            renderer.materials = newMaterials;
        }
    }

    void UpdatePlacementPose()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon | TrackableType.PlaneWithinBounds);

        foreach (var hit in hits)
        {
            ARPlane plane = (ARPlane)hit.trackable;
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                previewObject.transform.position = hit.pose.position;
                previewObject.transform.rotation = hit.pose.rotation;
                previewObject.SetActive(true);
                return;
            }
        }

        previewObject.SetActive(false);
    }

    void PlaceObject()
    {
        // Instantiate the object at the previewObject's position and rotation
        GameObject instance = Instantiate(objectToPlacePrefab, previewObject.transform.position, previewObject.transform.rotation);

        // Set up the metadata if available
        var metadata = instance.GetComponent<ObjectMetadata>();
        if (metadata != null)
        {
            metadata.category = currentGroup;
        }

        // Set up the anchor if available
        var anchorAssigner = instance.GetComponent<AnchorAssigner>();
        if (anchorAssigner != null)
        {
            anchorAssigner.StartCall();
        }

        // Create a new anchor gameObject like in the previous implementation
        GameObject anchor = new GameObject("PlacementAnchor");
        anchor.transform.position = previewObject.transform.position;
        anchor.transform.rotation = previewObject.transform.rotation;

        // Set the instantiated object as a child of the anchor
        instance.transform.parent = anchor.transform;

        // Find the TrackablesParent from the XR Origin or AR Session Origin
        anchor.transform.parent = xrOrigin.TrackablesParent;

        // Clean up the preview object
        Destroy(previewObject);
        placementActive = false;
        raycastDetector.isEditing = false;
    }

    public XROrigin xrOrigin;
}