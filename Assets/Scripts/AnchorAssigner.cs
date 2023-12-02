using UnityEngine;

public class AnchorAssigner : MonoBehaviour
{
    public void StartCall()
    {
        BaguaOnboarding onboardingScript = FindObjectOfType<BaguaOnboarding>();
        if (onboardingScript != null)
        {
            onboardingScript.OnObjectPlaced(transform);
        }
        else
        {
            Debug.LogError("BaguaOnboarding instance not found in the scene.");
        }
    }
}