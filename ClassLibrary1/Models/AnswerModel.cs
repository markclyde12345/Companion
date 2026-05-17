public class AnswerModel
{
    public string Text { get; set; }
    public int Score { get; set; }

    public AnswerModel(string text, int score)
    {
        Text = text;
        Score = score;
    }
}