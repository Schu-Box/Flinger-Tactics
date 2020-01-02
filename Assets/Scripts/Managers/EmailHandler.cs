using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class EmailHandler : MonoBehaviour {

    public TextMeshProUGUI thankYouText;

    private TMP_InputField inputField;

    void Start() {
        inputField = GetComponent<TMP_InputField>();

        ResetThankYouText();
    }
    
    public void SubmitInput() {
        string input = inputField.text;

        if(input.Length > 0) {
            WriteEmailToFile(input);

            thankYouText.text = "Thank You!";

            inputField.text = "";
        }
    }

    private void WriteEmailToFile(string input) {
        string path = "Assets/Resources/EmailList.txt";

        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(input);
        writer.Close();

        Debug.Log("New text file: " + input);
    }

    public void ResetThankYouText() {
        thankYouText.text = "";
    }
}
