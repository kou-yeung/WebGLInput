using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WebGLSupport;


public class Sample : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;

    public void Start()
    {
        uiDocument.rootVisualElement.Query<TextField>().ForEach(v =>
        {
            v.AddManipulator(new WebGLInputManipulator());
        });

        WebGLSupport.WebGLInput.OnKeyboardDown += OnKeyboardDown;
        WebGLSupport.WebGLInput.OnKeyboardUp += OnKeyboardUp;
    }

    private void OnKeyboardUp(WebGLSupport.WebGLInput input, KeyboardEvent keyboardEvent)
    {
        Debug.Log(string.Format("Sample:OnKeyboardUp({0}) shift({1}) ctrl({2}) alt({3})", keyboardEvent.Key, keyboardEvent.ShiftKey, keyboardEvent.CtrlKey, keyboardEvent.AltKey));
    }

    private void OnKeyboardDown(WebGLSupport.WebGLInput input, KeyboardEvent keyboardEvent)
    {
        Debug.Log(string.Format("Sample:OnKeyboardDown({0}) shift({1}) ctrl({2}) alt({3})", keyboardEvent.Key, keyboardEvent.ShiftKey, keyboardEvent.CtrlKey, keyboardEvent.AltKey));
    }

    public void OnValueChange(InputField o)
    {
        Debug.Log(string.Format("Sample:OnValueChange[{1}] ({0})", o.text, o.name));
    }
    public void OnEndEdit(InputField o)
    {
        Debug.Log(string.Format("Sample:OnEndEdit[{1}] ({0})", o.text, o.name));
    }

    public void OnClickFullScreen()
    {
        WebGLWindow.SwitchFullscreen();
    }
}
