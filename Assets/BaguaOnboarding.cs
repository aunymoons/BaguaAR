using BestHTTP.SecureProtocol.Org.BouncyCastle.Tls;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class BaguaOnboarding : MonoBehaviour
{

    public QuadResizer baguaQuad;
    public ARPlacementInteractable arPlacement;
    public GameObject frontPrefab;
    public GameObject backPrefab;
    public GameObject baguaQuadPrefab;
    public GameObject anchorPrefab;
    public Transform anchor1;
    public Transform anchor2;
    public CanvasGroup tutorial1;
    public CanvasGroup tutorial2;
    public CanvasGroup tutorial3;
    public CanvasGroup tutorial4;
    public CanvasGroup tutBG;
    public RaycastDetector raycastDetector;


    private void CloseAllPopups()
    {
        ClosePopup(tutorial1);
        ClosePopup(tutorial2);
        ClosePopup(tutorial3);
        ClosePopup(tutorial4);
    }

    public void ClosePopup(CanvasGroup canvas)
    {
        StartCoroutine(ClosePopupCoroutine(canvas));
    }
    public void OpenPopup(CanvasGroup canvas)
    {
        StartCoroutine(OpenPopupCoroutine(canvas));
    }

    public IEnumerator ClosePopupCoroutine(CanvasGroup canvas)
    {
        float startTime = Time.time;
        float duration = 0.25f;
        float startAlpha = canvas.alpha;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvas.alpha = Mathf.Lerp(startAlpha, 0, t);
            tutBG.alpha = Mathf.Lerp(startAlpha, 0, t);
            yield return null;
        }

        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;

        tutBG.alpha = 0;
        tutBG.blocksRaycasts = false;
        tutBG.interactable = false;
    }

    public IEnumerator OpenPopupCoroutine(CanvasGroup canvas)
    {

        float startTime = Time.time;
        float duration = 0.25f;
        float startAlpha = canvas.alpha;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvas.alpha = Mathf.Lerp(startAlpha, 1, t);
            tutBG.alpha = Mathf.Lerp(startAlpha, 1, t);
            yield return null;
        }

        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        canvas.interactable = true;

        tutBG.alpha = 1;
        tutBG.blocksRaycasts = true;
        tutBG.interactable = true;
    }

    public ARObjectPlacer arObjectPlacer;


  
    public void EnableDelayed()
    {
        StartCoroutine(EnableDelayedCR());
    }

    public IEnumerator EnableDelayedCR()
    {
        yield return new WaitForSeconds(.6f);
        //arPlacement.enabled = true;
        arObjectPlacer.ActivatePlacement(anchorPrefab);

    }

    private enum OnboardingStage
    {
        Tutorial1,
        Tutorial2,
        Tutorial3,
        Tutorial4,
        Completed
    }

    private OnboardingStage currentStage = OnboardingStage.Tutorial1;

    private void Awake()
    {
        CloseAllPopups();
        ShowTutorial1();
    }

    private void ShowTutorial1()
    {
        currentStage = OnboardingStage.Tutorial1;
        arPlacement.enabled = false;
        OpenPopup(tutorial1);
    }

    private void ShowTutorial2()
    {
        currentStage = OnboardingStage.Tutorial2;
        arPlacement.enabled = false;
        OpenPopup(tutorial2);
    }

    private void ShowTutorial3()
    {
        currentStage = OnboardingStage.Tutorial3;
        arPlacement.enabled = false;
        OpenPopup(tutorial3);
    }

    private void ShowTutorial4()
    {
        currentStage = OnboardingStage.Tutorial4;
        arPlacement.enabled = false;
        OpenPopup(tutorial4);
    }

    public void OnOkButtonPressed()
    {
        switch (currentStage)
        {
            case OnboardingStage.Tutorial1:
            case OnboardingStage.Tutorial2:
                EnableARPlacementAndClosePopups();
                break;
            case OnboardingStage.Tutorial3:
                CloseAllPopups();
                ShowTutorial4();
                break;
            case OnboardingStage.Tutorial4:
                InstantiateBaguaQuad();
                CloseAllPopups();
                raycastDetector.isReady = true;
                arPlacement.enabled = false;
                break;
            default:
                break;
        }
    }

    private void EnableARPlacementAndClosePopups()
    {
        CloseAllPopups();
        StartCoroutine(EnableDelayedCR());
    }

    private void InstantiateBaguaQuad()
    {
        baguaQuad = Instantiate(baguaQuadPrefab).GetComponent<QuadResizer>();
        baguaQuad.point1 = anchor1;
        baguaQuad.point2 = anchor2;
    }

    public void OnObjectPlaced(Transform objectPlaced)
    {
        if (currentStage == OnboardingStage.Tutorial1 || currentStage == OnboardingStage.Tutorial2)
        {
            AssignAnchor(objectPlaced);
            
        }
    }

    public void AssignAnchor(Transform placed)
    {
        if (currentStage == OnboardingStage.Tutorial1)
        {
            anchor1 = placed;
            ShowTutorial2();
        }
        else if (currentStage == OnboardingStage.Tutorial2)
        {
            anchor2 = placed;
            ShowTutorial3();
        }
    }
}
