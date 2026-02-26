namespace tests;

public class ResponseTest
{
    [Fact]
    public void CreationGood()
    {
        var response = new Response
        {
            Winner = "Alice",
            Hand = "2H, 3D, 5S, 9C, KD",
            Rank = 1,
            HandEval = "High Card with King"
        };

        Assert.Equal("Alice", response.Winner);
        Assert.Equal("2H, 3D, 5S, 9C, KD", response.Hand);
        Assert.Equal(1, response.Rank);
        Assert.Equal("High Card with King", response.HandEval);
    }

    [Fact]
    public void Tie()
    {
        var req = new HandRequest(
            new PokerHand("Alice", new string[] { "2H", "3D", "5S", "9C", "KD" }),
            new PokerHand("Bob", new string[] { "2D", "3H", "5C", "9S", "KH" })
        );

        var response = Evaluate.EvaluateRequest(req);
        Assert.Equal("Tie", response.Winner);
        
    }

    [Fact]
    public void TieBreakers()
    {
        var req = new HandRequest(
            new PokerHand("Alice", new string[] { "2H", "3D", "5S", "5H", "KD" }),
            new PokerHand("Bob", new string[] { "2D", "3H", "5C", "5D", "4H" })
        );  

        var response = Evaluate.EvaluateRequest(req);
        Assert.Equal("Alice", response.Winner);
        Assert.Equal(2, response.Rank);
        Assert.Equal("One pair of 5s with King-high card", response.HandEval);

    }
}