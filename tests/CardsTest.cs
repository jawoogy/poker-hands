namespace tests;

public class Cardstest
{
    [Fact]
    public void CreationGood()
    {
        var card = new Card("2H");
        Assert.Equal(2, card.NumValue);
        Assert.Equal("H", card.Suit);

        card = new Card("10D");
        Assert.Equal(10, card.NumValue);
        Assert.Equal("D", card.Suit);

        card = new Card("AS");
        Assert.Equal(14, card.NumValue);
        Assert.Equal("S", card.Suit);

        card = new Card("KC");
        Assert.Equal(13, card.NumValue);
        Assert.Equal("C", card.Suit);

        card = new Card("JH");
        Assert.Equal(11, card.NumValue);
        Assert.Equal("H", card.Suit);  

        card = new Card("QH");
        Assert.Equal(12, card.NumValue);
        Assert.Equal("H", card.Suit);
        
    }

    [Fact]
    public void CreationBad()
    {
        Assert.Throws<ArgumentException>(() => new Card("10X"));
    }
}