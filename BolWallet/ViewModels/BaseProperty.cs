namespace BolWallet.ViewModels;

public partial class BaseProperty : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsFilled))]
    [NotifyPropertyChangedFor(nameof(IsReady))]
    [NotifyPropertyChangedFor(nameof(State))]
    private string _value;

    public bool HasError
    {
        get => !string.IsNullOrEmpty(Value) && !IsValid(Value);
    }

    public bool IsMandatory { private get; set; }

    public bool IsFilled
    {
        get
        {
            if (IsMandatory)
            {
                return IsReady;
            }

            return string.IsNullOrEmpty(Value) || IsValid(Value);
        }
    }

    public bool IsReady
    {
        get => !string.IsNullOrEmpty(Value) && IsValid(Value);
    }

    public PropertyState State
    {
        get
        {
            if (HasError)
            {
                return PropertyState.HasError;
            }

            if (IsReady)
            {
                return PropertyState.IsReady;
            }

            return PropertyState.IsEmpty;
        }
    }

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private string _helpMessage;

    [ObservableProperty]
    private string _label;

    public Func<string, bool> IsValid { private get; set; }
}

public enum PropertyState
{
    IsEmpty,
    HasError,
    IsReady
}