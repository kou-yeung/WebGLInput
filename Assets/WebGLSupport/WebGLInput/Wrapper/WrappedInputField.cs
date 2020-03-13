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

        public WrappedInputField(InputField input)
        {
            this.input = input;
            checker = new RebuildChecker(this);
        }

        public RectTransform RectTransform()
        {
            return input.textComponent.GetComponent<RectTransform>();
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
                input.Rebuild(CanvasUpdate.LatePreRender);
                input.textComponent.SetAllDirty();
            }
        }
    }
}