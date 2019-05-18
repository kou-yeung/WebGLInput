using UnityEngine;
using UnityEngine.UI;

namespace WebGLSupport
{
    public enum ContentType
    {
        Standard = 0,
        Autocorrected = 1,
        IntegerNumber = 2,
        DecimalNumber = 3,
        Alphanumeric = 4,
        Name = 5,
        EmailAddress = 6,
        Password = 7,
        Pin = 8,
        Custom = 9
    }
    public enum LineType
    {
        SingleLine = 0,
        MultiLineSubmit = 1,
        MultiLineNewline = 2
    }
    public interface IInputField
    {
        ContentType contentType { get; }
        LineType lineType { get; }
        int fontSize { get; }
        string text { get; set; }
        int characterLimit { get; }
        int caretPosition { get; }
        bool isFocused { get; }
        int selectionFocusPosition { set; }
        int selectionAnchorPosition { set; }

        RectTransform TextComponentRectTransform();
        void ActivateInputField();
        void DeactivateInputField();
        void Rebuild(CanvasUpdate update);
        void SetAllDirty();
    }
}
