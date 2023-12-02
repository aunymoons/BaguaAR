using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class RaycastDetector : MonoBehaviour
{
    public Transform rayOrigin;  // The point from which ray originates
    public LayerMask targetLayer;  // Layer on which your 9 colliders reside
    private string lastHitSquareName = "";  // Stores the last hit square name


    public CanvasGroup wealthGroup;
    public CanvasGroup fameGroup;
    public CanvasGroup relationshipsGroup;
    public CanvasGroup familyGroup;
    public CanvasGroup healthGroup;
    public CanvasGroup childrenGroup;
    public CanvasGroup knowledgeGroup;
    public CanvasGroup careerGroup;
    public CanvasGroup travelGroup;

    public CanvasGroup parentGroup;
    public CanvasGroup cartGroup;

    public bool isReady;
    public bool isEditing;
    [SerializeField]
    public bool isCheckout;

    bool enabledCart;

    public void SetCheckout(bool value)
    {
        isCheckout = value;
    }

    void Update()
    {
        if (isCheckout)
        {
            parentGroup.alpha = 0;
            parentGroup.interactable = false;
            parentGroup.blocksRaycasts = false;
            cartGroup.alpha = 0;
            cartGroup.interactable = false;
            cartGroup.blocksRaycasts = false;
        }
        else
        {


            if (!enabledCart && isReady)
            {
                cartGroup.alpha = 1;
                cartGroup.interactable = true;
                cartGroup.blocksRaycasts = true;
            }

            if (isReady)
            {
                if (isCheckout && parentGroup.alpha != 0)
                {
                    parentGroup.alpha = 0;
                    parentGroup.interactable = false;
                    parentGroup.blocksRaycasts = false;
                    return;
                }

                if (isEditing && parentGroup.alpha != 0)
                {
                    parentGroup.alpha = 0;
                    parentGroup.interactable = false;
                    parentGroup.blocksRaycasts = false;
                    cartGroup.alpha = 0;
                    cartGroup.interactable = false;
                    cartGroup.blocksRaycasts = false;

                }
                else if (!isEditing && parentGroup.alpha != 1)
                {
                    parentGroup.alpha = 1;
                    parentGroup.interactable = true;
                    parentGroup.blocksRaycasts = true;
                    cartGroup.alpha = 1;
                    cartGroup.interactable = true;
                    cartGroup.blocksRaycasts = true;

                }

                /*
                RaycastHit hit;
                string currentHitSquareName = "";

                if (Physics.Raycast(rayOrigin.position, Vector3.down, out hit, Mathf.Infinity, targetLayer))
                {
                    SquareIdentifier squareIdentifier = hit.collider.GetComponent<SquareIdentifier>();
                    if (squareIdentifier != null)
                    {
                        currentHitSquareName = squareIdentifier.squareName;
                    }
                }
                else
                {
                    currentHitSquareName = "No square detected";
                }

                // Log only if it's a new square or no square detected
                if (!currentHitSquareName.Equals(lastHitSquareName))
                {
                    Debug.Log($"Status: {currentHitSquareName}");
                    SetActiveGroup(currentHitSquareName);
                    lastHitSquareName = currentHitSquareName;
                }
                */

            }

        }
    }


    public void SetActiveGroup(string option)
    {
        // First, disable all CanvasGroups
        wealthGroup.alpha = 0;
        wealthGroup.interactable = false;
        wealthGroup.blocksRaycasts = false;

        fameGroup.alpha = 0;
        fameGroup.interactable = false;
        fameGroup.blocksRaycasts = false;

        relationshipsGroup.alpha = 0;
        relationshipsGroup.interactable = false;
        relationshipsGroup.blocksRaycasts = false;

        familyGroup.alpha = 0;
        familyGroup.interactable = false;
        familyGroup.blocksRaycasts = false;

        healthGroup.alpha = 0;
        healthGroup.interactable = false;
        healthGroup.blocksRaycasts = false;

        childrenGroup.alpha = 0;
        childrenGroup.interactable = false;
        childrenGroup.blocksRaycasts = false;

        knowledgeGroup.alpha = 0;
        knowledgeGroup.interactable = false;
        knowledgeGroup.blocksRaycasts = false;

        careerGroup.alpha = 0;
        careerGroup.interactable = false;
        careerGroup.blocksRaycasts = false;

        travelGroup.alpha = 0;
        travelGroup.interactable = false;
        travelGroup.blocksRaycasts = false;

        // Then enable the selected one
        switch (option)
        {
            case "WEALTH":
                wealthGroup.alpha = 1;
                wealthGroup.interactable = true;
                wealthGroup.blocksRaycasts = true;
                break;
            case "FAME":
                fameGroup.alpha = 1;
                fameGroup.interactable = true;
                fameGroup.blocksRaycasts = true;
                break;
            case "RELATIONSHIPS":
                relationshipsGroup.alpha = 1;
                relationshipsGroup.interactable = true;
                relationshipsGroup.blocksRaycasts = true;
                break;
            case "FAMILY":
                familyGroup.alpha = 1;
                familyGroup.interactable = true;
                familyGroup.blocksRaycasts = true;
                break;
            case "HEALTH":
                healthGroup.alpha = 1;
                healthGroup.interactable = true;
                healthGroup.blocksRaycasts = true;
                break;
            case "CREATIVITY":
                childrenGroup.alpha = 1;
                childrenGroup.interactable = true;
                childrenGroup.blocksRaycasts = true;
                break;
            case "KNOWLEDGE":
                knowledgeGroup.alpha = 1;
                knowledgeGroup.interactable = true;
                knowledgeGroup.blocksRaycasts = true;
                break;
            case "CAREER":
                careerGroup.alpha = 1;
                careerGroup.interactable = true;
                careerGroup.blocksRaycasts = true;
                break;
            case "TRAVEL":
                travelGroup.alpha = 1;
                travelGroup.interactable = true;
                travelGroup.blocksRaycasts = true;
                break;
            case "OFF":
                // All are already disabled
                break;
        }
    }

}
