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
        public static extern bool WebGLDocumentCopyToClipboard(string text);
#else
        public static bool WebGLDocumentCopyToClipboard(string text) { return false; }
#endif
    }

    public static class WebGLDocument
    {
        public static bool CopyToClipboard(string text)
        {
            return WebGLDocumentPlugin.WebGLDocumentCopyToClipboard(text);
        }
    }
}
