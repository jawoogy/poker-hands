using System.Text.RegularExpressions;

internal class Card : IComparable<Card>
{
    public string Suit { get; init; }
    public string Face { get; init; }

    public int NumValue
    {
        get
        {
            return Face switch
            {
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "6" => 6,
                "7" => 7,
                "8" => 8,
                "9" => 9,
                "10" => 10,
                "J" => 11,
                "Q" => 12,
                "K" => 13,
                "A" => 14,
                _ => throw new ArgumentException($"Invalid card face: {Face}"),
            };
        }
    }

    public Card(string card)
    {
        var match = Regex.Match(card.Trim(), @"^(10|[2-9JQKA])([HDCS])$");
        if (!match.Success)
        {
            throw new ArgumentException($"Invalid card format: {card}");
        }
        var face = match.Groups[1].Value;
        var suit = match.Groups[2].Value;
        Suit = suit;
        Face = face;
    }

    public int CompareTo(Card? other)
    {
        return NumValue.CompareTo(other?.NumValue);
    }

    public override string ToString()
    {
        return $"{Face}{Suit}";
    }
}
