using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Wordle.Models;
using Wordle.Services;

namespace Wordle.Controllers;

[ApiController]
// [Route("[controller]")]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WordleContext _context;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, WordleContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IEnumerable<Guess> GetGuesses(Sort sort, string? search = null)
    {
        Console.WriteLine($"{sort} {search}");

        IOrderedQueryable<Guess> query;

        if (sort == Sort.Green)
            query = _context.Guesses.OrderByDescending(x => x.AverageGreen);
        if (sort == Sort.Yellow)
            query = _context.Guesses.OrderByDescending(x => x.AverageYellow);
        if (sort == Sort.Total)
            query = _context.Guesses.OrderByDescending(x => x.Total);
        else
            query = _context.Guesses.OrderBy(x => x.Value);

        if (!string.IsNullOrWhiteSpace(search))
            return query.Where(x => x.Value.Contains(search)).Take(100).ToList();
        else
            return query.Take(100).ToList();
    }

    [HttpGet]
    public IEnumerable<SecondGuess> GetSecondGuesses(Sort sort)
    {
        Console.WriteLine($"{sort}");

        // var list = second ? _context.SecondGuesses : _context.Guesses;

        if (sort == Sort.Green)
            return _context.SecondGuesses.OrderByDescending(x => x.AverageGreen).Take(100).ToList();
        if (sort == Sort.Yellow)
            return _context.SecondGuesses.OrderByDescending(x => x.AverageYellow).Take(100).ToList();
        if (sort == Sort.Total)
            return _context.SecondGuesses.OrderByDescending(x => x.Total).Take(100).ToList();

        return _context.SecondGuesses.OrderBy(x => x.Value).Take(100).ToList();
    }

    [HttpGet]
    public int Count()
    {
        var count = _context.Guesses.Count();
        return count;
    }

    [HttpPost]
    public void ImportGuesses()
    {
        var count = 0;

        var guesses = _context.Guesses.ToList();
        if (guesses.Any())
        {
            foreach (var guess in guesses)
            {
                _context.Remove(guess);
            }

            _context.SaveChanges();
        }

        foreach (var line in System.IO.File.ReadLines("guesses.txt"))
        {
            _context.Guesses.Add(new Guess {
                Value = line
            });

            count++;

            if (count % 1000 == 0)
            {
                Console.WriteLine($"Guess count at {count}");
                _context.SaveChanges();
            }
        }

        _context.SaveChanges();
        Console.WriteLine($"Guesses import complete with total count at {count}");
    }

    [HttpPost]
    public void ImportSecondGuesses()
    {
        var count = 0;

        var guesses = _context.SecondGuesses.ToList();
        if (guesses.Any())
        {
            foreach (var guess in guesses)
            {
                _context.Remove(guess);
            }

            _context.SaveChanges();
        }

        foreach (var line in System.IO.File.ReadLines("guesses.txt"))
        {
            _context.SecondGuesses.Add(new SecondGuess {
                Value = line
            });

            count++;

            if (count % 1000 == 0)
            {
                Console.WriteLine($"Second Guess count at {count}");
                _context.SaveChanges();
            }
        }

        _context.SaveChanges();
        Console.WriteLine($"Second Guesses import complete with total count at {count}");
    }

    [HttpPost]
    public void ImportVocab()
    {
        var count = 0;

        var vocab = _context.Vocab.ToList();
        if (vocab.Any())
        {
            foreach (var value in vocab)
            {
                _context.Remove(value);
            }

            _context.SaveChanges();
        }

        foreach (var line in System.IO.File.ReadLines("vocab.txt"))
        {
            _context.Vocab.Add(new Vocab {
                Value = line.ToUpper()
            });

            count++;

            if (count % 1000 == 0)
            {
                Console.WriteLine($"Vocab count at {count}");
                _context.SaveChanges();
            }
        }

        _context.SaveChanges();
        Console.WriteLine($"Vocab import complete with total count at {count}");
    }
    
    [HttpPost]
    public void ProcessGuesses()
    {
        var batchSize = 500;
        var guesses = _context.Guesses.Where(g => !g.Processed).Take(batchSize).ToList();
        var vocab = _context.Vocab.Select(v => v.Value).ToList();

        while (guesses.Count > 0)
        {
            foreach (var guess in guesses)
            {
                var greenCount = 0;
                var yellowCount = 0;

                foreach (var vocabValue in vocab)
                {
                    var result = GuessService.GetColorsForGuess(guess.Value, vocabValue);

                    greenCount += result.Item1;
                    yellowCount += result.Item2;
                }

                guess.AverageGreen = greenCount;
                guess.AverageYellow = yellowCount;
                guess.Total = greenCount + yellowCount;
                guess.Processed = true;
            }

            _context.SaveChanges();
            Console.WriteLine($"Processed batch of {guesses.Count}");
            guesses = _context.Guesses.Where(g => !g.Processed).Take(batchSize).ToList();
        }

        Console.WriteLine("Done processing guesses");
    }
    
    [HttpPost]
    public void ProcessSecondGuesses([FromBody]string firstWord)
    {
        Console.WriteLine(firstWord);

        var filterOutLetters = firstWord.ToList();
        var batchSize = 500;
        var guesses = _context.SecondGuesses.Where(g => !g.Processed).Take(batchSize).ToList();
        var unfilteredvocab = _context.Vocab.Select(v => v.Value).ToList();
        var vocab = unfilteredvocab.Where(v => !v.Any(filterOutLetters.Contains));

        while (guesses.Count > 0)
        {
            foreach (var guess in guesses)
            {
                var greenCount = 0;
                var yellowCount = 0;

                foreach (var vocabValue in vocab)
                {
                    var result = GuessService.GetColorsForGuess(guess.Value, vocabValue);

                    greenCount += result.Item1;
                    yellowCount += result.Item2;
                }

                guess.AverageGreen = greenCount;
                guess.AverageYellow = yellowCount;
                guess.Total = greenCount + yellowCount;
                guess.Processed = true;
            }

            _context.SaveChanges();
            Console.WriteLine($"Processed batch of {guesses.Count}");
            guesses = _context.SecondGuesses.Where(g => !g.Processed).Take(batchSize).ToList();
        }

        Console.WriteLine("Done processing second guesses");

        /* yellows
        C - DITCH
        A - ALONG or TALON (ALOIN)
        R - PROUD (DROIT)
        E - NOBLE (TOILE)
        S - TOILS (HOIST)
        CA - FOCAL
        */
    }

    [HttpGet]
    public IEnumerable<Guess> GetLetters(Sort sort)
    {
        var vocab = _context.Vocab.Select(v => v.Value).ToList();
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var dictionary = new Dictionary<char, int>();
        
        foreach (var letter in letters)
        {
            dictionary.Add(letter, 0);
        }

        foreach (var word in vocab)
        {
            var letterSet = word.ToHashSet();
            
            foreach (var letter in letterSet)
            {
                dictionary[letter] += 1;
            }
        }

        var result = dictionary.Select(x => new Guess {
            Id = (int)x.Key,
            Value = x.Key.ToString(),
            AverageGreen = x.Value
        }).OrderByDescending(x => x.AverageGreen);

        return result;
    }

    public enum Sort
    {
        Alpha,
        Green,
        Yellow,
        Total
    }
}
