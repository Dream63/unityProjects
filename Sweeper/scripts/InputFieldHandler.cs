using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Util;


public class InputFieldHandler : MonoBehaviour
{
    public enum InputType { Integer, Float, String }

    public TMP_InputField inputField;
    private string startingInputText;

    public InputType inputFieldType = InputType.String;
    public float minimumNumber = 404, maximumNumber = 404;

    private void Awake()
    {
        gameObject.TryGetComponent<TMP_InputField>(out inputField);
        inputField.onValueChanged.AddListener(delegate { LegalValueChange(); });
        startingInputText = inputField.text;
    }

    public void LegalValueChange()
    {
        if(inputFieldType == InputType.Integer)
        {
            if (Utils.IsInt(inputField.text, out int value))
            {
                value = Utils.Clamp(value, (int)minimumNumber, (int)maximumNumber);
                inputField.text = value.ToString();
                startingInputText = value.ToString();
            }
            else inputField.text = startingInputText;
        }
        if (inputFieldType == InputType.Float)
        {
            if (Utils.IsFloat(inputField.text, out float value))
            {
                value = Utils.Clamp(value, (float)minimumNumber, (float)maximumNumber);
                inputField.text = value.ToString();
                startingInputText = value.ToString();
            }
            else inputField.text = startingInputText;
        }
    }
    
}
