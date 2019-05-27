using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebGLSupport;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        WebGLDocumentPlugin.WebGLDocumentCopyToClipboard("Hello World!!1");
    }
    public void OnValueChange(InputField o)
    {
        Debug.Log(string.Format("Sample:OnValueChange[{1}] ({0})", o.text, o.name));
    }
    public void OnEndEdit(InputField o)
    {
        Debug.Log(string.Format("Sample:OnEndEdit[{1}] ({0})", o.text, o.name));
    }
}
