using System.Collections.ObjectModel;
namespace project;

public partial class ChatBot : ContentPage
{
    ObservableCollection<ChatMessage> Messages =
        new ObservableCollection<ChatMessage>();
    public ChatBot()
	{
		InitializeComponent();

        MessagesView.ItemsSource = Messages;

        Messages.Add(new ChatMessage
        {
            Text = "Hello, I'm Dr. Aria. How are you feeling today?",
            BubbleColor = Color.FromArgb("#DFF3E3"),
            Align = LayoutOptions.Start
        });

    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        string userText = MessageEntry.Text?.Trim();

        if (string.IsNullOrEmpty(userText))
            return;

        Messages.Add(new ChatMessage
        {
            Text = userText,
            BubbleColor = Color.FromArgb("#BFD7EA"),
            Align = LayoutOptions.End
        });

        MessageEntry.Text = "";

        string reply = await AIService.GetReply(userText);

        Messages.Add(new ChatMessage
        {
            Text = reply,
            BubbleColor = Color.FromArgb("#DFF3E3"),
            Align = LayoutOptions.Start
        });

        MessagesView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: true);
    }

    private async void OnCloseTapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}