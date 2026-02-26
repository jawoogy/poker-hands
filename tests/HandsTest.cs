namespace tests;

public class Handstest
{
    [Fact]
    public void CreationGood()
    {
        var hand = new PokerHand("Alice", new string[] { "2H", "3D", "5S", "9C", "KD" });
        Assert.Equal("Alice", hand.Player);
        Assert.Equal(5, hand.CardList.Length);
        Assert.Equal(13, hand.CardList[0].NumValue);
        Assert.Equal(9, hand.CardList[1].NumValue);
        Assert.Equal(5, hand.CardList[2].NumValue);
        Assert.Equal(3, hand.CardList[3].NumValue);
        Assert.Equal(2, hand.CardList[4].NumValue);
        Assert.Equal("D", hand.CardList[0].Suit);
        Assert.Equal("C", hand.CardList[1].Suit);
        Assert.Equal("S", hand.CardList[2].Suit);
        Assert.Equal("D", hand.CardList[3].Suit);
        Assert.Equal("H", hand.CardList[4].Suit);

    }

    [Fact]
    public void CreationBad()
    {
        Assert.Throws<ArgumentException>(() => new PokerHand("Bob", new string[] { "2H", "3D" }));
    }

    [Fact]
    public void RankHand()
    {
        // One pair hand
        var onePair = new PokerHand("Alice", new string[] { "2H", "2D", "5S", "9C", "KD" });
        Assert.Equal(2, onePair.Rank);

        // High card hand
        var highCard = new PokerHand("Bob", new string[] { "2H", "3D", "5S", "9C", "KD" });
        Assert.Equal(1, highCard.Rank);

        // Two pair hand
        var twoPair = new PokerHand("Charlie", new string[] { "2H", "2D", "5S", "5C", "KD" });
        Assert.Equal(3, twoPair.Rank);

        //  Three of a kind hand
        var threeKind = new PokerHand("Dave", new string[] { "2H", "2D", "2S", "5C", "KD" });
        Assert.Equal(4, threeKind.Rank);

        // Four of a kind hand
        var fourKind = new PokerHand("Eve", new string[] { "2H", "2D", "2S", "2C", "KD" });
        Assert.Equal(8, fourKind.Rank); 

        // Straight hand
        var straight = new PokerHand("Frank", new string[] { "2H", "3D", "4S", "5C", "6D" });
        Assert.Equal(5, straight.Rank);

        // Flush hand
        var flush = new PokerHand("Grace", new string[] { "2H", "5H", "7H", "9H", "KH" });
        Assert.Equal(6, flush.Rank);

        // Straight flush hand
        var straightFlush = new PokerHand("Heidi", new string[] { "2H", "3H", "4H", "5H", "6H" });
        Assert.Equal(9, straightFlush.Rank);

        // Full house hand
        var fullHouse = new PokerHand("Ivan", new string[] { "2H", "2D", "2S", "5C", "5D" });
        Assert.Equal(7, fullHouse.Rank);

    }

    [Fact]
    public void CompareHands()
    {
        var hand1 = new PokerHand("Alice", new string[] { "2H", "3D", "5S", "9C", "KD" });
        var hand2 = new PokerHand("Bob", new string[] { "2H", "3D", "5S", "9C", "KD" });
        var res = hand1.CompareHand(hand2);
        Assert.Equal(0, res.result);

    }

    [Fact]
    public void CompareHandsDifferentRanks()
    {
        var hand1 = new PokerHand("Alice", new string[] { "2H", "3D", "5S", "9C", "KD" });
        var hand2 = new PokerHand("Bob", new string[] { "2H", "2D", "5S", "9C", "KD" });
        Assert.Equal(-1, hand1.CompareHand(hand2).result);
    }

    [Fact]
    public void CompareHandsSameRankDifferentHighCards()
    {
        var hand1 = new PokerHand("Alice", new string[] { "2H", "3D", "5S", "9C", "KD" });
        var hand2 = new PokerHand("Bob", new string[] { "2H", "3D", "5S", "9C", "AD" });
        Assert.Equal(-1, hand1.CompareHand(hand2).result);
    }
}
