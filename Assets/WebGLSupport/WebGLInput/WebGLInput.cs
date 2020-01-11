#if UNITY_2018_2_OR_NEWER
#define TMP_WEBGL_SUPPORT
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using AOT;
using System.Runtime.InteropServices; // for DllImport
using System.Collections;

namespace WebGLSupport
{
    class WebGLInputPlugin
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern int WebGLInputCreate(string canvasId, int x, int y, int width, int height, int fontsize, string text, bool isMultiLine, bool isPassword);

        [DllImport("__Internal")]
        public static extern void WebGLInputEnterSubmit(int id, bool flag);

        [DllImport("__Internal")]
        public static extern void WebGLInputTab(int id, Action<int, int> cb);

        [DllImport("__Internal")]
        public static extern void WebGLInputFocus(int id);

        [DllImport("__Internal")]
        public static extern void WebGLInputOnFocus(int id, Action<int> cb);

        [DllImport("__Internal")]
        public static extern void WebGLInputOnBlur(int id, Action<int> cb);

        [DllImport("__Internal")]
        public static extern void WebGLInputOnValueChange(int id, Action<int, string> cb);
        
        [DllImport("__Internal")]
        public static extern void WebGLInputOnEditEnd(int id, Action<int, string> cb);

        [DllImport("__Internal")]
        public static extern int WebGLInputSelectionStart(int id);

        [DllImport("__Internal")]
        public static extern int WebGLInputSelectionEnd(int id);

        [DllImport("__Internal")]
        public static extern int WebGLInputSelectionDirection(int id);

        [DllImport("__Internal")]
        public static extern void WebGLInputSetSelectionRange(int id, int start, int end);

        [DllImport("__Internal")]
        public static extern void WebGLInputMaxLength(int id, int maxlength);

        [DllImport("__Internal")]
        public static extern void WebGLInputText(int id, string text);

        [DllImport("__Internal")]
        public static extern bool WebGLInputIsFocus(int id);

        [DllImport("__Internal")]
        public static extern void WebGLInputDelete(int id);
#else

