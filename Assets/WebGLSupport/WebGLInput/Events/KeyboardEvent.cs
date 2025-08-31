namespace WebGLSupport
{
    public delegate void KeyboardEventHandler(WebGLInput input, KeyboardEvent keyboardEvent);
    public sealed class KeyboardEvent
    {
        public int Id { get; }
        public string Key { get; }
        public int Code { get; }
        public bool ShiftKey { get; }
        public bool CtrlKey { get; }
        public bool AltKey { get; }
        public KeyboardEvent(int id, string key, int code, bool shiftKey, bool ctrlKey, bool altKey)
        {
            Id = id;
            Key = key;
            Code = code;
            ShiftKey = shiftKey;
            CtrlKey = ctrlKey;
            AltKey = altKey;
        }
    }
}
