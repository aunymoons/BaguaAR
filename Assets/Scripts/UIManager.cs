using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject uiPrefab; // Assign your UI prefab here
    public GameObject containerPrefab;
    public GameObject noObjectPrefab;
    public Transform container; // Assign the container for instantiated UI elements
    public Transform containerWealth;
    public Transform containerFame;
    public Transform containerRelationship;
    public Transform containerFamily;
    public Transform containerHealth;
    public Transform containerCreativity;
    public Transform containerKnowledge;
    public Transform containerCarreer;
    public Transform containerTravel;

    private List<ObjectMetadata> arObjects = new List<ObjectMetadata>();
    public List<GameObject> instantiatedContainers = new List<GameObject>();

    private void Awake()
    {
        InstantiateUIElements();
    }

    public void RegisterObject(ObjectMetadata arData)
    {
        if (!arObjects.Contains(arData))
            arObjects.Add(arData);
    }

    public void DeregisterObject(ObjectMetadata arData)
    {
        if (arObjects.Contains(arData))
            arObjects.Remove(arData);
    }

    public void InstantiateUIElements()
    {
        // Clear all containers
        ClearContainer(container);

        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "WEALTH"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "FAME"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "FAMILY"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "RELATIONSHIPS"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "HEALTH"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "CREATIVITY"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "KNOWLEDGE"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "CAREER"));
        instantiatedContainers.Add(InstantiatePrefabInContainer(containerPrefab, container, "TRAVEL"));

        // Instantiate UI elements in appropriate containers
        foreach (ObjectMetadata arData in arObjects)
        {
            Transform targetContainer = GetContainerForCategory(arData.category);
            GameObject uiElement = Instantiate(uiPrefab, targetContainer);
            uiElement.GetComponentInChildren<Image>().sprite = arData.image;
            uiElement.GetComponentInChildren<TextMeshProUGUI>().text = $"{arData.objectName}\n{arData.description}";
        }

        // Remove container instances with no more than one child
        RemoveEmptyContainers();

        // If all containers are removed, instantiate 'noObjectPrefab'
        if (instantiatedContainers.Count == 0)
        {
            Instantiate(noObjectPrefab, container);
        }
    }

    private void RemoveEmptyContainers()
    {
        for (int i = instantiatedContainers.Count - 1; i >= 0; i--)
        {
            if (instantiatedContainers[i].transform.childCount == 1)
            {
                Destroy(instantiatedContainers[i]);
                instantiatedContainers.RemoveAt(i);
            }
        }
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            if(child.GetComponent<TextMeshProUGUI>() == null)
            {
                Destroy(child.gameObject);
            }
            
        }
    }

    private ref Transform GetContainerForCategory(string category)
    {
        switch (category)
        {
            case "WEALTH":
                return ref containerWealth;
            case "FAME":
                return ref containerFame;
            case "RELATIONSHIPS":
                return ref containerRelationship;
            case "FAMILY":
                return ref containerFamily;
            case "HEALTH":
                return ref containerHealth;
            case "CREATIVITY":
                return ref containerCreativity;
            case "KNOWLEDGE":
                return ref containerKnowledge;
            case "CAREER":
                return ref containerCarreer;
            case "TRAVEL":
                return ref containerTravel;
            default:
                return ref container; // Or some default container if needed
        }
    }

    public GameObject InstantiatePrefabInContainer(GameObject prefab, Transform container, string category)
    {
        // Instantiate the prefab in the container
        GameObject instance = Instantiate(prefab, container);

        GetContainerForCategory(category) = instance.transform;

        // Set the color of the image in the prefab
        Image imageComponent = instance.GetComponent<Image>();
        
        if (imageComponent != null)
        {
            imageComponent.color = GetColorForCategory(category);
        }

        TextMeshProUGUI textMesh = instance.GetComponentInChildren<TextMeshProUGUI>();
        if(textMesh != null)
        {
            textMesh.text = GetDescription(category);
        }

        return instance;
    }

    private Color GetColorForCategory(string category)
    {
        switch (category)
        {
            case "WEALTH":
                return new Color(0.33f, 0.1f, 0.55f); // A richer shade of purple
            case "FAME":
                return new Color(0.9f, 0.2f, 0.2f); // A deep, yet vibrant red
            case "RELATIONSHIPS":
                return new Color(1f, 0.6f, 0.7f); // A soft, inviting pink
            case "FAMILY":
                return new Color(0.2f, 0.8f, 0.2f); // A soothing, natural green
            case "HEALTH":
                return new Color(0.9f, 0.9f, 0.4f); // A warm, comforting yellow
            case "CREATIVITY":
                return new Color(0.7f, 0.7f, 0.7f); // A gentle, neutral gray
            case "KNOWLEDGE":
                return new Color(0.1f, 0.1f, 0.6f); // A deep, thoughtful blue
            case "CAREER":
                return new Color(0.2f, 0.2f, 0.2f); // A strong, solid black
            case "TRAVEL":
                return new Color(0.5f, 0.5f, 0.5f); // A strong, solid black
            default:
                return new Color(0.95f, 0.95f, 0.95f); // A light, versatile white
        }
    }

    private string GetDescription(string category)
    {
        switch (category)
        {
            case "WEALTH":
                return $"{category}:\nExplore the treasures you've selected for the Wealth square. These gems aren't just pretty; they're keys to unlocking greater prosperity. Did you know? In Feng Shui, the color purple is often associated with wealth.";
            case "FAME":
                return $"{category}:\nTake a look at your Fame square selections. Each item is a stepping stone to greater recognition. Remember, the fire element fuels fame, so any hints of red can ignite your path to success.";
            case "RELATIONSHIPS":
                return $"{category}:\nYour choices for the Relationships square are more than just objects; they're symbols of harmony and love. Soft, rounded shapes here foster smoother interactions in your personal connections.";
            case "FAMILY":
                return $"{category}:\nThe Family square is enriched with your picks, each nurturing stronger family ties. The wood element is a pillar of this square, symbolizing growth and vitality within family bonds.";
            case "HEALTH":
                return $"{category}:\nYour Health square selections are not only aesthetically pleasing but also vital for your well-being. Ever heard that earth tones in this area can ground and stabilize your health energy?";
            case "CREATIVITY":
                return $"{category}:\nYour chosen items for the Creativity square are brimming with inspiration. Metals in this area can sharpen creativity, turning your space into a hotbed of innovative ideas.";
            case "KNOWLEDGE":
                return $"{category}:\nThe items you've gathered for the Knowledge square are more than just objects; they're beacons of wisdom. It’s said that earthy elements here can solidify your quest for knowledge.";
            case "CAREER":
                return $"{category}:\nYour Career square choices pave the way for professional advancement. Water elements in this area are believed to help your career flow smoothly, just like a meandering river.";
            case "TRAVEL":
                return $"{category}:\nHere are the items you chose based on the Travel bagua square. These will boost your adventures and help you with your explorations.";
            default:
                return $"{category}:\n";
        }
    }
}