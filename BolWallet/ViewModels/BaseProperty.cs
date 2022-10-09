namespace BolWallet.ViewModels;

public partial class BaseProperty : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsNotReady))]
    [NotifyPropertyChangedFor(nameof(IsReady))]
    private string _value;

    public bool HasError
    {
        get => !string.IsNullOrEmpty(Value) && !IsValid(Value);
    }

    public bool IsMandatory { private get; set; }

    public bool IsNotReady
    {
        get => IsMandatory && string.IsNullOrEmpty(Value);
    }

    public bool IsReady
    {
        get => !string.IsNullOrEmpty(Value) && IsValid(Value);
    }

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private string _helpMessage;

    [ObservableProperty]
    private string _label;

    public Func<string, bool> IsValid { private get; set; }
}

