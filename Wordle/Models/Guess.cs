namespace Wordle.Models;

public class Guess
{
    public int Id { get; set; }
    public string Value { get; set; }
    public bool Processed { get; set; }
    public float AverageGreen { get; set; }
    public float AverageYellow { get; set; }
    public float Total { get; set; }
}

public class SecondGuess
{
    public int Id { get; set; }
    public string Value { get; set; }
    public bool Processed { get; set; }
    public float AverageGreen { get; set; }
    public float AverageYellow { get; set; }
    public float Total { get; set; }
}