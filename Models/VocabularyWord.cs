namespace itdontholdnineitholdfifty.Models;

public class VocabularyWord
{
    public int Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string ExampleSentence { get; set; } = string.Empty;
    public string Category { get; set; } = "Everyday";
}
