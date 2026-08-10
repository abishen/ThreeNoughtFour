using ThreeZeroFour.Services;

namespace ThreeZeroFour.Tests;

public sealed class DeckServiceTests
{
    [Fact]
    public void CreateShuffledDeck_WhenCalled_ReturnsCompleteUniqueDeck()
    {
        DeckService service = new(new Random(42));

        List<Card> deck = service.CreateShuffledDeck();

        Assert.Equal(32, deck.Count);
        Assert.Equal(32, deck.Distinct().Count());
        Assert.Equal(304, deck.Sum(card => card.Points));
    }

    [Fact]
    public void DealCards_WhenFourCardsRequested_DealsFourCardsToEveryPlayer()
    {
        DeckService service = new(new Random(42));
        List<Card> deck = service.CreateShuffledDeck();
        List<Player> players = CreatePlayers();

        service.DealCards(deck, players, 4);

        Assert.All(players, player => Assert.Equal(4, player.Hand.Count));
        Assert.Equal(16, deck.Count);
        Assert.Equal(16, players.SelectMany(player => player.Hand).Distinct().Count());
    }

    [Fact]
    public void SortPlayerHands_WhenCardsAreUnordered_SortsBySuitThenDescendingStrength()
    {
        DeckService service = new(new Random(42));
        Player player = new("Player", true, 0);
        player.Hand.AddRange(
        [
            new Card(Suit.Hearts, Rank.Seven),
            new Card(Suit.Clubs, Rank.Nine),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Ace)
        ]);

        service.SortPlayerHands([player]);

        Assert.Equal(
        [
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Clubs, Rank.Nine),
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Hearts, Rank.Seven)
        ], player.Hand);
    }

    private static List<Player> CreatePlayers() =>
    [
        new("Player 1", true, 0),
        new("Player 2", false, 1),
        new("Player 3", false, 2),
        new("Player 4", false, 3)
    ];
}