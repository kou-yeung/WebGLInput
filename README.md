# WebGLInput

IME for Unity WebGL ( Support TextMesh Pro from Unity2018.2 )

support “copy and paste”

support "tab" and "shift+tab" change focus to other InputField

support mobile. (Experiment)

support UI Toolkit. (Experiment) (Support from Unity2022)

# DEMO

Build with Unity's WebGL Template ( Unity2023.2.2f1 )

[Default](https://kou-yeung.github.io/webgl_input_sample/default/index.html)

[Minimal](https://kou-yeung.github.io/webgl_input_sample/minimal/index.html)

[PWA](https://kou-yeung.github.io/webgl_input_sample/pwa/index.html)

# How to use
1.download [WebGLSupport.unitypackage](https://github.com/kou-yeung/WebGLInput/releases) and import to project

2.add "WebGLInput" Component to InputField GameObject

3.build and run!!

no need to setting anything.

# insert \t use tab key instead of changing focus
Add "WEBGLINPUT_TAB" to Scripting Define Symbols.

and check "Enable Tab Text" at WebGLInput. 

# fullscreen support

Call
```
WebGLSupport.WebGLWindow.SwitchFullscreen();
```
to switch fullscreen mode.

# Experimental : Support UI Toolkit
sample code
```
[SerializeField] UIDocument uiDocument;
public void Start()
{
    // find all TextField element
    uiDocument.rootVisualElement.Query<TextField>().ForEach(v =>
    {
        // add WebGLInputManipulator to TextField
        v.AddManipulator(new WebGLSupport.WebGLInputManipulator());
    });
}
```
