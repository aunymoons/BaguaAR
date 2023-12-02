using UnityEngine;
using TMPro;

public class SaveInput : MonoBehaviour
{
    public string uniqueID;
    private TMP_InputField inputField;

    private void Start()
    {
        // Get the TMP_InputField component
        inputField = GetComponent<TMP_InputField>();

        // Load saved string from PlayerPrefs
        string savedInput = PlayerPrefs.GetString(uniqueID, "");
        inputField.text = savedInput;

        // Add listener for automatic saving
        inputField.onValueChanged.AddListener(delegate { SaveInputValue(); });
    }

    private void SaveInputValue()
    {
        // Save the current input to PlayerPrefs
        PlayerPrefs.SetString(uniqueID, inputField.text);
    }
}
