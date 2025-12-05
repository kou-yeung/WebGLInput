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
using UnityEngine.EventSystems;

namespace WebGLSupport
{
    internal class WebGLInputPlugin
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void WebGLInputInit();
        [DllImport("__Internal")]
        public static extern int WebGLInputCreate(string canvasId, int x, int y, int width, int height, int fontsize, string text, string placeholder, bool isMultiLine, bool isPassword, bool isHidden, bool isMobile, string autoComplete);

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
        public static extern void WebGLInputOnKeyboardEvent(int id, Action<int, int, string, int, int, int, int> cb);

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

        [DllImport("__Internal")]
        public static extern void WebGLInputForceBlur(int id);

#if WEBGLINPUT_TAB
        [DllImport("__Internal")]
        public static extern void WebGLInputEnableTabText(int id, bool enable);
#endif
#else
        public static void WebGLInputInit() { }
        public static int WebGLInputCreate(string canvasId, int x, int y, int width, int height, int fontsize, string text, string placeholder, bool isMultiLine, bool isPassword, bool isHidden, bool isMobile, string autoComplete) { return 0; }
        public static void WebGLInputEnterSubmit(int id, bool flag) { }
        public static void WebGLInputTab(int id, Action<int, int> cb) { }
        public static void WebGLInputFocus(int id) { }
        public static void WebGLInputOnFocus(int id, Action<int> cb) { }
        public static void WebGLInputOnBlur(int id, Action<int> cb) { }
        public static void WebGLInputOnValueChange(int id, Action<int, string> cb) { }
        public static void WebGLInputOnEditEnd(int id, Action<int, string> cb) { }
        public static void WebGLInputOnKeyboardEvent(int id, Action<int, int, string, int, int, int, int> cb) { }
        public static int WebGLInputSelectionStart(int id) { return 0; }
        public static int WebGLInputSelectionEnd(int id) { return 0; }
        public static int WebGLInputSelectionDirection(int id) { return 0; }
        public static void WebGLInputSetSelectionRange(int id, int start, int end) { }
        public static void WebGLInputMaxLength(int id, int maxlength) { }
        public static void WebGLInputText(int id, string text) { }
        public static bool WebGLInputIsFocus(int id) { return false; }
        public static void WebGLInputDelete(int id) { }
        public static void WebGLInputForceBlur(int id) { }

#if WEBGLINPUT_TAB
        public static void WebGLInputEnableTabText(int id, bool enable) { }
#endif

#endif
    }

    public class WebGLInput : MonoBehaviour, IDeselectHandler, IComparable<WebGLInput>
    {
        public static event KeyboardEventHandler OnKeyboardDown;
        public static event KeyboardEventHandler OnKeyboardUp;

        static Dictionary<int, WebGLInput> instances = new Dictionary<int, WebGLInput>();
        public static string CanvasId { get; set; }

#if WEBGLINPUT_TAB
        public bool enableTabText = false;
#endif

        public static bool IsInstanceActive()
        {
            foreach (var inst in instances)
            {
                if (inst.Value.input != null && inst.Value.input.isFocused) return true;
            }

            return false;
        }

        static WebGLInput()
        {
            CanvasId = WebGLWindow.GetCanvasName();
            WebGLInputPlugin.WebGLInputInit();
        }
        public int Id { get { return id; } }
        internal int id = -1;
        public IInputField input { get; private set; }
        bool blurBlock = false;

        [TooltipAttribute("show input element on canvas. this will make you select text by drag.")]
        public bool showHtmlElement = false;


        [TooltipAttribute("This will add a \"autocomplete=\" tag to the HTML element. This helps for autofill of fields.")]
        public string autoComplete = "";

        private IInputField Setup()
        {
            if (GetComponent<InputField>()) return new WrappedInputField(GetComponent<InputField>());

            if (GetComponent<WebGLUIToolkitTextField>()) return new WrappedUIToolkit(GetComponent<WebGLUIToolkitTextField>());
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
            // for mobile platform
            if (Application.isMobilePlatform)
            {
                if (input.EnableMobileSupport)
                {
                    gameObject.AddComponent<WebGLInputMobile>();
                }
                else
                {
                    // when disable mobile input. disable self!
                    enabled = false;
                }
            }
            OnKeyboardDown += KeyboardDownHandler;
        }

        /// <summary>
        /// Get the element rect of input
        /// </summary>
        /// <returns></returns>
        RectInt GetElemetRect()
        {
            var rect = input.GetScreenCoordinates();
            // モバイルの場合、強制表示する
            if (showHtmlElement || Application.isMobilePlatform)
            {
                var x = (int)(rect.x);
                var y = (int)(Screen.height - (rect.y + rect.height));
                return new RectInt(x, y, (int)rect.width, (int)rect.height);
            }
            else
            {
                var x = (int)(rect.x);
                var y = (int)(Screen.height - (rect.y));
                return new RectInt(x, y, (int)rect.width, (int)1);
            }
        }
        /// <summary>
        /// 対象が選択されたとき
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect()
        {
            if (id != -1) throw new Exception("OnSelect : id != -1");

            var rect = GetElemetRect();
            bool isPassword = input.contentType == ContentType.Password;

            var fontSize = Mathf.Max(14, input.fontSize); // limit font size : 14 !!

            // モバイルの場合、強制表示する
            var isHidden = !(showHtmlElement || Application.isMobilePlatform);
            id = WebGLInputPlugin.WebGLInputCreate(WebGLInput.CanvasId, rect.x, rect.y, rect.width, rect.height, fontSize, input.text, input.placeholder, input.lineType != LineType.SingleLine, isPassword, isHidden, false, autoComplete);

            instances[id] = this;
            WebGLInputPlugin.WebGLInputEnterSubmit(id, input.lineType != LineType.MultiLineNewline);
            WebGLInputPlugin.WebGLInputOnFocus(id, OnFocus);
            WebGLInputPlugin.WebGLInputOnBlur(id, OnBlur);
            WebGLInputPlugin.WebGLInputOnValueChange(id, OnValueChange);
            WebGLInputPlugin.WebGLInputOnEditEnd(id, OnEditEnd);
            WebGLInputPlugin.WebGLInputOnKeyboardEvent(id, OnKeyboardEvent);
            WebGLInputPlugin.WebGLInputTab(id, OnTab);

            // default value : https://www.w3schools.com/tags/att_input_maxlength.asp
            WebGLInputPlugin.WebGLInputMaxLength(id, (input.characterLimit > 0) ? input.characterLimit : 524288);
            WebGLInputPlugin.WebGLInputFocus(id);
#if WEBGLINPUT_TAB
            WebGLInputPlugin.WebGLInputEnableTabText(id, enableTabText);
#endif
            if (input.OnFocusSelectAll)
            {
                WebGLInputPlugin.WebGLInputSetSelectionRange(id, 0, input.text.Length);
            }
            else
            {
                WebGLInputPlugin.WebGLInputSetSelectionRange(id, input.caretPosition, input.caretPosition);
            }

            WebGLWindow.OnBlurEvent += OnWindowBlur;
        }

        /// <summary>
        /// sync text from inputfield
        /// </summary>
        /// <param name="cursorIndex"></param>
        public void SyncText(int? cursorIndex = null)
        {
            if (!instances.ContainsKey(id)) return;

            var instance = instances[id];

            WebGLInputPlugin.WebGLInputText(id, instance.input.text);

            if (cursorIndex.HasValue)
            {
                WebGLInputPlugin.WebGLInputSetSelectionRange(id, cursorIndex.Value, cursorIndex.Value);
            }
        }

        private void OnWindowBlur()
        {
            blurBlock = true;
        }

        internal void DeactivateInputField()
        {
            if (!instances.ContainsKey(id)) return;

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
            Input.ResetInputAxes(); // Inputの状態リセット
            UnityEngine.WebGLInput.captureAllKeyboardInput = false;
#endif
        }

        [MonoPInvokeCallback(typeof(Action<int>))]
        static void OnBlur(int id)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.WebGLInput.captureAllKeyboardInput = true;
            Input.ResetInputAxes(); // Inputの状態リセット
#endif
            instances[id].StartCoroutine(Blur(id));
        }

        static IEnumerator Blur(int id)
        {
            yield return null;
            if (!instances.ContainsKey(id)) yield break;

            var block = instances[id].blurBlock;    // get blur block state
            instances[id].blurBlock = false;        // reset instalce block state
            if (block) yield break;                 // if block. break it!!
            // instances[id].DeactivateInputField();
        }

        [MonoPInvokeCallback(typeof(Action<int, string>))]
        static void OnValueChange(int id, string value)
        {
            if (!instances.ContainsKey(id)) return;

            var instance = instances[id];
            if (!instance.input.ReadOnly)
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

            var start = WebGLInputPlugin.WebGLInputSelectionStart(id);
            var end = WebGLInputPlugin.WebGLInputSelectionEnd(id);

            // InputField の ContentType による整形したテキストを HTML の input に再設定します
            if (value != instance.input.text)
            {
                // take the offset.when char remove from input.
                var offset = instance.input.text.Length - value.Length;

                WebGLInputPlugin.WebGLInputText(id, instance.input.text);
                // reset the input element selection range!!
                WebGLInputPlugin.WebGLInputSetSelectionRange(id, start + offset, end + offset);
            }

            // 選択方向によって設定します
            if (WebGLInputPlugin.WebGLInputSelectionDirection(id) == -1)
            {
                instance.input.selectionFocusPosition = start;
                instance.input.selectionAnchorPosition = end;
            }
            else
            {
                instance.input.selectionFocusPosition = end;
                instance.input.selectionAnchorPosition = start;
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

        [MonoPInvokeCallback(typeof(Action<int, int, string, int, int, int, int>))]
        static void OnKeyboardEvent(int id, int mode, string key, int code, int shiftKey, int ctrlKey, int altKey)
        {
            if (!instances.ContainsKey(id)) return;
            var instance = instances[id];

            // mode : keydown(1) keyup(2)
            var cb = mode switch
            {
                1 => OnKeyboardDown,
                2 => OnKeyboardUp,
                _ => default
            };

            if (key != null)
            {
                cb?.Invoke(instance, new KeyboardEvent(id, key, code, shiftKey != 0, ctrlKey != 0, altKey != 0));
            }
        }

        private bool TryGetKeyboardEventKeyString(KeyboardEvent keyboardEvent, out string eventKeyString)
        {
            eventKeyString = default;

            string eventModifiers = string.Empty;
            eventModifiers += keyboardEvent.ShiftKey ? "#" : "";
            eventModifiers += keyboardEvent.CtrlKey ? "^" : "";
            eventModifiers += keyboardEvent.AltKey ? "&" : "";

            string eventKey = string.Empty;
            // 必要なイベントだけ処理する
            switch (keyboardEvent.Key)
            {
                case "ArrowLeft":
                    eventKey = "left";
                    break;
                case "ArrowUp":
                    eventKey = "up";
                    break;
                case "ArrowRight":
                    eventKey = "right";
                    break;
                case "ArrowDown":
                    eventKey = "down";
                    break;
                case "Home":
                    eventKey = "home";
                    break;
                case "End":
                    eventKey = "end";
                    break;
                case "Shift":
                    eventKey = KeyCode.LeftShift.ToString();
                    break;
                case "Control":
                    eventKey = KeyCode.LeftControl.ToString();
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(eventModifiers))
            {
                // Ctrl + A : SelectAll Event
                if (keyboardEvent.Key == "a" || keyboardEvent.Key == "A")
                {
                    eventKey = KeyCode.A.ToString();
                }
            }

            if (!string.IsNullOrEmpty(eventKey))
            {
                eventKeyString = eventModifiers + eventKey;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void KeyboardDownHandler(WebGLInput instance, KeyboardEvent keyboardEvent)
        {
            if (instance == null || id != keyboardEvent.Id)
            {
                return;
            }

            if (TryGetKeyboardEventKeyString(keyboardEvent, out var eventKeyString))
            {
                instance.input.CreateKeyEvent(Event.KeyboardEvent(eventKeyString));
            }

            int startPos = instance.input.selectionAnchorPosition;
            int endPos = instance.input.selectionFocusPosition;

            if (startPos <= endPos)
            {
                WebGLInputPlugin.WebGLInputSetSelectionRange(keyboardEvent.Id, startPos, endPos);
            }
            else
            {
                WebGLInputPlugin.WebGLInputSetSelectionRange(keyboardEvent.Id, endPos, startPos);
            }
        }

        void Update()
        {
            if (input == null || !input.isFocused)
            {
                CheckOutFocus();
                return;
            }

            // 未登録の場合、選択する
            if (!instances.ContainsKey(id))
            {
                if (Application.isMobilePlatform)
                {
                    return;
                }
                else
                {
                    OnSelect();
                }
            }
            else if (!WebGLInputPlugin.WebGLInputIsFocus(id))
            {
                if (Application.isMobilePlatform)
                {
                    //input.DeactivateInputField();
                    return;
                }
                else
                {
                    // focus this id
                    WebGLInputPlugin.WebGLInputFocus(id);

                    int startPos = input.selectionAnchorPosition;
                    int endPos = input.selectionFocusPosition;

                    if (startPos <= endPos)
                    {
                        WebGLInputPlugin.WebGLInputSetSelectionRange(id, startPos, endPos);
                    }
                    else
                    {
                        WebGLInputPlugin.WebGLInputSetSelectionRange(id, endPos, startPos);
                    }
                }
            }

            input.Rebuild();
        }

        private void OnDestroy()
        {
            if (!instances.ContainsKey(id)) return;

#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.WebGLInput.captureAllKeyboardInput = true;
            Input.ResetInputAxes(); // Inputの状態リセット
#endif
            DeactivateInputField();
            OnKeyboardDown -= KeyboardDownHandler;
        }

        private void OnEnable()
        {
            WebGLInputTabFocus.Add(this);
        }
        private void OnDisable()
        {
            WebGLInputTabFocus.Remove(this);
            DeactivateInputField();
        }
        public int CompareTo(WebGLInput other)
        {
            var a = input.GetScreenCoordinates();
            var b = other.input.GetScreenCoordinates();
            var res = b.y.CompareTo(a.y);
            if (res == 0) res = a.x.CompareTo(b.x);
            return res;
        }

        private void CheckOutFocus()
        {
            if (!Application.isMobilePlatform) return;
            if (!instances.ContainsKey(id)) return;
            var current = EventSystem.current.currentSelectedGameObject;
            if (current != null) return;
            WebGLInputPlugin.WebGLInputForceBlur(id);   // Input ではないし、キーボードを閉じる
        }
        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            DeactivateInputField();
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
