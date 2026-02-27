using System.Text.Json.Serialization;

internal class PokerHand
{
    public string Player { get; init; }
    public string[] Cards { get; init; }
    public string HandEval { get; internal set; } = string.Empty;
    internal Card[] CardList { get; init; }

    // Things all collapse to the Ranking and total value of the cards.
    // Nothing in the reqs say anything about wrapping straights, we will ignore that condition, which according to what I saw online,
    // It's a homegrown rule similar to one-eyed jacks wild, etc...
    internal int Rank { get; private set; }

    [JsonConstructor]
    public PokerHand(string player, string[] cards)
    {
        Player = player;
        if (cards.Length != 5)
        {
            throw new ArgumentException("A poker hand must consist of exactly 5 cards.");
        }
        Cards = cards;
        CardList = cards.Select(c => new Card(c)).OrderByDescending(c => c.NumValue).ToArray();

        RankHand();
    }

    private void RankHand()
    {
        var groups = CardList
            .GroupBy(card => card.NumValue)
            .OrderByDescending(g => g.Key) // Group by rank (first character)
            .ToList();
        switch (groups.Count)
        {
            case 5: // Check to see if high card, straights, or flushes
                Rank = Check5CardHand();
                HandEval = Rank switch
                {
                    9 => $"Straight flush, {DescribeFace(CardList[0].NumValue)}-high",
                    6 => $"Flush",
                    5 => $"Straight",
                    _ => $"High card",
                };
                break;

            case 4: // One pair, since we trap on card amount != 5.
                Rank = 2;
                var pair = groups.First(g => g.Count() == 2);
                HandEval = $"One pair of {DescribeFace(pair.Key)}s";
                break;

            case 3: // Two pair or three of a kind
                Rank = CheckTwoPairOrThreeOfAKind(groups);

                HandEval = Rank switch
                {
                    3 =>
                        $"Two pair, {DescribeFace(groups.Where(g => g.Count() == 2).OrderByDescending(g => g.Key).First().Key)}s and {DescribeFace(groups.Where(g => g.Count() == 2).OrderByDescending(g => g.Key).Last().Key)}s",
                    4 =>
                        $"Three of a kind, {DescribeFace(groups.OrderByDescending(g => g.Count()).First().Key)}s",
                    _ => throw new ArgumentException("Unexpected rank value."),
                };
                break;

            case 2: // full house and four of a kind
                Rank = CheckFullHouseOrFourOfAKind(groups);
                HandEval = Rank switch
                {
                    8 =>
                        $"Four of a kind, {DescribeFace(groups.OrderByDescending(g => g.Count()).First().Key)}s",
                    7 =>
                        $"Full house, {DescribeFace(groups.OrderByDescending(g => g.Count()).First().Key)}s over {DescribeFace(groups.OrderByDescending(g => g.Count()).Last().Key)}s",
                    _ => throw new ArgumentException(
                        "Invalid hand: expected either full house or four of a kind, but found neither."
                    ),
                };
                break;

            case 1:
                throw new ArgumentException(
                    "Invalid hand: all cards are the same. Someone is cheating!"
                );
        }
    }

    private int Check5CardHand()
    {
        var isFlush = CardList.All(c => c.Suit == CardList[0].Suit);
        var isStraight =
            CardList[0].NumValue - CardList[4].NumValue == 4
            && CardList.Select(c => c.NumValue).Distinct().Count() == 5;
        if (isFlush && isStraight)
        {
            return 9; // Straight flush
        }
        else if (isFlush)
        {
            return 6; // Flush
        }
        else if (isStraight)
        {
            return 5; // Straight
        }
        return 1; // Assume High Card as no other types match
    }

    private int CheckTwoPairOrThreeOfAKind(List<IGrouping<int, Card>> groups)
    {
        if (groups.Any(g => g.Count() == 3))
        {
            return 4; // Three of a kind
        }

        return 3; // Two pair
    }

    private int CheckFullHouseOrFourOfAKind(List<IGrouping<int, Card>> groups)
    {
        if (groups.Any(g => g.Count() == 4))
        {
            return 8; // Four of a kind
        }
        else if (groups.Any(g => g.Count() == 3) && groups.Any(g => g.Count() == 2))
        {
            return 7; // Full house
        }

        throw new ArgumentException(
            "Invalid hand: expected either full house or four of a kind, but found neither."
        );
    }

    private (int result, string? highCard) CompareHighCards(PokerHand other)
    {
        string? tiebreaker = null;
        for (int i = 0; i < CardList.Length; i++)
        {
            int comparison = CardList[i].NumValue.CompareTo(other.CardList[i].NumValue);
            if (comparison != 0)
            {
                if (Rank != 1) // For non-high-card hands, annotate which kicker broke the tie.
                {
                    if (comparison < 0)
                    {
                        var highcard = other.CardList[i];
                        tiebreaker = $"{DescribeFace(highcard.NumValue)}-high card";
                    }
                    else
                    {
                        var highcard = CardList[i];
                        tiebreaker = $"{DescribeFace(highcard.NumValue)}-high card";
                    }
                }
                return (comparison, tiebreaker);
            }
        }
        return (0, null); // Hands are identical in rank and high cards
    }

    // This should allow a simple comparison of hands, first by rank, then by total value of the hand.
    public (int result, string? highCard) CompareHand(PokerHand? other)
    {
        if (other == null)
            return (1, null);

        if (Rank != other.Rank)
        {
            return (Rank.CompareTo(other.Rank), null);
        }
        else
        {
            if (Rank == 1 || Rank == 6 || Rank == 9) // For high card, flush, and straight flush, we compare high cards.
            {
                return CompareHighCards(other);
            }
            else if (Rank == 7 || Rank == 8) // For full house and four of a kind, we compare the rank of the three or four of a kind first, then high cards.
            {
                var thisGroup = CardList
                    .GroupBy(c => c.NumValue)
                    .OrderByDescending(g => g.Count())
                    .First();
                var otherGroup = other
                    .CardList.GroupBy(c => c.NumValue)
                    .OrderByDescending(g => g.Count())
                    .First();
                int comparison = thisGroup.Key.CompareTo(otherGroup.Key);
                if (comparison != 0)
                {
                    return (comparison, null);
                }
                return CompareHighCards(other);
            }
            else // For pairs and two pair, we compare the rank of the pairs first, then high cards.
            {
                var thisGroups = CardList
                    .GroupBy(c => c.NumValue)
                    .Where(g => g.Count() > 1)
                    .OrderByDescending(g => g.Key)
                    .ToList();
                var otherGroups = other
                    .CardList.GroupBy(c => c.NumValue)
                    .Where(g => g.Count() > 1)
                    .OrderByDescending(g => g.Key)
                    .ToList();
                for (int i = 0; i < thisGroups.Count; i++)
                {
                    int comparison = thisGroups[i].Key.CompareTo(otherGroups[i].Key);
                    if (comparison != 0)
                    {
                        return (comparison, null);
                    }
                }

                return CompareHighCards(other);
            }
        }
    }

    private static string DescribeFace(int value) =>
        value switch
        {
            14 => "Ace",
            13 => "King",
            12 => "Queen",
            11 => "Jack",
            _ => value.ToString(),
        };
}
