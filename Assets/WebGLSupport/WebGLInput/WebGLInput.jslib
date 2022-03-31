var WebGLInput = {
    $instances: [],
	WebGLInputInit : function() {
		// Remove the `Runtime` object from "v1.37.27: 12/24/2017"
		// if Runtime not defined. create and add functon!!
		if(typeof Runtime === "undefined") Runtime = { dynCall : dynCall }
	},
    WebGLInputCreate: function (canvasId, x, y, width, height, fontsize, text, placeholder, isMultiLine, isPassword, isHidden, isMobile) {

        var container = document.getElementById(UTF8ToString(canvasId));
        var canvas = container.getElementsByTagName('canvas')[0];

        // if container is null and have canvas
        if (!container && canvas)
        {
            // set the container to canvas.parentNode
            container = canvas.parentNode;
        }

		if(canvas)
		{
			var scaleX = container.offsetWidth / canvas.width;
			var scaleY = container.offsetHeight / canvas.height;

			if(scaleX && scaleY)
			{
				x *= scaleX;
				width *= scaleX;
				y *= scaleY;
				height *= scaleY;
			}
		}

        var input = document.createElement(isMultiLine?"textarea":"input");
        input.style.position = "absolute";

		if(isMobile) {
			input.style.bottom = 1 + "vh";
			input.style.left = 5 + "vw";
			input.style.width = 90 + "vw";
			input.style.height = (isMultiLine? 18 : 10) + "vh";
			input.style.fontSize = 5 + "vh";
			input.style.borderWidth = 5 + "px";
			input.style.borderColor = "#000000";
		} else {
			input.style.top = y + "px";
			input.style.left = x + "px";
			input.style.width = width + "px";
			input.style.height = height + "px";
			input.style.fontSize = fontsize + "px";
		}

		input.style.outlineWidth = 1 + 'px';
		input.style.opacity = isHidden?0:1;
		input.style.resize = 'none'; // for textarea
		input.style.padding = '0px 1px';
		input.style.cursor = "default";
		input.style.touchAction = 'manipulation'; // for mobile

		input.spellcheck = false;
		input.value = UTF8ToString(text);
		input.placeholder = UTF8ToString(placeholder);
		
		if(isPassword){
			input.type = 'password';
		}

		if(isMobile) {
			document.body.appendChild(input);
		} else {
	        container.appendChild(input);
		}
        return instances.push(input) - 1;
    },
	WebGLInputEnterSubmit: function(id, falg){
		var input = instances[id];
		// for enter key
		input.addEventListener('keydown', function(e) {
			if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
				if(falg)
				{
					e.preventDefault();
					input.blur();
				}
			}
		});
    },
	WebGLInputTab:function(id, cb) {
		var input = instances[id];
		// for tab key
        input.addEventListener('keydown', function (e) {
            if ((e.which && e.which === 9) || (e.keyCode && e.keyCode === 9)) {
                e.preventDefault();

				// if enable tab text
				if(input.enableTabText){
                    var val = input.value;
                    var start = input.selectionStart;
                    var end = input.selectionEnd;
                    input.value = val.substr(0, start) + '\t' + val.substr(end, val.length);
                    input.setSelectionRange(start + 1, start + 1);
                    input.oninput();	// call oninput to exe ValueChange function!!
				} else {
				    Runtime.dynCall("vii", cb, [id, e.shiftKey ? -1 : 1]);
				}
            }
		});
	},
	WebGLInputFocus: function(id){
		var input = instances[id];
		input.focus();
    },
    WebGLInputOnFocus: function (id, cb) {
        var input = instances[id];
        input.onfocus = function () {
            Runtime.dynCall("vi", cb, [id]);
        };
    },
    WebGLInputOnBlur: function (id, cb) {
        var input = instances[id];
        input.onblur = function () {
            Runtime.dynCall("vi", cb, [id]);
        };
    },
	WebGLInputIsFocus: function (id) {
		return instances[id] === document.activeElement;
	},
	WebGLInputOnValueChange:function(id, cb){
        var input = instances[id];
        input.oninput = function () {
			var intArray = intArrayFromString(input.value);
            var value = (allocate.length <= 2) ? allocate(intArray, ALLOC_NORMAL):allocate(intArray, 'i8', ALLOC_NORMAL);
            Runtime.dynCall("vii", cb, [id,value]);
        };
    },
	WebGLInputOnEditEnd:function(id, cb){
        var input = instances[id];
        input.onchange = function () {
			var intArray = intArrayFromString(input.value);
            var value = (allocate.length <= 2) ? allocate(intArray, ALLOC_NORMAL):allocate(intArray, 'i8', ALLOC_NORMAL);
            Runtime.dynCall("vii", cb, [id,value]);
        };
    },
	WebGLInputSelectionStart:function(id){
        var input = instances[id];
		return input.selectionStart;
	},
	WebGLInputSelectionEnd:function(id){
        var input = instances[id];
		return input.selectionEnd;
	},
	WebGLInputSelectionDirection:function(id){
        var input = instances[id];
		return (input.selectionDirection == "backward")?-1:1;
	},
	WebGLInputSetSelectionRange:function(id, start, end){
		var input = instances[id];
		input.setSelectionRange(start, end);
	},
	WebGLInputMaxLength:function(id, maxlength){
        var input = instances[id];
		input.maxLength = maxlength;
	},
	WebGLInputText:function(id, text){
        var input = instances[id];
		input.value = UTF8ToString(text);
	},
	WebGLInputDelete:function(id){
        var input = instances[id];
        input.parentNode.removeChild(input);
        instances[id] = null;
    },
	WebGLInputEnableTabText:function(id, enable) {
        var input = instances[id];
		input.enableTabText = enable;
	},
	WebGLInputForceBlur:function(id) {
        var input = instances[id];
		input.blur();
	},
}

autoAddDeps(WebGLInput, '$instances');
mergeInto(LibraryManager.library, WebGLInput);
