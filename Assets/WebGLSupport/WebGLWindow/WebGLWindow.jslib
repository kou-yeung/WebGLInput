var WebGLWindow = {
    WebGLWindowInit : function() {
        // use WebAssembly.Table : makeDynCall
        // when enable. dynCall is undefined
        if(typeof dynCall === "undefined")
        {
            // make Runtime.dynCall to undefined
            Runtime = { dynCall : undefined }
        }
        else
        {
            // Remove the `Runtime` object from "v1.37.27: 12/24/2017"
            // if Runtime not defined. create and add functon!!
            if(typeof Runtime === "undefined") Runtime = { dynCall : dynCall }
        }
    },
    WebGLWindowGetCanvasName: function() {
        var elements = document.getElementsByTagName('canvas');
        var returnStr = "";
        if(elements.length >= 1)
        {
            returnStr = elements[0].parentNode.id;
            // workaround : for WebGLTemplate:Minimal temp! body element not have id!
            if(returnStr == '')
            {
                returnStr = elements[0].parentNode.id = 'WebGLWindow:Canvas:ParentNode';
            }
        }
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    WebGLWindowOnFocus: function (cb) {
        window.addEventListener('focus', function () {
            (!!Runtime.dynCall) ? Runtime.dynCall("v", cb, []) : {{{ makeDynCall("v", "cb") }}}();
        });
    },
    WebGLWindowOnBlur: function (cb) {
        window.addEventListener('blur', function () {
            (!!Runtime.dynCall) ? Runtime.dynCall("v", cb, []) : {{{ makeDynCall("v", "cb") }}}();
        });
    },
    WebGLWindowOnResize: function(cb) {
        window.addEventListener('resize', function () {
            (!!Runtime.dynCall) ? Runtime.dynCall("v", cb, []) : {{{ makeDynCall("v", "cb") }}}();
        });
    },
    WebGLWindowInjectFullscreen : function () {
        document.makeFullscreen = function (id, keepAspectRatio) {
            // get fullscreen object
            var getFullScreenObject = function () {
                var doc = window.document;
                var objFullScreen = doc.fullscreenElement || doc.mozFullScreenElement || doc.webkitFullscreenElement || doc.msFullscreenElement;
                return (objFullScreen);
            }

            // handle fullscreen event
            var eventFullScreen = function (callback) {
                document.addEventListener("fullscreenchange", callback, false);
                document.addEventListener("webkitfullscreenchange", callback, false);
                document.addEventListener("mozfullscreenchange", callback, false);
                document.addEventListener("MSFullscreenChange", callback, false);
            }

            var removeEventFullScreen = function (callback) {
                document.removeEventListener("fullscreenchange", callback, false);
                document.removeEventListener("webkitfullscreenchange", callback, false);
                document.removeEventListener("mozfullscreenchange", callback, false);
                document.removeEventListener("MSFullscreenChange", callback, false);
            }

            var div = document.createElement("div");
            document.body.appendChild(div);

            // save canvas size to originSize
            var canvas = document.getElementsByTagName('canvas')[0];
            var originSize = 
            {
                width : canvas.style.width,
                height : canvas.style.height,
            };

            var fullscreenRoot = document.getElementById(id);

            // when build with minimal default template
            // the fullscreenRoot is <body>
            var isBody = fullscreenRoot.tagName.toLowerCase() == "body";
            if(isBody)
            {
                // swip the id to div
                div.id = fullscreenRoot.id;
                fullscreenRoot.id = "";
                // overwrite the fullscreen root
                fullscreenRoot = canvas;
            }

            var beforeParent = fullscreenRoot.parentNode;
            var beforeStyle = window.getComputedStyle(fullscreenRoot);
            var beforeWidth = parseInt(beforeStyle.width);
            var beforeHeight = parseInt(beforeStyle.height);

            // to keep element index after fullscreen
            var index = Array.from(beforeParent.children).findIndex(function (v) { return v == fullscreenRoot; });
            div.appendChild(fullscreenRoot);

            // recv fullscreen function
            var fullscreenFunc = function () {
                if (getFullScreenObject()) {
                    if (keepAspectRatio) {
                        var ratio = Math.min(window.screen.width / beforeWidth, window.screen.height / beforeHeight);
                        var width = Math.floor(beforeWidth * ratio);
                        var height = Math.floor(beforeHeight * ratio);

                        fullscreenRoot.style.width = width + 'px';
                        fullscreenRoot.style.height = height + 'px';
                    } else {
                        fullscreenRoot.style.width = window.screen.width + 'px';
                        fullscreenRoot.style.height = window.screen.height + 'px';
                    }

                    // make canvas size 100% to fix screen size
                    canvas.style.width = "100%";
                    canvas.style.height = "100%";

                } else {
                    fullscreenRoot.style.width = beforeWidth + 'px';
                    fullscreenRoot.style.height = beforeHeight + 'px';
                    beforeParent.insertBefore(fullscreenRoot, Array.from(beforeParent.children)[index]);

                    if(isBody)
                    {
                        beforeParent.id = div.id;
                    }

                    div.parentNode.removeChild(div);

                    // set canvas size to origin size
                    canvas.style.width = originSize.width;
                    canvas.style.height = originSize.height;

                    // remove this function
                    removeEventFullScreen(fullscreenFunc);
                }
            }

            // listener fullscreen event
            eventFullScreen(fullscreenFunc);

            if (div.mozRequestFullScreen) div.mozRequestFullScreen();
            else if (div.webkitRequestFullScreen) div.webkitRequestFullScreen();
            else if (div.msRequestFullscreen) div.msRequestFullscreen();
            else if (div.requestFullscreen) div.requestFullscreen();
        }
    },
    MakeFullscreen : function (str) {
        document.makeFullscreen(UTF8ToString(str));
    },
    ExitFullscreen : function() {
        // get fullscreen object
        var doc = window.document;
        var objFullScreen = doc.fullscreenElement || doc.mozFullScreenElement || doc.webkitFullscreenElement || doc.msFullscreenElement;

        if (objFullScreen)
        {
            if (document.exitFullscreen) document.exitFullscreen();
            else if (document.msExitFullscreen) document.msExitFullscreen();
            else if (document.mozCancelFullScreen) document.mozCancelFullScreen();
            else if (document.webkitExitFullscreen) document.webkitExitFullscreen();
        }
    },
    IsFullscreen : function() {
        // get fullscreen object
        var doc = window.document;
        var objFullScreen = doc.fullscreenElement || doc.mozFullScreenElement || doc.webkitFullscreenElement || doc.msFullscreenElement;

        // check full screen elemenet is not null!
        return objFullScreen != null;
    },
}

mergeInto(LibraryManager.library, WebGLWindow);
