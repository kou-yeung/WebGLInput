using UnityEngine;
using UnityEngine.UIElements;

namespace WebGLSupport
{
    /// <summary>
    /// Wrapper for UnityEngine.UIElements.TextField
    /// </summary>
    class WrappedUIToolkit : IInputField
    {
        TextField input;

        public bool ReadOnly { get { return input.isReadOnly; } }

        public string text
        {
            get { return input.value; }
            set { input.value = value; }
        }

        public string placeholder
        {
            get
            {
#if UNITY_2023_1_OR_NEWER
                return input.textEdition.placeholder;
#else
                return "";
#endif
            }
        }

        public int fontSize
        {
            /// MEMO : how to get the fontsize?
            get { return 20; }
        }

        public ContentType contentType
        {
            get
            {
                if (input.isPasswordField)
                {
                    return ContentType.Password;
                }

#if UNITY_2022_1_OR_NEWER
                var keyboardType = input.keyboardType;
#else
                var keyboardType = TouchScreenKeyboardType.Default;
#endif
                return keyboardType switch
                {
                    TouchScreenKeyboardType.Default => ContentType.Standard,
                    TouchScreenKeyboardType.ASCIICapable => ContentType.Alphanumeric,
                    TouchScreenKeyboardType.NumbersAndPunctuation => ContentType.Standard,
                    TouchScreenKeyboardType.URL => ContentType.Standard,
                    TouchScreenKeyboardType.NumberPad => ContentType.IntegerNumber,
                    TouchScreenKeyboardType.PhonePad => ContentType.Standard,
                    TouchScreenKeyboardType.NamePhonePad => ContentType.Standard,
                    TouchScreenKeyboardType.EmailAddress => ContentType.EmailAddress,
                    //TouchScreenKeyboardType.NintendoNetworkAccount => throw new System.NotImplementedException(),
                    TouchScreenKeyboardType.Social => ContentType.Standard,
                    TouchScreenKeyboardType.Search => ContentType.Standard,
                    TouchScreenKeyboardType.DecimalPad => ContentType.DecimalNumber,
                    TouchScreenKeyboardType.OneTimeCode => ContentType.Standard,
                    _ => ContentType.Standard,
                };
            }
        }

        public LineType lineType
        {
            get
            {
                return input.multiline ? LineType.MultiLineNewline : LineType.SingleLine;
            }
        }

        public int characterLimit
        {
            get { return input.maxLength; }
        }

        public int caretPosition
        {
            get { return input.cursorIndex; }
        }

        public bool isFocused
        {
            get
            {
                return true;
            }
        }

        public int selectionFocusPosition
        {
            get { return input.cursorIndex; }
#if UNITY_2022_1_OR_NEWER
            set { input.cursorIndex = value; }
#else
            set { input.SelectRange(value, input.selectIndex); }
#endif
        }

        public int selectionAnchorPosition
        {
            get { return input.selectIndex; }
#if UNITY_2022_1_OR_NEWER
            set { input.selectIndex = value; }
#else
            set { input.SelectRange(input.cursorIndex, value); }
#endif
        }

        public bool OnFocusSelectAll
        {
#if UNITY_2022_1_OR_NEWER
            get { return input.selectAllOnFocus || input.selectAllOnMouseUp; }
#else
            get { return true; }
#endif
        }

        public bool EnableMobileSupport
        {
            get
            {
                // return false to use unity mobile keyboard support
                return false;
            }
        }

        public WrappedUIToolkit(WebGLUIToolkitTextField input)
        {
            this.input = input.TextField;
        }

        public Rect GetScreenCoordinates()
        {
            var textInput = input.Q("unity-text-input");
            var rect = textInput.worldBound;
            return new Rect(rect.x, Screen.height - (rect.y + rect.height), rect.width, rect.height);
        }

        public void ActivateInputField()
        {
        }

        public void DeactivateInputField()
        {
            input.Blur();
        }

        public void Rebuild()
        {
        }
    }
}