using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAssigner : MonoBehaviour
{
    public string groupName;

    void Start()
    {
        groupName = gameObject.name;
        AssignGroupToButtons(transform);
    }

    private void AssignGroupToButtons(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Assign group if ButtonHandler is present
            ButtonHandler buttonHandler = child.GetComponent<ButtonHandler>();
            if (buttonHandler != null)
            {
                buttonHandler.group = groupName;
            }

            // Recursively call this method for all children
            if (child.childCount > 0)
            {
                AssignGroupToButtons(child);
            }
        }
    }
}
