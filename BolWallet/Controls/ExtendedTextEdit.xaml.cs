using BolWallet.Converters;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace BolWallet.Controls;

public partial class ExtendedTextEdit : ContentView
{

    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(ExtendedTextEdit), ReturnType.Default);
    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create("IsPassword", typeof(bool), typeof(ExtendedTextEdit), default(bool));
    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create("Keyboard", typeof(Microsoft.Maui.Keyboard), typeof(ExtendedTextEdit), Microsoft.Maui.Keyboard.Default, coerceValue: (o, v) => (Microsoft.Maui.Keyboard)v ?? Microsoft.Maui.Keyboard.Default);
    public static readonly BindableProperty ClearIconVisibilityProperty = BindableProperty.Create(nameof(ClearIconVisibility),typeof(ClearButtonVisibility),typeof(ExtendedTextEdit));
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(ExtendedTextEdit));
    public static readonly BindableProperty BoxHeightProperty = BindableProperty.Create(nameof(BoxHeight), typeof(int), typeof(ExtendedTextEdit));
    public static readonly BindableProperty AllowAnimationProperty = BindableProperty.Create(nameof(AllowAnimation), typeof(bool), typeof(ExtendedTextEdit));
    public static readonly BindableProperty IsLabelFloatingProperty = BindableProperty.Create(nameof(IsLabelFloating), typeof(bool), typeof(ExtendedTextEdit));
    public static readonly BindableProperty IsErrorIconVisibleProperty = BindableProperty.Create(nameof(IsErrorIconVisible), typeof(bool), typeof(ExtendedTextEdit));
    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(ExtendedTextEdit));
    public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(nameof(HasError), typeof(bool), typeof(ExtendedTextEdit));
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ExtendedTextEdit));
    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ExtendedTextEdit));
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ExtendedTextEdit));
    public static readonly BindableProperty HelpTextProperty = BindableProperty.Create(nameof(HelpText), typeof(string), typeof(ExtendedTextEdit));
    public static readonly BindableProperty EndIconColorProperty = BindableProperty.Create(nameof(EndIconColor), typeof(Color), typeof(ExtendedTextEdit));
    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(ExtendedTextEdit));
    public static readonly BindableProperty FocusedBorderColorProperty = BindableProperty.Create(nameof(FocusedBorderColor), typeof(Color), typeof(ExtendedTextEdit));
    public static readonly BindableProperty FocusedLabelColorProperty = BindableProperty.Create(nameof(FocusedLabelColor), typeof(Color), typeof(ExtendedTextEdit));
    public static readonly BindableProperty EndIconProperty = BindableProperty.Create(nameof(EndIcon), typeof(FontImageSource), typeof(ExtendedTextEdit));
    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(PropertyState), typeof(ExtendedTextEdit));
    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExtendedTextEdit));
    public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(nameof(ReturnCommand), typeof(ICommand), typeof(ExtendedTextEdit));
    public static readonly BindableProperty ReturnCommandParameterProperty = BindableProperty.Create(nameof(ReturnCommandParameter), typeof(object), typeof(ExtendedTextEdit));
    public static readonly BindableProperty TextTransformProperty = BindableProperty.Create(nameof(TextTransform), typeof(Microsoft.Maui.TextTransform), typeof(ExtendedTextEdit));
    public static readonly BindableProperty NumberOfTapsRequiredProperty = BindableProperty.Create(nameof(NumberOfTapsRequired), typeof(int), typeof(ExtendedTextEdit));
    
    public event EventHandler Completed;
    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler<EventArgs> Tap;

    private readonly StateColorConverter _labelTemplateConverter;
    private readonly EndIconConverter _endIconConverter;

    public int EntryWidth { get; set; } = Convert.ToInt32(Math.Round(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density * 0.9));


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
        var control = bindable as ExtendedTextEdit;
        if (!control.eEntry.IsFocused)
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

    public ExtendedTextEdit()
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
    
    void Handle_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextChanged.Invoke(sender, e);
    }
    void Handle_Completed(object sender, EventArgs e)
    {
        Completed?.Invoke(sender, e);
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        Tap?.Invoke(sender, e);
    }
    
    public Microsoft.Maui.TextTransform TextTransform
    {
        get { return (Microsoft.Maui.TextTransform)GetValue(TextTransformProperty); }
        set { SetValue(TextTransformProperty, value); }
    }
    public object ReturnCommandParameter
    {
        get { return (object)GetValue(ReturnCommandParameterProperty); }
        set { SetValue(ReturnCommandParameterProperty, value); }
    }
    public ICommand ReturnCommand
    {
        get { return (ICommand)GetValue(ReturnCommandProperty); }
        set { SetValue(ReturnCommandProperty, value); }
    }
    public ClearButtonVisibility ClearIconVisibility
    {
        get { return (ClearButtonVisibility)GetValue(ClearIconVisibilityProperty); }
        set { SetValue(ClearIconVisibilityProperty, value); }
    }
    public int NumberOfTapsRequired
    {
        get { return (int)GetValue(NumberOfTapsRequiredProperty); }
        set { SetValue(NumberOfTapsRequiredProperty, value); }
    }
    public int FontSize
    {
        get { return (int)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }
    public int BoxHeight
    {
        get { return (int)GetValue(BoxHeightProperty); }
        set { SetValue(BoxHeightProperty, value); }
    }
    
    public bool AllowAnimation
    {
        get { return (bool)GetValue(AllowAnimationProperty); }
        set { SetValue(AllowAnimationProperty, value); }
    }

    public bool IsLabelFloating
    {
        get { return (bool)GetValue(IsLabelFloatingProperty); }
        set { SetValue(IsLabelFloatingProperty, value); }
    }

    public bool IsErrorIconVisible
    {
        get { return (bool)GetValue(IsErrorIconVisibleProperty); }
        set { SetValue(IsErrorIconVisibleProperty, value); }
    }
    public bool IsReadOnly
    {
        get { return (bool)GetValue(IsReadOnlyProperty); }
        set { SetValue(IsReadOnlyProperty, value); }
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

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    
    public string HelpText
    {
        get { return (string)GetValue(HelpTextProperty); }
        set { SetValue(HelpTextProperty, value); }
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
    public ReturnType ReturnType
    {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }

    public bool IsPassword
    {
        get { return (bool)GetValue(IsPasswordProperty); }
        set { SetValue(IsPasswordProperty, value); }
    }

    public Microsoft.Maui.Keyboard Keyboard
    {
        get { return (Microsoft.Maui.Keyboard)GetValue(KeyboardProperty); }
        set { SetValue(KeyboardProperty, value); }
    }
    public PropertyState State
    {
        get { return (PropertyState)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    // ///////////////////////////////////////////////////////
    public new void Focus()
    {
        if (IsEnabled)
        {
            eEntry.Focus();
        }
    }

    async void Handle_Focused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
        {
            await TransitionToTitle(true);
        }
    }
    async void Handle_Unfocused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
        {
            await TransitionToPlaceholder(true);
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
        //LabelTitle.Text = "  " + LabelTitle.Text;
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
        //LabelTitle.Text= LabelTitle.Text.Trim();
    }

    void Handle_Tapped(object sender, EventArgs e)
    {
        if (IsEnabled)
        {
            eEntry.Focus();
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
    // ////////////////////////////////////// //

    protected override async void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TextProperty.PropertyName)
        {
            if(!string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle(true);
            }
        }
        else if (propertyName == FontSizeProperty.PropertyName)
        {
            _placeholderFontSize = FontSize;
            LabelTitle.FontSize = _placeholderFontSize;
        }
        else if(propertyName == StateProperty.PropertyName || propertyName == HasErrorProperty.PropertyName)
        {
            var color = !HasError ? Color.Parse("Gray") : (Color)_labelTemplateConverter.Convert(State, typeof(Color), null, CultureInfo.CurrentCulture);
            EndIconColor = color;
            FocusedBorderColor = color;
            FocusedLabelColor = color;

            EndIcon = new FontImageSource()
            {
                FontFamily = "MaterialIconsRegular",
                Glyph =  (string)_endIconConverter.Convert(State, typeof(string), null, CultureInfo.CurrentCulture),
                Size = 24
            };
        }
    }

}