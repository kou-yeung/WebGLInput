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