        public static int WebGLInputCreate(string canvasId, int x, int y, int width, int height, int fontsize, string text, bool isMultiLine, bool isPassword) { return 0; }
        public static void WebGLInputEnterSubmit(int id, bool flag) { }
        public static void WebGLInputTab(int id, Action<int, int> cb) { }
        public static void WebGLInputFocus(int id) { }
        public static void WebGLInputOnFocus(int id, Action<int> cb) { }
        public static void WebGLInputOnBlur(int id, Action<int> cb) { }
        public static void WebGLInputOnValueChange(int id, Action<int, string> cb) { }
        public static void WebGLInputOnEditEnd(int id, Action<int, string> cb) { }
        public static int WebGLInputSelectionStart(int id) { return 0; }
        public static int WebGLInputSelectionEnd(int id) { return 0; }
        public static int WebGLInputSelectionDirection(int id) { return 0; }
        public static void WebGLInputSetSelectionRange(int id, int start, int end) { }
        public static void WebGLInputMaxLength(int id, int maxlength) { }
        public static void WebGLInputText(int id, string text) { }
        public static bool WebGLInputIsFocus(int id) { return false; }
        public static void WebGLInputDelete(int id) { }
#endif
    }

    public class WebGLInput : MonoBehaviour, IComparable<WebGLInput>
    {
        static Dictionary<int, WebGLInput> instances = new Dictionary<int, WebGLInput>();
        public static string CanvasId { get; set; }

        static WebGLInput()
        {
#if UNITY_2019_1_OR_NEWER
            WebGLInput.CanvasId = "unityContainer";
#else
            WebGLInput.CanvasId = "gameContainer";
#endif
        }

        int id = -1;
        IInputField input;
        bool blueBlock = false;

        private IInputField Setup()
        {
            if (GetComponent<InputField>()) return new WrappedInputField(GetComponent<InputField>());
#if TMP_WEBGL_SUPPORT
            if (GetComponent<TMPro.TMP_InputField>()) return new WrappedTMPInputField(GetComponent<TMPro.TMP_InputField>());
#endif // TMP_WEBGL_SUPPORT
            throw new Exception("Can not Setup WebGLInput!!");
        }

        private void Awake()
        {
            input = Setup();
#if !(UNITY_WEBGL && !UNITY_EDITOR)
            // WebGL 以外、更新メソッドは動作しないようにします
            enabled = false;
#endif
        }
        /// <summary>
        /// 対象が選択されたとき
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(/*BaseEventData eventData*/)
        {
            var rect = GetScreenCoordinates(input.TextComponentRectTransform());
            bool isPassword = input.contentType == ContentType.Password;

            var x = (int)(rect.x);
            //var y = (int)(Screen.height - (rect.y + rect.height));
            //id = WebGLInputPlugin.WebGLInputCreate(x, y, (int)rect.width, (int)rect.height, input.textComponent.fontSize, input.text);
            var y = (int)(Screen.height - (rect.y));
            id = WebGLInputPlugin.WebGLInputCreate(WebGLInput.CanvasId, x, y, (int)rect.width, (int)1, input.fontSize, input.text, input.lineType != LineType.SingleLine, isPassword);

            instances[id] = this;
            WebGLInputPlugin.WebGLInputEnterSubmit(id, input.lineType != LineType.MultiLineNewline);
            WebGLInputPlugin.WebGLInputOnFocus(id, OnFocus);
            WebGLInputPlugin.WebGLInputOnBlur(id, OnBlur);
            WebGLInputPlugin.WebGLInputOnValueChange(id, OnValueChange);
            WebGLInputPlugin.WebGLInputOnEditEnd(id, OnEditEnd);
            WebGLInputPlugin.WebGLInputTab(id, OnTab);
            // default value : https://www.w3schools.com/tags/att_input_maxlength.asp
            WebGLInputPlugin.WebGLInputMaxLength(id, (input.characterLimit > 0) ? input.characterLimit : 524288);
            WebGLInputPlugin.WebGLInputFocus(id);

            if(input.OnFocusSelectAll)
            {
                WebGLInputPlugin.WebGLInputSetSelectionRange(id, 0, input.text.Length);
            }

            WebGLWindow.OnBlurEvent += OnWindowBlur;
        }

        void OnWindowBlur()
        {
            blueBlock = true;
        }

        /// <summary>
        /// 画面内の描画範囲を取得する
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        Rect GetScreenCoordinates(RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);

            // try to support RenderMode:WorldSpace
            var canvas = uiElement.GetComponentInParent<Canvas>();
            var hasCamera = (canvas.renderMode != RenderMode.ScreenSpaceOverlay) && (canvas.worldCamera != null);
            if (canvas && hasCamera )
            {
                for (var i = 0; i < worldCorners.Length; i++)
                {
                    worldCorners[i] = canvas.worldCamera.WorldToScreenPoint(worldCorners[i]);
                }
            }

            var min = new Vector3(float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue);
            for (var i = 0; i < worldCorners.Length; i++)
            {
                min.x = Mathf.Min(min.x, worldCorners[i].x);
                min.y = Mathf.Min(min.y, worldCorners[i].y);
                max.x = Mathf.Max(max.x, worldCorners[i].x);
                max.y = Mathf.Max(max.y, worldCorners[i].y);
            }

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        void DeactivateInputField()
        {
            WebGLInputPlugin.WebGLInputDelete(id);
            input.DeactivateInputField();
            instances.Remove(id);
            id = -1;    // reset id to -1;
            WebGLWindow.OnBlurEvent -= OnWindowBlur;
        }

        [MonoPInvokeCallback(typeof(Action<int>))]
        static void OnFocus(int id)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.WebGLInput.captureAllKeyboardInput = false;
#endif
        }

        [MonoPInvokeCallback(typeof(Action<int>))]
        static void OnBlur(int id)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.WebGLInput.captureAllKeyboardInput = true;
