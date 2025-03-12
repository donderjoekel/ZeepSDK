namespace ZeepSDK.UI.Elements
{
    public class FloatField : NumericInputBaseField<float>
    {
        public FloatField()
            : base()
        {
        }
        
        public FloatField(int maxLength, char maskChar)
            : base(maxLength, maskChar)
        {
        }
        
        public FloatField(string label)
            : base(label)
        {
        }
        
        public FloatField(string label, int maxLength, char maskChar)
            : base(label, maxLength, maskChar)
        {
        }
        
        public override float StringToValue(string str)
        {
            return float.TryParse(str, out float result) ? result : value;
        }
    }
}
