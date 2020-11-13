using System;
using AOT;
using System.Runtime.InteropServices; // for DllImport
using UnityEngine;

namespace WebGLSupport
{
    static class WebGLWindowPlugin
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void WebGLWindowOnFocus(Action cb);

        [DllImport("__Internal")]
        public static extern void WebGLWindowOnBlur(Action cb);

        [DllImport("__Internal")]
        public static extern void WebGLWindowInjectFullscreen();

        [DllImport("__Internal")]
        public static extern void WebGLWindowSetFullscreen(string element, bool enable);

        [DllImport("__Internal")]
        public static extern bool WebGLWindowGetFullscreen();

#else
        public static void WebGLWindowOnFocus(Action cb) { }
        public static void WebGLWindowOnBlur(Action cb) { }
        public static void WebGLWindowInjectFullscreen() { }
        public static void WebGLWindowSetFullscreen(string element, bool enable) { Screen.fullScreen = enable; }
        public static bool WebGLWindowGetFullscreen() { return Screen.fullScreen; }
#endif

    }

    public static class WebGLWindow
    {
        public static bool Focus { get; private set; }
        public static event Action OnFocusEvent = () => { };
        public static event Action OnBlurEvent = () => { };

        public static bool fullScreen
        {
            get
            {
                return WebGLWindowPlugin.WebGLWindowGetFullscreen();
            }
            set
            {
                WebGLWindowPlugin.WebGLWindowSetFullscreen(WebGLInput.CanvasId, value);
            }
        }

        static void Init()
        {
            Focus = true;
            WebGLWindowPlugin.WebGLWindowOnFocus(OnWindowFocus);
            WebGLWindowPlugin.WebGLWindowOnBlur(OnWindowBlur);
            WebGLWindowPlugin.WebGLWindowInjectFullscreen();
        }

        [MonoPInvokeCallback(typeof(Action))]
        static void OnWindowFocus()
        {
            Focus = true;
            OnFocusEvent();
        }

        [MonoPInvokeCallback(typeof(Action))]
        static void OnWindowBlur()
        {
            Focus = false;
            OnBlurEvent();
        }

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInitializeOnLoadMethod()
        {
            Init();
        }
    }
}
