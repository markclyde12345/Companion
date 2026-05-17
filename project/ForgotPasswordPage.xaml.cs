namespace project;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Send Firebase password reset email
    private async void OnSendOtpTapped(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            MessageLabel.TextColor = Colors.Red;
            MessageLabel.Text = "Please enter your email.";
            return;
        }

        // Basic email format check
        if (!email.Contains("@") || !email.Contains("."))
        {
            MessageLabel.TextColor = Colors.Red;
            MessageLabel.Text = "Please enter a valid email address.";
            return;
        }

        try
        {
            // Firebase sends the reset link directly to the user's Gmail/email
            await FirebaseService.SendPasswordResetAsync(email);

            MessageLabel.TextColor = Color.FromArgb("#3A8A3A");
            MessageLabel.Text = "";

            await DisplayAlert(
                "📧 Check Your Email",
                $"A password reset link has been sent to:\n{email}\n\nOpen your email and click the link to reset your password.",
                "OK"
            );

            // Go back to login after sending
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            MessageLabel.TextColor = Colors.Red;
            MessageLabel.Text = "Failed to send reset email. Please check your email and try again.";
        }
    }

    // These are kept to avoid XAML errors but are no longer used
    private void OnConfirmedTapped(object sender, EventArgs e) { }
    private async void OnChangePasswordTapped(object sender, EventArgs e) { }
}