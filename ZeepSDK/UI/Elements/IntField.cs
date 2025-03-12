namespace ZeepSDK.UI.Elements
{
    public class IntField : NumericInputBaseField<int>
    {
        public IntField()
            : base()
        {
        }
        
        public IntField(int maxLength, char maskChar)
            : base(maxLength, maskChar)
        {
        }
        
        public IntField(string label)
            : base(label)
        {
        }
        
        public IntField(string label, int maxLength, char maskChar)
            : base(label, maxLength, maskChar)
        {
        }
        
        public override int StringToValue(string str)
        {
            return int.TryParse(str, out int result) ? result : value;
        }
    }
}
