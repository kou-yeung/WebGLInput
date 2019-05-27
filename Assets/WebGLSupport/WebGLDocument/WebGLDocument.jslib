var WebGLDocument = {
    WebGLDocumentCopyToClipboard: function (text)
	{
		var tmp = document.createElement("div");

		// 選択用のタグ生成
		var pre = document.createElement('pre');

		// 親要素のCSSで user-select: none だとコピーできないので書き換える
		pre.style.webkitUserSelect = 'auto';
		pre.style.userSelect = 'auto';

		tmp.appendChild(pre).textContent = Pointer_stringify(text);

		// 要素を画面外へ
		var s = tmp.style;
		s.position = 'fixed';
		s.right = '200%';

		// body に追加
		document.body.appendChild(tmp);
		// 要素を選択
		document.getSelection().selectAllChildren(tmp);

		// クリップボードにコピー
		var result = document.execCommand("copy");

		// 要素削除
		document.body.removeChild(tmp);
		
		console.log(result);

		return result;
	},
}

mergeInto(LibraryManager.library, WebGLDocument);