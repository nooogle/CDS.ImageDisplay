using System;
using System.ComponentModel;

namespace CDS.ImageDisplay.Utils;


/// <summary>
/// Use this attribute on a class whose instance will become a JSON property that may require display
/// on a property grid with automatic expansion support. For list/combo boxes, turn off 
/// the FormattingEnabled property when displaying items with this attribute (see remarks)
/// </summary>
/// <remarks>
/// Turn off the FormattingEnabled property on any listbox or combobox that will use instances of
/// classes that use this class as a type converter; also make sure that the classes have appropriate
/// ToString operators. Reason: this class explicitly prevents conversion to a string via a 
/// type conversion, it only allows for conventional ToString conversion. When FormattingEnabled is
/// true a list box will try to perform automatic conversion to a string, this class will return False
/// to prevent the conversion, and an internal exception will be raised and caught; normally not a problem
/// but if the catch-exceptions-when-thrown is on it will be  real pain.
/// </remarks>
public class SerializableExpandableObjectConverter : ExpandableObjectConverter
{
    /// <summary>
    /// True if the conversion can take place
    /// </summary>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType != typeof(string) && base.CanConvertTo(context, destinationType);



    /// <summary>
    /// True if the conversion can take place
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType != typeof(string) && base.CanConvertFrom(context, sourceType);
}
