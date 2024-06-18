using BolWallet.Converters;
using Microsoft.Maui.Controls;
using System.Dynamic;
using System.Globalization;

namespace BolWallet.Controls;

public partial class ExtendedComboBox : ContentView
{
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(ExtendedComboBox));
    public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(nameof(HasError), typeof(bool), typeof(ExtendedComboBox));
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ExtendedComboBox));
    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ExtendedComboBox));
    public static readonly BindableProperty EndIconColorProperty = BindableProperty.Create(nameof(EndIconColor), typeof(Color), typeof(ExtendedComboBox));
    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(ExtendedComboBox));
    public static readonly BindableProperty FocusedBorderColorProperty = BindableProperty.Create(nameof(FocusedBorderColor), typeof(Color), typeof(ExtendedComboBox));
    public static readonly BindableProperty FocusedLabelColorProperty = BindableProperty.Create(nameof(FocusedLabelColor), typeof(Color), typeof(ExtendedComboBox));
    public static readonly BindableProperty EndIconProperty = BindableProperty.Create(nameof(EndIcon), typeof(FontImageSource), typeof(ExtendedComboBox));
    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(PropertyState), typeof(ExtendedComboBox));
    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExtendedComboBox));
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(System.Collections.IList), typeof(ExtendedComboBox));
    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(ExtendedComboBox));
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ExtendedComboBox));
    public static readonly BindableProperty ItemTextProperty = BindableProperty.Create(nameof(ItemText), typeof(string), typeof(ExtendedComboBox));

    private readonly StateColorConverter _labelTemplateConverter;
    private readonly EndIconConverter _endIconConverter;

    int _placeholderFontSize = 18;
    int _titleFontSize =
#if MACCATALYST
    14;
#elif WINDOWS
    14;
#else
    12;
#endif
    int _topMargin = -20;
    static async void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as ExtendedComboBox;
        if (!control.ePicker.IsFocused)
        {
            if (!string.IsNullOrEmpty((string)newValue))
            {
                await control.TransitionToTitle(false);
            }
            else
            {
                await control.TransitionToPlaceholder(false);
            }
        }
    }

    public ExtendedComboBox()
	{
		InitializeComponent();
        FontSize = 18;
        _labelTemplateConverter = new StateColorConverter();
        _endIconConverter = new EndIconConverter();
        ErrorColor = Color.Parse("Red");
        LabelTitle.TranslationX = 10;
        LabelTitle.FontSize = _placeholderFontSize;
        FocusedBorderColor = Color.Parse("Gray");
    }
    public System.Collections.IList ItemsSource
    {
        get { return (System.Collections.IList)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public object SelectedItem
    {
        get { return (object)GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }
    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public int FontSize
    {
        get { return (int)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public bool HasError
    {
        get { return (bool)GetValue(HasErrorProperty); }
        set { SetValue(HasErrorProperty, value); }
    }
    public string ErrorText
    {
        get { return (string)GetValue(ErrorTextProperty); }
        set { SetValue(ErrorTextProperty, value); }
    }
    
    public string ItemText
    {
        get { return (string)GetValue(ItemTextProperty); }
        set { SetValue(ItemTextProperty, value); }
    }
    public string LabelText
    {
        get { return (string)GetValue(LabelTextProperty); }
        set { SetValue(LabelTextProperty, value); }
    }

    public Color EndIconColor
    {
        get { return (Color)GetValue(EndIconColorProperty); }
        set { SetValue(EndIconColorProperty, value); }
    }


    public Color PlaceholderColor
    {
        get { return (Color)GetValue(PlaceholderColorProperty); }
        set { SetValue(PlaceholderColorProperty, value); }
    }

    public Color FocusedBorderColor
    {
        get { return (Color)GetValue(FocusedBorderColorProperty); }
        set { SetValue(FocusedBorderColorProperty, value); }
    }

    public Color FocusedLabelColor
    {
        get { return (Color)GetValue(FocusedLabelColorProperty); }
        set { SetValue(FocusedLabelColorProperty, value); }
    }

    public Color ErrorColor
    {
        get { return (Color)GetValue(ErrorColorProperty); }
        set { SetValue(ErrorColorProperty, value); }
    }

    public FontImageSource EndIcon
    {
        get { return (FontImageSource)GetValue(EndIconProperty); }
        set { SetValue(EndIconProperty, value); }
    }

    public PropertyState State
    {
        get { return (PropertyState)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    /// <summary>
    /// Floating Label
    /// </summary>
    /// 
    public new void Focus()
    {
        if (IsEnabled)
        {
            ePicker.Focus();
        }
    }

    async void Handle_Focused(object sender, FocusEventArgs e)
    {
        if (SelectedIndex == -1)
        {
            await TransitionToTitle(true);
        }
    }
    async void Handle_Unfocused(object sender, FocusEventArgs e)
    {
        if (SelectedIndex == -1)
        {
            await TransitionToPlaceholder(true);
        }
    }


    private async void ePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SelectedIndex == -1)
        {
            await TransitionToPlaceholder(true);
        }else
        {
            await TransitionToTitle(true);
        }
    }
    async Task TransitionToTitle(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(10, _topMargin, 100);
            var t2 = SizeTo(_titleFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 10;
            LabelTitle.TranslationY = -30;
            LabelTitle.FontSize = 14;
        }
    }

    async Task TransitionToPlaceholder(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(10, 0, 100);
            var t2 = SizeTo(_placeholderFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 10;
            LabelTitle.TranslationY = 0;
            LabelTitle.FontSize = _placeholderFontSize;
        }
    }

    void Handle_Tapped(object sender, EventArgs e)
    {
        if (IsEnabled)
        {
            ePicker.Focus();
        }
    }

    Task SizeTo(int fontSize)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        // setup information for animation
        Action<double> callback = input => { LabelTitle.FontSize = input; };
        double startingHeight = LabelTitle.FontSize;
        double endingHeight = fontSize;
        uint rate = 5;
        uint length = 100;
        Easing easing = Easing.Linear;

        // now start animation with all the setup information
        LabelTitle.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

        return taskCompletionSource.Task;
    }
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == ItemTextProperty.PropertyName || propertyName == ItemsSourceProperty.PropertyName)
        {
            if(!string.IsNullOrEmpty(ItemText) && ItemsSource != null)
            {
                ePicker.ItemDisplayBinding = new Binding(ItemText);
                ePicker.ItemsSource = ItemsSource;
            }
            else if(propertyName == ItemsSourceProperty.PropertyName)
            {
                var items = new List<string>();
                foreach (object item in ItemsSource)
                {
                    items.Add(item.ToString());
                }
                ePicker.ItemsSource = items;
            }
        }
        else if (propertyName == FontSizeProperty.PropertyName)
        {
            _placeholderFontSize = FontSize;
            LabelTitle.FontSize = _placeholderFontSize;
        }
        else if (propertyName == StateProperty.PropertyName || propertyName == HasErrorProperty.PropertyName)
        {
            var color = !HasError ? Color.Parse("Gray") : (Color)_labelTemplateConverter.Convert(State, typeof(Color), null, CultureInfo.CurrentCulture);
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
