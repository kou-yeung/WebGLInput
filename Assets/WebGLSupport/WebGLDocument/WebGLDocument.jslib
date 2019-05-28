var WebGLDocument = {
    WebGLDocumentCopyToClipboard: function (text)
	{
		// ref: https://qiita.com/simiraaaa/items/2e7478d72f365aa48356
		
		// create div
		var tmp = document.createElement("div");

		// create select tag and set "user-select". (if "none", it can not copy!!)
		var pre = document.createElement('pre');
		pre.style.webkitUserSelect = 'auto';
		pre.style.userSelect = 'auto';

		// add pre to tmp and set textContent
		tmp.appendChild(pre).textContent = Pointer_stringify(text);

		// move outside of screen
		var s = tmp.style;
		s.position = 'fixed';
		s.right = '200%';

		// add tmp to body and select all.
		document.body.appendChild(tmp);
		document.getSelection().selectAllChildren(tmp);

		// execCommand
		var result = document.execCommand("copy");

		// remove tmp
		document.body.removeChild(tmp);

		return result;
	},
}

mergeInto(LibraryManager.library, WebGLDocument);