var WebGLInputMobile = {
    $instances: [],

    WebGLInputMobileRegister: function (touchend) {
        var id = instances.push(null) - 1;

        document.body.addEventListener("touchend", function () {
            document.body.removeEventListener("touchend", arguments.callee);
            Runtime.dynCall("vi", touchend, [id]);
        });

        return id;
    },
    WebGLInputMobileOnFocusOut: function (id, focusout) {
        document.body.addEventListener("focusout", function () {
            document.body.removeEventListener("focusout", arguments.callee);
            Runtime.dynCall("vi", focusout, [id]);
        });
    },
}

autoAddDeps(WebGLInputMobile, '$instances');
mergeInto(LibraryManager.library, WebGLInputMobile);