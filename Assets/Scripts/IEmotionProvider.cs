using System.Collections.Generic;

public interface IEmotionProvider
{
    Dictionary<string, int> GetEmotionFrequency();
}