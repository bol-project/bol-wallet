using CommunityToolkit.Maui.Views;

namespace BolWallet.ViewModels;

public sealed class PasswordPopup : Popup
{
    public TaskCompletionSource<string> TaskCompletionSource { get; set; } = new TaskCompletionSource<string>();

    public PasswordPopup()
    {
        Color primaryBlue = Color.FromArgb("#0078D7");
        Color accentBlue = Color.FromArgb("#0052CC");

        var passwordEntry = new Entry 
        { 
            IsPassword = true,
            Placeholder = "Password",
            Margin = new Thickness(20),
            HeightRequest = 60,
            HorizontalOptions = LayoutOptions.Fill,
            FontSize = 18,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing
        };
        
        passwordEntry.Completed += (sender, e) =>
        {
            Close();
            TaskCompletionSource.SetResult(passwordEntry.Text);
        };

        var submitButton = new Button 
        { 
            Text = "Submit",
            BackgroundColor = primaryBlue,
            TextColor = Colors.White,
            Margin = new Thickness(20, 5),
            HeightRequest = 50,
            CornerRadius = 25,
            HorizontalOptions = LayoutOptions.Fill,
            IsVisible = false
        };

        submitButton.Clicked += (sender, e) =>
        {
            Close();
            TaskCompletionSource.SetResult(passwordEntry.Text);
        };

        var cancelButton = new Button 
        {
            Text = "Cancel",
            BackgroundColor = accentBlue,
            TextColor = Colors.White,
            Margin = new Thickness(20, 5),
            HeightRequest = 50,
            CornerRadius = 25,
            HorizontalOptions = LayoutOptions.Fill
        };

        cancelButton.Clicked += (sender, e) =>
        {
            Close();
            TaskCompletionSource.SetResult(null);
        };

        // Toggle the visibility of the submit button based on text entry
        passwordEntry.TextChanged += (sender, e) =>
        {
            submitButton.IsVisible = !string.IsNullOrWhiteSpace(passwordEntry.Text);
        };

        var titleLabel = new Label 
        {
            Text = "Enter Your Password",
            HorizontalOptions = LayoutOptions.Center,
            FontSize = 24, 
            Margin = new Thickness(20, 20, 20, 10),
            TextColor = primaryBlue
        };
        
        // The layout hosting the entry and buttons
        var stackLayout = new VerticalStackLayout
        {
            Children = { passwordEntry, submitButton, cancelButton },
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Fill
        };

        // The frame that gives the popup its structure
        var frame = new Frame
        {
            CornerRadius = 20,
            Padding = 0, 
            Margin = new Thickness(20, 100),
            Content = new VerticalStackLayout
            {
                Children = { titleLabel, stackLayout },
                Spacing = 10,
                Padding = new Thickness(20)
            },
            BackgroundColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 300,
            HeightRequest = 400
        };

        Content = frame;
    }

    protected override Task OnDismissedByTappingOutsideOfPopup(CancellationToken token)
    {
        base.OnDismissedByTappingOutsideOfPopup(token);
        
        TaskCompletionSource.TrySetResult(null);
        return Task.CompletedTask;
    }
}