#endif
            instances[id].StartCoroutine(Blue(id));
        }
        static IEnumerator Blue(int id)
        {
            yield return null;
            if (!instances.ContainsKey(id)) yield break;

            var block = instances[id].blueBlock;    // get blue block state
            instances[id].blueBlock = false;        // reset instalce block state
            if (block) yield break;                 // if block. break it!!

            instances[id].DeactivateInputField();
        }

        [MonoPInvokeCallback(typeof(Action<int, string>))]
        static void OnValueChange(int id, string value)
        {
            if (!instances.ContainsKey(id)) return;

            var instance = instances[id];
            var index = instance.input.caretPosition;
            if(!instance.input.ReadOnly)
            {
                instance.input.text = value;
            }
            

            // InputField.ContentType.Name が Name の場合、先頭文字が強制的大文字になるため小文字にして比べる
            if (instance.input.contentType == ContentType.Name)
            {
                if (string.Compare(instance.input.text, value, true) == 0)
                {
                    value = instance.input.text;
                }
            }

            // InputField の ContentType による整形したテキストを HTML の input に再設定します
            if (value != instance.input.text)
            {
                WebGLInputPlugin.WebGLInputText(id, instance.input.text);
                WebGLInputPlugin.WebGLInputSetSelectionRange(id, index, index);
            }
        }
        [MonoPInvokeCallback(typeof(Action<int, string>))]
        static void OnEditEnd(int id, string value)
        {
            if (!instances[id].input.ReadOnly)
            {
                instances[id].input.text = value;
            }
        }
        [MonoPInvokeCallback(typeof(Action<int, int>))]
        static void OnTab(int id, int value)
        {
            WebGLInputTabFocus.OnTab(instances[id], value);
        }

        void Update()
        {
            if (input == null || !input.isFocused) return;
            // 未登録の場合、選択する
            if (!instances.ContainsKey(id))
            {
                OnSelect();
            } else if(!WebGLInputPlugin.WebGLInputIsFocus(id))
            {
                // focus this id
                WebGLInputPlugin.WebGLInputFocus(id);
            }

            var start = WebGLInputPlugin.WebGLInputSelectionStart(id);
            var end = WebGLInputPlugin.WebGLInputSelectionEnd(id);
            // 選択方向によって設定します
            if (WebGLInputPlugin.WebGLInputSelectionDirection(id) == -1)
            {
                input.selectionFocusPosition = start;
                input.selectionAnchorPosition = end;
            }
            else
            {
                input.selectionFocusPosition = end;
                input.selectionAnchorPosition = start;
            }

            input.Rebuild(CanvasUpdate.LatePreRender);
            input.SetAllDirty();
        }
        private void OnEnable()
        {
            WebGLInputTabFocus.Add(this);
        }
        private void OnDisable()
        {
            WebGLInputTabFocus.Remove(this);
        }
        public int CompareTo(WebGLInput other)
        {
            var a = GetScreenCoordinates(input.TextComponentRectTransform());
            var b = GetScreenCoordinates(other.input.TextComponentRectTransform());
            var res = b.y.CompareTo(a.y);
            if (res == 0) res = a.x.CompareTo(b.x);
            return res;
        }

        /// <summary>
        /// to manage tab focus
        /// base on scene position
        /// </summary>
        static class WebGLInputTabFocus
        {
            static List<WebGLInput> inputs = new List<WebGLInput>();

            public static void Add(WebGLInput input)
            {
                inputs.Add(input);
                inputs.Sort();
            }

            public static void Remove(WebGLInput input)
            {
                inputs.Remove(input);
            }

            public static void OnTab(WebGLInput input, int value)
            {
                if (inputs.Count <= 1) return;
                var index = inputs.IndexOf(input);
                index += value;
                if (index < 0) index = inputs.Count - 1;
                else if (index >= inputs.Count) index = 0;
                inputs[index].input.ActivateInputField();
            }
        }
    }
}
