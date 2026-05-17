using Newtonsoft.Json;
using System.Text;
using System.Xml;
namespace project;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
        LoadUserProfile();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
{
    string currentPassword = CurrentPasswordEntry.Text?.Trim();
    string newPassword = NewPasswordEntry.Text?.Trim();

    if (string.IsNullOrWhiteSpace(currentPassword) ||
        string.IsNullOrWhiteSpace(newPassword))
    {
        await DisplayAlert("Error", "Fill all fields.", "OK");
        return;
    }

    try
    {
        await FirebaseService.ChangePasswordAsync(currentPassword, newPassword);

        await DisplayAlert("Success", "Password changed successfully.", "OK");

        CurrentPasswordEntry.Text = "";
        NewPasswordEntry.Text = "";
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", ex.Message, "OK");
    }
}

    private async void LoadUserProfile()
    {
        try
        {
            var profile = await FirebaseService.GetUserProfileAsync();

            if (profile == null)
            {
                await DisplayAlert("Error", "Profile not found.", "OK");
                return;
            }

            // 👤 Name (fix null safety)
            NameLabel.Text =
                $"{profile.FirstName ?? ""} {profile.LastName ?? ""}".Trim();

            // 📅 Member since (safe formatting)
            if (!string.IsNullOrEmpty(profile.CreatedAt))
            {
                var date = DateTime.Parse(profile.CreatedAt);
                MemberSinceLabel.Text = $"Member since {date:MMMM yyyy}";
            }
            else
            {
                MemberSinceLabel.Text = "Member since unknown";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}