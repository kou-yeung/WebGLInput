var WebGLWindow = {
    WebGLWindowOnFocus: function (cb) {
        window.addEventListener('focus', function () {
            Runtime.dynCall("v", cb, []);
        });
    },
    WebGLWindowOnBlur: function (cb) {
        window.addEventListener('blur', function () {
            Runtime.dynCall("v", cb, []);
        });
    },
}

mergeInto(LibraryManager.library, WebGLWindow);