using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace WebGLSupport
{
    public class Postprocessor
    {
        const string MenuPath = "Assets/WebGLSupport/OverwriteFullscreenButton";

#if UNITY_2021_1_OR_NEWER
        static readonly bool supportedPostprocessor = true;
        static readonly string defaultFullscreenFunc = "unityInstance.SetFullscreen(1);";
        static readonly string fullscreenNode = "unity-container";
#else
        static readonly bool supportedPostprocessor = false;
        static readonly string defaultFullscreenFunc = "";
        static readonly string fullscreenNode = "";
#endif

        private static bool IsEnable => PlayerPrefs.GetInt(MenuPath, 1) == 1;

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.WebGL) return;
            if (!supportedPostprocessor) return;
            if (!IsEnable) return;

            var path = Path.Combine(pathToBuiltProject, "index.html");
            if (!File.Exists(path)) return;

            var html = File.ReadAllText(path);

            // check node is exist
            if (html.Contains(fullscreenNode))
            {
                html = html.Replace(defaultFullscreenFunc, $"document.makeFullscreen('{fullscreenNode}');");
                File.WriteAllText(path, html);
            }
        }

        [MenuItem(MenuPath)]
        public static void OverwriteDefaultFullscreenButton()
        {
            var flag = !Menu.GetChecked(MenuPath);
            Menu.SetChecked(MenuPath, flag);
            PlayerPrefs.SetInt(MenuPath, flag ? 1 : 0);
        }

        [MenuItem(MenuPath, validate = true)]
        private static bool OverwriteDefaultFullscreenButtonValidator()
        {
            Menu.SetChecked(MenuPath, IsEnable);
            return true;
        }
    }
}
