namespace itdontholdnineitholdfifty.ViewModels;

public sealed class WordProgressItem
{
    public WordProgressItem(string word, string translation, int attempts, int correct)
    {
        Word = word;
        Translation = translation;
        Attempts = attempts;
        Correct = correct;
    }

    public string Word { get; }
    public string Translation { get; }
    public int Attempts { get; }
    public int Correct { get; }
    public double AccuracyValue => Attempts == 0 ? 0 : (double)Correct / Attempts * 100;
    public string AccuracyText => Attempts == 0 ? "Not practiced" : $"{AccuracyValue:0}%";
    public string ResultsText => Attempts == 0 ? "0 tries" : $"{Correct} / {Attempts} correct";
}
