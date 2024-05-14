using UnityEngine;
using UnityEngine.UIElements;

namespace WebGLSupport
{
    public class WebGLInputManipulator : Manipulator
    {
        private GameObject go;
        private bool showHtmlElement;

        public WebGLInputManipulator(bool showHtmlElement = false)
        {
            this.showHtmlElement = showHtmlElement;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            // uitoolkit is already support mobile.
            if (!Application.isMobilePlatform)
            {
                var textInput = target.Q("unity-text-input");
                textInput.RegisterCallback<FocusInEvent>(OnFocusInEvent);
                textInput.RegisterCallback<FocusOutEvent>(OnFocusOutEvent);
            }
        }
        protected override void UnregisterCallbacksFromTarget()
        {
            // uitoolkit is already support mobile.
            if (!Application.isMobilePlatform)
            {
                var textInput = target.Q("unity-text-input");
                textInput.UnregisterCallback<FocusInEvent>(OnFocusInEvent);
                textInput.UnregisterCallback<FocusOutEvent>(OnFocusOutEvent);
            }
        }

        private void OnFocusInEvent(FocusInEvent evt)
        {
            if (go != null)
            {
                GameObject.Destroy(go);
                go = null;
            }

            go = new GameObject("WebGLInputManipulator");

            // add WebGLUIToolkitMonoBehaviour for hold TextField!
            var uitoolkit = go.AddComponent<WebGLUIToolkitTextField>();
            uitoolkit.TextField = target as TextField;

            // add WebGLInput to handle the event!
            var webglInput = go.AddComponent<WebGLInput>();
            webglInput.showHtmlElement = showHtmlElement;

            // select it!!
            webglInput.OnSelect();
        }

        private void OnFocusOutEvent(FocusOutEvent evt)
        {
            if (go != null)
            {
                GameObject.Destroy(go);
                go = null;
            }
        }
    }
}
