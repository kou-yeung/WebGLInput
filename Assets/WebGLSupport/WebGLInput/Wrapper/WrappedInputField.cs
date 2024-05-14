using UnityEngine;
using UnityEngine.UI;
using WebGLSupport.Detail;

namespace WebGLSupport
{
    /// <summary>
    /// Wrapper for UnityEngine.UI.InputField
    /// </summary>
    class WrappedInputField : IInputField
    {
        InputField input;
        RebuildChecker checker;

        public bool ReadOnly { get { return input.readOnly; } }

        public string text
        {
            get { return input.text; }
            set { input.text = value; }
        }

        public string placeholder
        {
            get
            {
                if (!input.placeholder) return "";
                var text = input.placeholder.GetComponent<Text>();
                return text ? text.text : "";
            }
        }

        public int fontSize
        {
            get { return input.textComponent.fontSize; }
        }

        public ContentType contentType
        {
            get { return (ContentType)input.contentType; }
        }

        public LineType lineType
        {
            get { return (LineType)input.lineType; }
        }

        public int characterLimit
        {
            get { return input.characterLimit; }
        }

        public int caretPosition
        {
            get { return input.caretPosition; }
        }

        public bool isFocused
        {
            get { return input.isFocused; }
        }

        public int selectionFocusPosition
        {
            get { return input.selectionFocusPosition; }
            set { input.selectionFocusPosition = value; }
        }

        public int selectionAnchorPosition
        {
            get { return input.selectionAnchorPosition; }
            set { input.selectionAnchorPosition = value; }
        }

        public bool OnFocusSelectAll
        {
            get { return true; }
        }

        public bool EnableMobileSupport
        {
            get
            {
                // return false to use unity mobile keyboard support
                return false;
            }
        }

        public WrappedInputField(InputField input)
        {
            this.input = input;
            checker = new RebuildChecker(this);
        }

        public void ActivateInputField()
        {
            input.ActivateInputField();
        }

        public void DeactivateInputField()
        {
            input.DeactivateInputField();
        }

        public void Rebuild()
        {
            if (checker.NeedRebuild())
            {
                input.textComponent.SetAllDirty();
                input.Rebuild(CanvasUpdate.LatePreRender);
            }
        }

        public Rect GetScreenCoordinates()
        {
            return Support.GetScreenCoordinates(input.GetComponent<RectTransform>());
        }
    }
}