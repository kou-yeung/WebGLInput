using System;
using AOT;
using System.Runtime.InteropServices; // for DllImport
using UnityEngine;

namespace WebGLSupport
{
    static class WebGLDocumentPlugin
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void WebGLDocumentCopyToClipboard(string text);
#else
        public static void WebGLDocumentCopyToClipboard(string text) { }
#endif
    }

    public static class WebGLDocument
    {
        static void CopyToClipboard(string text)
        {
            WebGLDocumentPlugin.WebGLDocumentCopyToClipboard(text);
        }
    }
}
