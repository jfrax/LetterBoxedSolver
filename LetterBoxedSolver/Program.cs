using System;
using System.Text.RegularExpressions;



var board = new Board(
      new Side('L', 'C', 'V')
    , new Side('R', 'W', 'A')
    , new Side('E', 'N', 'G')
    , new Side('T', 'I', 'O'));

var dictSource = File.ReadAllText("dictionary.txt");

var dict = Regex.Matches(dictSource, @"^[a-z]{3,}[\s\S]$", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace)
    .Select(v => v.Value.Trim().ToUpper());

var validWords = dict.Where(x => board.IsWordValid(x));

var dictXProduct = (from d in validWords
                   join d2 in validWords on 1 equals 1
                   select new { Word1 = d, Word2 = d2 }).ToList();


var solutions = from d in dictXProduct
                where (board.GetPositionalCoverage(d.Word1).Concat(board.GetPositionalCoverage(d.Word2))).Distinct().Count() == 12
                    && (d.Word1.EndsWith(d.Word2[0]))
                select d;

Console.WriteLine("Here are two-word solutions:");
foreach (var s in solutions) { Console.WriteLine($"{s.Word1}, {s.Word2}"); }

Console.WriteLine();
Console.WriteLine();

Console.WriteLine("And here are some long words:");

foreach (var longWord in validWords.OrderByDescending(x => x.Length).Take(50))
{
    Console.WriteLine(longWord);
}
Console.ReadLine();





class Side
{
    public Side(char a, char b, char c)
    {
        Letters = new [] { a, b, c };
    }

    public char[] Letters { get; set; }

}

class Board
{
    public Board(Side a, Side b, Side c, Side d)
    {
        Sides = new Tuple<Side, Side, Side, Side>(a, b, c, d);
    }

    public Tuple<Side, Side, Side, Side> Sides { get; set; }

    public IEnumerable<int> GetPositionalCoverage(string word)
    {
        var list = new List<int>();

        foreach(var c in word)
        {
            if (Sides.Item1.Letters[0] == c) list.Add(1);
            if (Sides.Item1.Letters[1] == c) list.Add(2);
            if (Sides.Item1.Letters[2] == c) list.Add(3);

            if (Sides.Item2.Letters[0] == c) list.Add(4);
            if (Sides.Item2.Letters[1] == c) list.Add(5);
            if (Sides.Item2.Letters[2] == c) list.Add(6);

            if (Sides.Item3.Letters[0] == c) list.Add(7);
            if (Sides.Item3.Letters[1] == c) list.Add(8);
            if (Sides.Item3.Letters[2] == c) list.Add(9);

            if (Sides.Item4.Letters[0] == c) list.Add(10);
            if (Sides.Item4.Letters[1] == c) list.Add(11);
            if (Sides.Item4.Letters[2] == c) list.Add(12);
        }

        return list.Distinct();
    }

    public bool IsWordValid(string word)
    {
        Side? lastSide = null;
        foreach (var c in word)
        {
            if (!IsLetterValid(c)) return false;

            var ls = SideOfLetter(c);

            if (lastSide != null && ls == lastSide)
            {
                return false;    
            }
            
            lastSide = ls;

        }

        return true;
    }

    public Side SideOfLetter(char letter)
    {
        if (Sides.Item1.Letters.Contains(letter)) return Sides.Item1;
        if (Sides.Item2.Letters.Contains(letter)) return Sides.Item2;
        if (Sides.Item3.Letters.Contains(letter)) return Sides.Item3;
        if (Sides.Item4.Letters.Contains(letter)) return Sides.Item4;

        throw new Exception($"Letter {letter} not contained on any side");
    }

    public bool IsLetterValid(char c)
    {
        return
                Sides.Item1.Letters.Contains(c)
                || Sides.Item2.Letters.Contains(c)
                || Sides.Item3.Letters.Contains(c)
                || Sides.Item4.Letters.Contains(c);
    }
}

