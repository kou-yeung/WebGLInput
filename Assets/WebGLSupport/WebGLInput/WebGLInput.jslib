var WebGLInput = {
    $instances: [],

    WebGLInputCreate: function (canvasId, x, y, width, height, fontsize, text, isMultiLine, isPassword) {
        var container = document.getElementById(Pointer_stringify(canvasId));
		
		var canvas = document.getElementsByTagName('canvas')[0];
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
        input.style.backgroundColor = '#00000000';
        input.style.color = '#00000000';
        input.style.outline = "none";
		input.style.border = "hidden";
		input.style.opacity = 0;
		input.style.cursor = "default";
		input.spellcheck = false;
		input.value = Pointer_stringify(text);
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
                Runtime.dynCall("vii", cb, [id, e.shiftKey ? -1 : 1]);
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
}

autoAddDeps(WebGLInput, '$instances');
mergeInto(LibraryManager.library, WebGLInput);