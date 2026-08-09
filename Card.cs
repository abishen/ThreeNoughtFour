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