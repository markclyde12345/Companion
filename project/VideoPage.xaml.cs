namespace project;

public partial class VideoPage : ContentPage
{
	public VideoPage()
	{
		InitializeComponent();

        LoadVideo();

        videoWebView.Navigating += VideoWebView_Navigating;
    }

    private void LoadVideo()
    {
        videoWebView.Source = new HtmlWebViewSource
        {
            Html = @"
<html>
<body style='margin:0;background:black;overflow:hidden;'>

    <video id='video'
           width='100%'
           height='100%'
           controls
           autoplay
           style='position:fixed;top:0;left:0;object-fit:contain;background:black;'>

        <source src='meditation.mp4' type='video/mp4'>
    </video>

    <button onclick='window.location.href=""closeapp""'
        style='position:fixed;
               top:20px;
               right:20px;
               z-index:999;
               padding:12px 18px;
               border:none;
               border-radius:10px;
               font-size:16px;
               background:red;
               color:white;'>
        Close
    </button>

    <script>
        var video = document.getElementById('video');

        video.onended = function () {
            window.location.href = 'closeapp';
        };
    </script>

</body>
</html>"
        };
    }

    private async void VideoWebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.Contains("closeapp"))
        {
            e.Cancel = true;
            await Navigation.PopAsync();
        }
    }
}