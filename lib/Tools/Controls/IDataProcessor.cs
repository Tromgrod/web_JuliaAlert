namespace LIB.Tools.Controls
{
    using LIB.AdvancedProperties;
using LIB.Tools.BO;

    public interface IDataProcessor
    {
        object SetValue(object value, AdvancedProperty property, ItemBase BOItem, bool ReadOnly = false, DisplayMode mode = DisplayMode.Simple);

        object GetValue(AdvancedProperty property,string prefix="", DisplayMode mode = DisplayMode.Simple);

        string ToString(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print);
    }
}
