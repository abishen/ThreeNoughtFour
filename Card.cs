namespace ThreeZeroFour;

enum Suit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

enum Rank
{
    Seven = 7,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

readonly record struct Card(Suit Suit, Rank Rank)
{
    public int Points => Rank switch
    {
        Rank.Jack => 30,
        Rank.Nine => 20,
        Rank.Ace => 11,
        Rank.Ten => 10,
        Rank.King => 3,
        Rank.Queen => 2,
        _ => 0
    };

    public int TrickStrength => Rank switch
    {
        Rank.Jack => 8,
        Rank.Nine => 7,
        Rank.Ace => 6,
        Rank.Ten => 5,
        Rank.King => 4,
        Rank.Queen => 3,
        Rank.Eight => 2,
        Rank.Seven => 1,
        _ => 0
    };

    public override string ToString() => $"{RankText(Rank)}{SuitText(Suit)}";

    private static string RankText(Rank rank) => rank switch
    {
        Rank.Ace => "A",
        Rank.King => "K",
        Rank.Queen => "Q",
        Rank.Jack => "J",
        Rank.Ten => "10",
        Rank.Nine => "9",
        Rank.Eight => "8",
        Rank.Seven => "7",
        _ => "?"
    };

    private static string SuitText(Suit suit) => suit switch
    {
        Suit.Clubs => "C",
        Suit.Diamonds => "D",
        Suit.Hearts => "H",
        Suit.Spades => "S",
        _ => "?"
    };
}

static class Deck
{
    public static List<Card> Create() =>
        Enum.GetValues<Suit>()
            .SelectMany(suit => Enum.GetValues<Rank>().Select(rank => new Card(suit, rank)))
            .ToList();

    public static void Shuffle(List<Card> cards, Random random)
    {
        for (var index = cards.Count - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (cards[index], cards[swapIndex]) = (cards[swapIndex], cards[index]);
        }
    }
}

static class RulesVerifier
{
    public static void Run()
    {
        var deck = Deck.Create();
        Assert(deck.Count == 32, "Deck contains 32 cards");
        Assert(deck.Distinct().Count() == 32, "Every card is unique");
        Assert(deck.Sum(card => card.Points) == 304, "Card values total 304 points");

        var hand = new List<Card>
        {
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Hearts, Rank.Jack)
        };
        Assert(
            GameRules.LegalCards(hand, Suit.Clubs).SequenceEqual([new Card(Suit.Clubs, Rank.Ace)]),
            "A player must follow the lead suit");

        var trick = new List<Card>
        {
            new(Suit.Clubs, Rank.Jack),
            new(Suit.Clubs, Rank.Nine),
            new(Suit.Hearts, Rank.Seven)
        };
        Assert(GameRules.WinningPlayIndex(trick, Suit.Clubs, Suit.Hearts, false) == 0,
            "The Jack is strongest before trump is revealed");
        Assert(GameRules.WinningPlayIndex(trick, Suit.Clubs, Suit.Hearts, true) == 2,
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