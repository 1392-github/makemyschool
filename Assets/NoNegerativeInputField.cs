using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoNegerativeInputField : MonoBehaviour
{
    UnityEngine.UI.InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<UnityEngine.UI.InputField>();
        inputField.onValueChanged.AddListener(ValueChanged);
    }
    public void ValueChanged(string txt)
    {
        if (txt.Length > 0 && txt[0] == '-') inputField.text = txt.Remove(0, 1);
    }
}
