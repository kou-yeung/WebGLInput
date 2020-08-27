var WebGLInput = {
    $instances: [],

    WebGLInputCreate: function (canvasId, x, y, width, height, fontsize, text, placeholder, isMultiLine, isPassword, isHidden) {
        var container = document.getElementById(Pointer_stringify(canvasId));
        var canvas = document.getElementsByTagName('canvas')[0];

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
        input.style.top = y + "px";
        input.style.left = x + "px";
        input.style.width = width + "px";
        input.style.height = height + "px";

		input.style.outlineWidth = 1 + 'px';
		input.style.opacity = isHidden?0:1;
		input.style.resize = 'none'; // for textarea
		input.style.padding = '0px 1px';
		input.style.cursor = "default";

		input.spellcheck = false;
		input.value = Pointer_stringify(text);
		input.placeholder = Pointer_stringify(placeholder);
		input.style.fontSize = fontsize + "px";
		//input.setSelectionRange(0, input.value.length);
		
		if(isPassword){
			input.type = 'password';
		}

        container.appendChild(input);
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
			var value = allocate(intArrayFromString(input.value), 'i8', ALLOC_NORMAL);
            Runtime.dynCall("vii", cb, [id,value]);
        };
    },
	WebGLInputOnEditEnd:function(id, cb){
        var input = instances[id];
        input.onchange = function () {
			var value = allocate(intArrayFromString(input.value), 'i8', ALLOC_NORMAL);
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
		input.value = Pointer_stringify(text);
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
}

autoAddDeps(WebGLInput, '$instances');
mergeInto(LibraryManager.library, WebGLInput);