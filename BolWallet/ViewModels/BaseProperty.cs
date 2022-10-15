namespace BolWallet.ViewModels;

public partial class BaseProperty : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(IsReady))]
    [NotifyPropertyChangedFor(nameof(State))]
    private string _value;

    public bool HasError
    {
        get => !string.IsNullOrEmpty(Value) && !IsValid(Value);
    }

    public bool IsMandatory { private get; set; }

    public bool IsReady
    {
        get
        {
            if (IsMandatory)
            {
                return !string.IsNullOrEmpty(Value) && IsValid(Value);
            }

            return string.IsNullOrEmpty(Value) || IsValid(Value);
        }
    }

    public PropertyState State
    {
        get
        {
            if (string.IsNullOrEmpty(Value))
            {
                return PropertyState.IsEmpty;
            }

            return IsValid(Value) ? PropertyState.IsReady : PropertyState.HasError;
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