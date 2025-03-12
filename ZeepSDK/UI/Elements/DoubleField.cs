namespace ZeepSDK.UI.Elements;

public class DoubleField : NumericInputBaseField<double>
{
    public DoubleField()
        : base()
    {
    }
        
    public DoubleField(int maxLength, char maskChar)
        : base(maxLength, maskChar)
    {
    }
        
    public DoubleField(string label)
        : base(label)
    {
    }
        
    public DoubleField(string label, int maxLength, char maskChar)
        : base(label, maxLength, maskChar)
    {
    }
        
    public override double StringToValue(string str)
    {
        return double.TryParse(str, out double result) ? result : value;
    }
}
