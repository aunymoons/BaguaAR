using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SquareIdentifier : MonoBehaviour
{
    public string squareName;

    void Update()
    {
        // Refactor to include UI check
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.wasPressedThisFrame && !IsPointerOverUIObject(touch.position.ReadValue()))
                {
                    Debug.LogError("Touch Began at position: " + touch.position.ReadValue());
                    CheckInput(touch.position.ReadValue());
                }
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (!IsPointerOverUIObject(mousePosition))
            {
                Debug.LogError("Mouse Click at position: " + mousePosition);
                CheckInput(mousePosition);
            }
        }
    }

    private bool IsPointerOverUIObject(Vector2 pos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void CheckInput(Vector2 screenPosition)
    {
        // Additional UI check
        if (IsPointerOverUIObject(screenPosition))
        {
            return;
        }
        Debug.Log("Checking Input at Screen Position: " + screenPosition);

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.LogError("Raycast Hit: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject == gameObject) // Check if the hit object is this object
            {
                Debug.LogError("Hit the intended gameObject: " + gameObject.name);

                RaycastDetector externalComponent = FindObjectOfType<RaycastDetector>();
                if (externalComponent != null)
                {
                    Debug.LogError("External component found. Setting active group to: " + squareName);
                    externalComponent.SetActiveGroup(squareName);
                }
                else
                {
                    Debug.LogWarning("External component not found!");
                }
            }
            else
            {
                Debug.LogError("Hit a different gameObject: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.LogError("Raycast did not hit any object");
        }
    }

}
