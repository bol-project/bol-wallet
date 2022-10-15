using BolWallet.Converters;
using DevExpress.Maui.Editors;
using System.Globalization;

namespace BolWallet.Controls;

public class ExtendedTextEdit : TextEdit
{
    public ExtendedTextEdit()
    {
        _labelTemplateConverter = new StateColorConverter();
        _endIconConverter = new EndIconConverter();
    }

    private readonly StateColorConverter _labelTemplateConverter;
    private readonly EndIconConverter _endIconConverter;

    public static readonly BindableProperty StateProperty =
        BindableProperty.Create(
            nameof(State),
            typeof(PropertyState),
            typeof(ExtendedTextEdit));

    public PropertyState State
    {
        get { return (PropertyState)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == StateProperty.PropertyName)
        {
            var color = (Color)_labelTemplateConverter.Convert(State, typeof(Color), null, CultureInfo.CurrentCulture);
            EndIconColor = color;
            FocusedBorderColor = color;
            FocusedLabelColor = color;
            EndIcon = new FontImageSource()
            {
                FontFamily = "MaterialIconsRegular",
                Glyph = (string)_endIconConverter.Convert(State, typeof(string), null, CultureInfo.CurrentCulture),
                Size = 24
            };
        }
    }
}