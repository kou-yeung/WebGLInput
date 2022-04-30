var WebGLWindow = {
	WebGLWindowInit : function() {
		// Remove the `Runtime` object from "v1.37.27: 12/24/2017"
		// if Runtime not defined. create and add functon!!
		if(typeof Runtime === "undefined") Runtime = { dynCall : dynCall }
	},
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
	WebGLWindowOnResize: function(cb) {
        window.addEventListener('resize', function () {
            Runtime.dynCall("v", cb, []);
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

            var canvas = document.getElementById(id);
            var beforeParent = canvas.parentNode;
            var beforeStyle = window.getComputedStyle(canvas);
            var beforeWidth = parseInt(beforeStyle.width);
            var beforeHeight = parseInt(beforeStyle.height);

            // to keep element index after fullscreen
            var index = Array.from(beforeParent.children).findIndex(function (v) { return v == canvas; });
            div.appendChild(canvas);

            // recv fullscreen function
            var fullscreenFunc = function () {
                if (getFullScreenObject()) {
                    if (keepAspectRatio) {
                        var ratio = Math.min(window.screen.width / beforeWidth, window.screen.height / beforeHeight);
                        var width = Math.floor(beforeWidth * ratio);
                        var height = Math.floor(beforeHeight * ratio);

                        canvas.style.width = width + 'px';
                        canvas.style.height = height + 'px';;
                    } else {
                        canvas.style.width = window.screen.width + 'px';;
                        canvas.style.height = window.screen.height + 'px';;
                    }

                } else {
					canvas.style.width = beforeWidth + 'px';;
                    canvas.style.height = beforeHeight + 'px';;
                    beforeParent.insertBefore(canvas, Array.from(beforeParent.children)[index]);

                    div.parentNode.removeChild(div);

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
}

mergeInto(LibraryManager.library, WebGLWindow);