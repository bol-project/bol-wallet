using BolWallet.Converters;
using System.Dynamic;
using System.Globalization;

namespace BolWallet.Controls;

public partial class ExtendedDatePicker : ContentView
{
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(ExtendedDatePicker));
    public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(nameof(HasError), typeof(bool), typeof(ExtendedDatePicker));
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ExtendedDatePicker));
    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ExtendedDatePicker));
    public static readonly BindableProperty EndIconColorProperty = BindableProperty.Create(nameof(EndIconColor), typeof(Color), typeof(ExtendedDatePicker));
    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(ExtendedDatePicker));
    public static readonly BindableProperty FocusedBorderColorProperty = BindableProperty.Create(nameof(FocusedBorderColor), typeof(Color), typeof(ExtendedDatePicker));
    public static readonly BindableProperty FocusedLabelColorProperty = BindableProperty.Create(nameof(FocusedLabelColor), typeof(Color), typeof(ExtendedDatePicker));
    public static readonly BindableProperty EndIconProperty = BindableProperty.Create(nameof(EndIcon), typeof(FontImageSource), typeof(ExtendedDatePicker));
    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(PropertyState), typeof(ExtendedDatePicker));
    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExtendedDatePicker));
    public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(ExtendedDatePicker));
    public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(ExtendedDatePicker));
    public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(ExtendedDatePicker));
    public static readonly BindableProperty FormatProperty = BindableProperty.Create(nameof(Format), typeof(string), typeof(ExtendedDatePicker));
    
    private readonly StateColorConverter _labelTemplateConverter;
    private readonly EndIconConverter _endIconConverter;

    int _placeholderFontSize = 18;
    int _titleFontSize = 10;
    int _topMargin = -20;
    static async void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as ExtendedDatePicker;
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

    public ExtendedDatePicker()
	{
		InitializeComponent();
        FontSize = 18;
        _labelTemplateConverter = new StateColorConverter();
        _endIconConverter = new EndIconConverter();
        ErrorColor = Color.Parse("Red");
        LabelTitle.TranslationX = 10;
        LabelTitle.FontSize = _titleFontSize;
        LabelTitle.TranslateTo(0, _topMargin, 100); 
        FocusedBorderColor = Color.Parse("Gray");
    }
    public DateTime Date
    {
        get { return (DateTime)GetValue(DateProperty); }
        set { SetValue(DateProperty, value); }
    }
    public DateTime MinimumDate
    {
        get { return (DateTime)GetValue(MinimumDateProperty); }
        set { SetValue(MinimumDateProperty, value); }
    }
    public DateTime MaximumDate
    {
        get { return (DateTime)GetValue(MaximumDateProperty); }
        set { SetValue(MaximumDateProperty, value); }
    }
    public string Format
    {
        get { return (string)GetValue(FormatProperty); }
        set { SetValue(FormatProperty, value); }
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
    /// <param name="propertyName"></param>
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
        
        {
            //await TransitionToTitle(true);
        }
    }
    async void Handle_Unfocused(object sender, FocusEventArgs e)
    {
        
        {
            //await TransitionToPlaceholder(true);
        }
    }

    async Task TransitionToTitle(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(0, _topMargin, 100);
            var t2 = SizeTo(_titleFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 0;
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

    private Dictionary<string, object> Params = new();
    protected override async void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        /*
        if(!Params.ContainsKey(propertyName))
        {

            if (propertyName == FormatProperty.PropertyName)
            {
                Params.Add(propertyName, Format);
            }
            else if (propertyName == DateProperty.PropertyName)
            {
                MaximumDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
                if(Date > MaximumDate) Date= MaximumDate;
                Params.Add(propertyName, Date);
            }
            else if (propertyName == MaximumDateProperty.PropertyName)
            {
                Params.Add(propertyName, MaximumDate);
            }
            else if (propertyName == MinimumDateProperty.PropertyName)
            {
                Params.Add(propertyName, MinimumDate);
            }
            else if (propertyName == PlaceholderColorProperty.PropertyName)
            {
                Params.Add(propertyName, PlaceholderColor);
            }
            else if (propertyName == EndIconProperty.PropertyName)
            {
                Params.Add(propertyName, EndIcon);
            }
            else if (propertyName == FocusedLabelColorProperty.PropertyName)
            {
                Params.Add(propertyName, FocusedLabelColor);
            }
            else if (propertyName == FocusedBorderColorProperty.PropertyName)
            {
                Params.Add(propertyName, FocusedBorderColor);
            }
            else if (propertyName == ErrorColorProperty.PropertyName)
            {
                Params.Add(propertyName, ErrorColor);
            }
            else if (propertyName == EndIconColorProperty.PropertyName)
            {
                Params.Add(propertyName, EndIconColor);
            }
            else if (propertyName == LabelTextProperty.PropertyName)
            {
                Params.Add(propertyName, LabelText);
            }
            else if (propertyName == ErrorTextProperty.PropertyName)
            {
                Params.Add(propertyName, ErrorText);
            }
            else if (propertyName == FontSizeProperty.PropertyName)
            {
                _placeholderFontSize = FontSize;
                LabelTitle.FontSize = _placeholderFontSize;
                Params.Add(propertyName, FontSize);
            }
            else 
            
        }
        */
        
        if (propertyName == StateProperty.PropertyName || propertyName == HasErrorProperty.PropertyName)
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
            //Params.Add(propertyName, propertyName == StateProperty.PropertyName ? State : HasError);
        }
    }

}