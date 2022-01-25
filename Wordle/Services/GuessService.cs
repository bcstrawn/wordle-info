namespace Wordle.Services;

public static class GuessService
{
    public static Dictionary<char, Color> _hits = new Dictionary<char, Color>(5);

    public static (int, int) GetColorsForGuess(string guess, string vocabValue)
    {
        _hits.Clear();
        var greenCount = 0;
        var yellowCount = 0;

        for (var i = 0; i < guess.Length; i++)
        {
            var character = guess[i];
            var isGreen = vocabValue[i] == character;
            if (isGreen)
            {
                if (_hits.ContainsKey(character))
                {
                    var existingColor = _hits[character];
                    if (existingColor == Color.Green)
                    {
                        greenCount++;
                    }
                    else
                    {
                        _hits[character] = Color.Green;
                    }
                }
                else
                {
                    _hits.Add(character, Color.Green);
                }

                continue;
            }

            var isYellow = vocabValue.Contains(character);
            if (isYellow)
            {
                if (_hits.ContainsKey(character))
                {
                    var existingColor = _hits[character];
                    if (existingColor == Color.Green)
                    {
                        // do nothing
                    }
                    else
                    {
                        // do nothing
                    }
                }
                else
                {
                    _hits.Add(character, Color.Yellow);
                }
            }
        }

        foreach (var color in _hits.Values)
        {
            if (color == Color.Green)
                greenCount++;
            else
                yellowCount++;
        }

        return (greenCount, yellowCount);
    }
}

public enum Color
{
    Yellow,
    Green
}