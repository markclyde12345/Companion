using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;

namespace project;

public partial class Signup : ContentPage
{
    public Signup()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        var firstName = UsernameEntry.Text?.Trim();
        var lastName = LastNameEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        // 1. Empty field check
        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        // 2. Name validation
        var nameRegex = new Regex(@"^[a-zA-Z\s\-']+$");
        if (!nameRegex.IsMatch(firstName))
        {
            await DisplayAlert("Error", "First name can only contain letters, hyphens, or apostrophes.", "OK");
            return;
        }
        if (!nameRegex.IsMatch(lastName))
        {
            await DisplayAlert("Error", "Last name can only contain letters, hyphens, or apostrophes.", "OK");
            return;
        }

        // 3. Email format
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(email))
        {
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }

        // 4. Password strength
        if (password.Length < 8)
        {
            await DisplayAlert("Error", "Password must be at least 8 characters long.", "OK");
            return;
        }
        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            await DisplayAlert("Error", "Password must contain at least one uppercase letter.", "OK");
            return;
        }
        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            await DisplayAlert("Error", "Password must contain at least one number.", "OK");
            return;
        }
        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
        {
            await DisplayAlert("Error", "Password must contain at least one special character.", "OK");
            return;
        }

        // 5. Confirm password
        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            ConfirmPasswordEntry.Text = string.Empty;
            return;
        }

        try
        {
            await FirebaseService.RegisterAsync(firstName, lastName, email, password);

            // Show email verification notice
            await DisplayAlert(
                "📧 Verify Your Email",
                $"A verification link has been sent to:\n{email}\n\nPlease check your inbox and click the link before logging in.",
                "OK"
            );

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            // Friendly error messages for known Firebase errors
            string message;

            if (ex.Message.Contains("EMAIL_EXISTS") || ex.Message.Contains("EmailExists"))
            {
                message = "An account with this email already exists. Please sign in instead.";
            }
            else if (ex.Message.Contains("INVALID_EMAIL") || ex.Message.Contains("InvalidEmail"))
            {
                message = "The email address is not valid. Please check and try again.";
            }
            else if (ex.Message.Contains("WEAK_PASSWORD") || ex.Message.Contains("WeakPassword"))
            {
                message = "The password is too weak. Please choose a stronger password.";
            }
            else if (ex.Message.Contains("NETWORK_REQUEST_FAILED") || ex.Message.Contains("NetworkRequestFailed"))
            {
                message = "No internet connection. Please check your network and try again.";
            }
            else
            {
                message = ex.Message;
            }

            await DisplayAlert("Error", message, "OK");
        }
    }

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}