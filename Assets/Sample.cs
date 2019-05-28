using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebGLSupport;

public class Sample : MonoBehaviour
{
    public InputField input;

    public void OnValueChange(InputField o)
    {
        Debug.Log(string.Format("Sample:OnValueChange[{1}] ({0})", o.text, o.name));
    }
    public void OnEndEdit(InputField o)
    {
        Debug.Log(string.Format("Sample:OnEndEdit[{1}] ({0})", o.text, o.name));
    }
    public void OnCopy()
    {
        var text = input.text;
#if UNITY_WEBGL && !UNITY_EDITOR
        var res = WebGLDocument.CopyToClipboard(text);
        Debug.Log(res);
#else
        GUIUtility.systemCopyBuffer = text;
#endif
    }
}
