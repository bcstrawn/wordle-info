using System.Collections.Generic;
using Wordle.Services;
using Xunit;

namespace Wordle.Tests;

public class UnitTest1
{
    private static readonly List<string> _vocab = new List<string>
    {
        "APPLE",
        "BAABB"
    };
    
    [Fact]
    public void Test1()
    {
        var guess = "AAAAA";
        
        var greenCount = 0;
        var yellowCount = 0;

        foreach (var vocabValue in _vocab)
        {
            var result = GuessService.GetColorsForGuess(guess, vocabValue);

            greenCount += result.Item1;
            yellowCount += result.Item2;
        }

        Assert.Equal(3, greenCount);
        Assert.Equal(0, yellowCount);
    }
    
    [Fact]
    public void Test2()
    {
        var guess = "BCCAA";
        
        var greenCount = 0;
        var yellowCount = 0;

        foreach (var vocabValue in _vocab)
        {
            var result = GuessService.GetColorsForGuess(guess, vocabValue);

            greenCount += result.Item1;
            yellowCount += result.Item2;
        }

        Assert.Equal(1, greenCount);
        Assert.Equal(2, yellowCount);
    }
}