using UnityEngine;

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
        string placeholder { get; }
        int characterLimit { get; }
        int caretPosition { get; }
        bool isFocused { get; }
        int selectionFocusPosition { get; set; }
        int selectionAnchorPosition { get; set; }
        bool ReadOnly { get; }
        bool OnFocusSelectAll { get; }
        bool EnableMobileSupport { get; }

        Rect GetScreenCoordinates();

        void ActivateInputField();
        void DeactivateInputField();
        void Rebuild();
    }
}
