internal record HandRequest(PokerHand Hand1, PokerHand Hand2);

internal class Evaluate
{
    internal static Response EvaluateRequest(HandRequest request)
    {
        var allCards = request.Hand1.Cards.Concat(request.Hand2.Cards);
        if (allCards.Count() != allCards.Distinct().Count())
            throw new ArgumentException("Duplicate cards detected across hands.");

        if (
            string.IsNullOrEmpty(request.Hand1.Player) || string.IsNullOrEmpty(request.Hand2.Player)
        )
            throw new ArgumentException("Player names cannot be null or empty.");

        var (result, highCard) = request.Hand1.CompareHand(request.Hand2);
        if (result < 0)
        {
            return new Response
            {
                Winner = request.Hand2.Player,
                Hand = string.Join(", ", request.Hand2.CardList.Select(c => c.ToString())),
                Rank = request.Hand2.Rank,
                HandEval =
                    highCard != null
                        ? $"{request.Hand2.HandEval} with {highCard}"
                        : request.Hand2.HandEval,
            };
        }
        else if (result > 0)
        {
            return new Response
            {
                Winner = request.Hand1.Player,
                Hand = string.Join(", ", request.Hand1.CardList.Select(c => c.ToString())),
                Rank = request.Hand1.Rank,
                HandEval =
                    highCard != null
                        ? $"{request.Hand1.HandEval} with {highCard}"
                        : request.Hand1.HandEval,
            };
        }
        else // equal hands, tie
        {
            return new Response
            {
                Winner = "Tie",
                Hand =
                    $"{request.Hand1.Player}: {string.Join(", ", request.Hand1.CardList.Select(c => c.ToString()))} vs {request.Hand2.Player}: {string.Join(", ", request.Hand2.CardList.Select(c => c.ToString()))}",
                Rank = request.Hand1.Rank,
                HandEval = "",
            };
        }
    }

    internal static IResult EvaluateHand(HandRequest request)
    {
        try
        {
            var response = EvaluateRequest(request);
            return Results.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
