using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Linq;

namespace project;

public partial class MoodCheckInPage : ContentPage
{
    private int _currentQuestionIndex = 0;
    private int _totalScore = 0;
    private AnswerModel _selectedAnswer = null;

    private readonly Dictionary<int, int> _answerScores = new Dictionary<int, int>();

    private readonly List<QuestionModel> _questions = new List<QuestionModel>
    {
        new QuestionModel
        {
            QuestionText = "How do you feel physically right now?",
            Answers = new List<AnswerModel>
            {
                new AnswerModel("Relax and energized", 5),
                new AnswerModel("Calm and comfortable", 4),
                new AnswerModel("Normal, nothing special", 3),
                new AnswerModel("Tense or tired", 2),
                new AnswerModel("Heavy, exhausted, or in pain", 1)
            }
        },
        new QuestionModel
        {
            QuestionText = "What kind of thoughts are running through your mind?",
            Answers = new List<AnswerModel>
            {
                new AnswerModel("Positive and hopeful", 5),
                new AnswerModel("Mostly good, some worries", 4),
                new AnswerModel("Neutral, just going through the day", 3),
                new AnswerModel("Worried or stressed", 2),
                new AnswerModel("Very negative or overwhelming", 1)
            }
        },
        new QuestionModel
        {
            QuestionText = "How's your energy and motivation?",
            Answers = new List<AnswerModel>
            {
                new AnswerModel("High energy, ready to take on anything", 5),
                new AnswerModel("Good energy, feeling productive", 4),
                new AnswerModel("Moderate, getting by", 3),
                new AnswerModel("Low energy, struggling a bit", 2),
                new AnswerModel("Completely drained, can't function well", 1)
            }
        }
    };

    public MoodCheckInPage()
    {
        InitializeComponent();
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (_currentQuestionIndex >= _questions.Count)
            return;

        var current = _questions[_currentQuestionIndex];

        questionlabel.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span
                {
                    Text = current.QuestionText,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black
                },
                new Span
                {
                    Text = "\nBe honest with yourself - there are no wrong answers.",
                    FontSize = 14,
                    TextColor = Colors.Gray
                }
            }
        };

        answersCollection.ItemsSource = current.Answers;
        answersCollection.SelectedItem = null;

        progressBar.Progress = (double)(_currentQuestionIndex + 1) / _questions.Count;
        StepLabel.Text = $"Step {_currentQuestionIndex + 1} of {_questions.Count}";

        _selectedAnswer = null;
        NextButton.IsEnabled = false;
        NextButton.Opacity = 0.5;

        NextButton.Text = _currentQuestionIndex == _questions.Count - 1
            ? "Finish ✓"
            : "Next →";
    }

    private void OnAnswerSelected(object sender, SelectionChangedEventArgs e)
    {
        _selectedAnswer = e.CurrentSelection.FirstOrDefault() as AnswerModel;
        NextButton.IsEnabled = _selectedAnswer != null;
        NextButton.Opacity = _selectedAnswer != null ? 1.0 : 0.5;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (_selectedAnswer == null) return;

        _answerScores[_currentQuestionIndex] = _selectedAnswer.Score;
        _totalScore += _selectedAnswer.Score;
        _currentQuestionIndex++;

        if (_currentQuestionIndex < _questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            // Prepare answers list
            var answersList = _answerScores
                .OrderBy(x => x.Key)
                .Select(x => x.Value)
                .ToList();

            // Determine mood label
            string moodResult = GetMoodResult();

            // 🔥 SAVE TO DATABASE
            await FirebaseService.SaveMoodAsync(
                moodResult,
                _totalScore,
                "", // notes (optional)
                answersList
            );

            await NavigateByAnswers();
            ResetQuiz();
        }
    }

    private string GetMoodResult()
{
    if (_totalScore >= 13) return "Great";
    if (_totalScore >= 10) return "Good";
    if (_totalScore >= 8) return "Okay";
    if (_totalScore >= 5) return "Sad";
    return "Exhausted";
}

    private async Task NavigateByAnswers()
    {
        // Get individual question scores (0 = skipped)
        int q1 = _answerScores.ContainsKey(0) ? _answerScores[0] : 0; // Physical
        int q2 = _answerScores.ContainsKey(1) ? _answerScores[1] : 0; // Thoughts
        int q3 = _answerScores.ContainsKey(2) ? _answerScores[2] : 0; // Energ

        // Score-based fallback
        // 13–15 → moodvalidate "great"  (includes perfect 5,5,5 = 15)
        // 10–12 → moodvalidate "good"
        //  8–9  → OkayValidationPage
        //  5–7  → SadValidationPage
        //  0–4  → OverwhelmingValidationPage
        if (_totalScore >= 13)
            await Navigation.PushAsync(new moodvalidate("great"));
        else if (_totalScore >= 10)
            await Navigation.PushAsync(new GoodValidationPage());
        else if (_totalScore >= 8)
            await Navigation.PushAsync(new OkayValidationPage());
        else if (_totalScore >= 5)
            await Navigation.PushAsync(new SadValidationPage());
        else
            await Navigation.PushAsync(new HeavyExhaustedValidationPage());
    }

    private void OnSkipTapped(object sender, EventArgs e)
    {
        // Skipped = no score recorded for this question
        _currentQuestionIndex++;

        if (_currentQuestionIndex < _questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            _ = NavigateByAnswers();
            ResetQuiz();
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void ResetQuiz()
    {
        _currentQuestionIndex = 0;
        _totalScore = 0;
        _selectedAnswer = null;
        _answerScores.Clear();
        answersCollection.SelectedItem = null;
        ShowQuestion();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ResetQuiz();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}