using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Sample : MonoBehaviour
{
    public InputField input;

    public void OnClick()
    {
        input.Select();
        StartCoroutine(Select(input));
    }

    public void OnValueChange(InputField o)
    {
        Debug.Log(string.Format("Sample:OnValueChange ({0})", o.text));
    }
    public void OnEndEdit(InputField o)
    {
        Debug.Log(string.Format("Sample:OnEndEdit ({0})", o.text));
    }

    public void Update()
    {
        //Debug.Log(string.Format("selection:({0}) - ({1}) at ({0})", input.selectionFocusPosition, input.selectionAnchorPosition, input.caretPosition));
    }

    IEnumerator Select(InputField input)
    {
        yield return new WaitForEndOfFrame();
        input.caretPosition = 2;
        //input.selectionAnchorPosition = 4;
        //input.selectionFocusPosition = 2;

        yield return new WaitForEndOfFrame();
        //Debug.Log(string.Format("selectionFocusPosition:({0})", input.selectionFocusPosition));
        //Debug.Log(string.Format("selectionAnchorPosition:({0})", input.selectionAnchorPosition));

        input.Rebuild(CanvasUpdate.LatePreRender);
        input.Rebuild(CanvasUpdate.MaxUpdateValue);
    }
}
