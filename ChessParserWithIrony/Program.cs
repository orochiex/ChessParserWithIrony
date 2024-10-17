using System;
using System.Collections.Generic;
using Irony.Parsing;

class ChessNotationParser
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите шахматную нотацию:");
        string input = Console.ReadLine();
        var moves = ParseMoves(input);

        for (int i = 0; i < moves.Count; i++)
        {
            ProcessMove(moves[i], i);
        }
    }

    public static List<string> ParseMoves(string notation)
    {
        var movesList = new List<string>();
        var grammar = new ChessNotationGrammar();
        var parser = new Parser(grammar);
        var parseTree = parser.Parse(notation);

        if (parseTree.HasErrors())
        {
            Console.WriteLine("Ошибка парсинга: " + string.Join(", ", parseTree.ParserMessages));
            return movesList;
        }

        foreach (var node in parseTree.Root.ChildNodes)
        {
            movesList.Add(node.FindTokenAndGetText());
        }

        return movesList;
    }

    public static void ProcessMove(string move, int index)
    {
        string color = index % 2 == 0 ? "белый" : "черный";
        string piece = GetPieceName(move);
        string position = GetPosition(move);
        string note = GetNotation(move);
        string capture = GetCapture(move);

        if (move == "O-O" || move == "O-O-O")
        {
            Console.WriteLine($"Рокировка {color} короля.");
        }
        else
        {
            string description = $"{color} фигура {piece} сходила на позицию {position} {capture}";
            if (!string.IsNullOrEmpty(note))
            {
                description += $" ({note})";
            }
            Console.WriteLine(description + ".");
        }
    }

    public static string GetPieceName(string move) => move[0] switch
    {
        'K' => "король",
        'Q' => "королева",
        'R' => "ладья",
        'N' => "конь",
        'B' => "слон",
        _ => "пешка"
    };

    public static string GetPosition(string move) => move.Length > 1 ? move[^2..] : move;

    public static string GetNotation(string move) => move.EndsWith("+") ? "шах" : move.EndsWith("#") ? "мат" : string.Empty;

    public static string GetCapture(string move) => move.Contains("x") ? " (взятие)" : string.Empty;
}

public class ChessNotationGrammar : Grammar
{
    public ChessNotationGrammar()
    {
        var move = new RegexBasedTerminal("move", @"[KQRBN]?(x|cx)?[a-h][1-8]"); // Ход фигуры или пешки
        var castling = new RegexBasedTerminal("castling", "O-O(-O)?"); // Рокировка

        // Объединяем правила
        var moves = new NonTerminal("moves");
        moves.Rule = move | castling;

        Root = moves;
    }
}

