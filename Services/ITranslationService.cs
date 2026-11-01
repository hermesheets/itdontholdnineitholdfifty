namespace itdontholdnineitholdfifty.Services;

public interface ITranslationService
{
    bool TryTranslate(string word, out string translation);
}
