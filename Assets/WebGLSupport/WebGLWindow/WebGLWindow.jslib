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
	WebGLWindowInjectFullscreen : function () {
        document.makeFullscreen = function (id, keepAspectRatio) {
			var getFullScreenObject = function () {
				var doc = window.document;
				var objFullScreen = doc.fullscreenElement || doc.mozFullScreenElement || doc.webkitFullscreenElement || doc.msFullscreenElement;
				return (objFullScreen);
			}
			var eventFullScreen = function (callback) {
                document.addEventListener("fullscreenchange", callback, false);
                document.addEventListener("webkitfullscreenchange", callback, false);
                document.addEventListener("mozfullscreenchange", callback, false);
                document.addEventListener("MSFullscreenChange", callback, false);
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

            eventFullScreen(function (e) {
                if (getFullScreenObject()) {
                    if (keepAspectRatio) {
                        var ratio = Math.min(window.screen.width / beforeWidth, window.screen.height / beforeHeight);
                        var width = Math.floor(beforeWidth * ratio);
                        var height = Math.floor(beforeHeight * ratio);

                        canvas.style.width = width;
                        canvas.style.height = height;
                    } else {
                        canvas.style.width = window.screen.width;
                        canvas.style.height = window.screen.height;
                    }

                } else {
                    beforeParent.insertBefore(canvas, Array.from(beforeParent.children)[index]);
                    div.parentNode.removeChild(div);

                    canvas.style.width = beforeWidth;
                    canvas.style.height = beforeHeight;
                }
            });

            if (div.requestFullscreen) div.requestFullscreen();
            else if (div.mozRequestFullScreen) div.mozRequestFullScreen();
            else if (div.webkitRequestFullScreen) div.webkitRequestFullScreen();
            else if (div.msRequestFullscreen) div.msRequestFullscreen();
		}
	},
}

mergeInto(LibraryManager.library, WebGLWindow);