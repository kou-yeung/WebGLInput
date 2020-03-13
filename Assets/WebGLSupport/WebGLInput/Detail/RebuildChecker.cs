using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebGLSupport.Detail
{
    public class RebuildChecker
    {
        IInputField input;

        string beforeString;
        int beforeCaretPosition;
        int beforeSelectionFocusPosition;
        int beforeSelectionAnchorPosition;
        //Vector2 anchoredPosition;

        public RebuildChecker(IInputField input)
        {
            this.input = input;
        }

        public bool NeedRebuild(bool debug = false)
        {
            var res = false;

            // any not same
            if (beforeString != input.text)
            {
                if(debug) Debug.Log(string.Format("beforeString : {0} != {1}", beforeString, input.text));
                beforeString = input.text;
                res = true;
            }

            if (beforeCaretPosition != input.caretPosition)
            {
                if (debug) Debug.Log(string.Format("beforeCaretPosition : {0} != {1}", beforeCaretPosition, input.caretPosition));
                beforeCaretPosition = input.caretPosition;
                res = true;
            }

            if (beforeSelectionFocusPosition != input.selectionFocusPosition)
            {
                if (debug) Debug.Log(string.Format("beforeSelectionFocusPosition : {0} != {1}", beforeSelectionFocusPosition, input.selectionFocusPosition));
                beforeSelectionFocusPosition = input.selectionFocusPosition;
                res = true;
            }

            if (beforeSelectionAnchorPosition != input.selectionAnchorPosition)
            {
                if (debug) Debug.Log(string.Format("beforeSelectionAnchorPosition : {0} != {1}", beforeSelectionAnchorPosition, input.selectionAnchorPosition));
                beforeSelectionAnchorPosition = input.selectionAnchorPosition;
                res = true;
            }

            //if (anchoredPosition != input.TextComponentRectTransform().anchoredPosition)
            //{
            //    if (debug) Debug.Log(string.Format("anchoredPosition : {0} != {1}", anchoredPosition, input.TextComponentRectTransform().anchoredPosition));
            //    anchoredPosition = input.TextComponentRectTransform().anchoredPosition;
            //    res = true;
            //}
            return res;
        }
    }
}
