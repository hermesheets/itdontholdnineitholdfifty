namespace itdontholdnineitholdfifty.Services;

public sealed class OfflineTranslationService : ITranslationService
{
    public bool TryTranslate(string word, out string translation) =>
        OfflineTranslator.TryTranslate(word, out translation);
}
