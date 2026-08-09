using ThreeZeroFour.Services;

namespace ThreeZeroFour.Verification;

static class RulesVerifier
{
    public static void Run(IDeckService deckService, IGameRulesService rules)
    {
        var deck = deckService.CreateShuffledDeck();
        Assert(deck.Count == 32, "Deck contains 32 cards");
        Assert(deck.Distinct().Count() == 32, "Every card is unique");
        Assert(deck.Sum(card => card.Points) == 304, "Card values total 304 points");

        var hand = new List<Card>
        {
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Hearts, Rank.Jack)
        };
        Assert(
            rules.GetLegalCards(hand, Suit.Clubs).SequenceEqual([new Card(Suit.Clubs, Rank.Ace)]),
            "A player must follow the lead suit");

        var trick = new List<Card>
        {
            new(Suit.Clubs, Rank.Jack),
            new(Suit.Clubs, Rank.Nine),
            new(Suit.Hearts, Rank.Seven)
        };
        Assert(rules.FindWinningCardIndex(trick, Suit.Clubs, Suit.Hearts, false) == 0,
            "The Jack is strongest before trump is revealed");
        Assert(rules.FindWinningCardIndex(trick, Suit.Clubs, Suit.Hearts, true) == 2,
            "A revealed trump beats the lead suit");
        Console.WriteLine("All rules checks passed.");
    }

    private static void Assert(bool condition, string description)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"Rules check failed: {description}");
        }

        Console.WriteLine($"PASS: {description}");
    }
}