using ThreeZeroFour.Services;

namespace ThreeZeroFour.Tests;

public sealed class GameRulesServiceTests
{
    private readonly GameRulesService _service = new();

    [Fact]
    public void GetLegalCards_WhenNoSuitHasBeenLed_ReturnsEntireHand()
    {
        Card[] hand =
        [
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Hearts, Rank.Jack)
        ];

        var legalCards = _service.GetLegalCards(hand, leadSuit: null);

        Assert.Equal(hand, legalCards);
    }

    [Fact]
    public void GetLegalCards_WhenLeadSuitIsAvailable_ReturnsOnlyCardsFromLeadSuit()
    {
        Card[] hand =
        [
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Clubs, Rank.Seven),
            new(Suit.Hearts, Rank.Jack)
        ];

        var legalCards = _service.GetLegalCards(hand, Suit.Clubs);

        Assert.Equal(2, legalCards.Count);
        Assert.All(legalCards, card => Assert.Equal(Suit.Clubs, card.Suit));
    }

    [Fact]
    public void GetLegalCards_WhenLeadSuitIsUnavailable_ReturnsEntireHand()
    {
        Card[] hand =
        [
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Hearts, Rank.Jack)
        ];

        var legalCards = _service.GetLegalCards(hand, Suit.Spades);

        Assert.Equal(hand, legalCards);
    }

    [Fact]
    public void FindWinningCardIndex_WhenTrumpIsHidden_SelectsStrongestCardFromLeadSuit()
    {
        Card[] playedCards =
        [
            new(Suit.Clubs, Rank.Nine),
            new(Suit.Hearts, Rank.Jack),
            new(Suit.Clubs, Rank.Jack),
            new(Suit.Clubs, Rank.Ace)
        ];

        var winningIndex = _service.FindWinningCardIndex(
            playedCards,
            Suit.Clubs,
            Suit.Hearts,
            trumpRevealed: false);

        Assert.Equal(2, winningIndex);
    }

    [Fact]
    public void FindWinningCardIndex_WhenCardsShareSuit_SelectsCardWithHighestStrength()
    {
        Card[] playedCards =
        [
            new(Suit.Clubs, Rank.Seven),
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Clubs, Rank.Jack),
            new(Suit.Clubs, Rank.Nine)
        ];

        var winningIndex = _service.FindWinningCardIndex(
            playedCards,
            Suit.Clubs,
            Suit.Hearts,
            trumpRevealed: false);

        Assert.Equal(2, winningIndex);
    }

    [Fact]
    public void FindWinningCardIndex_WhenTrumpIsRevealed_SelectsStrongestTrumpCard()
    {
        Card[] playedCards =
        [
            new(Suit.Clubs, Rank.Jack),
            new(Suit.Hearts, Rank.Seven),
            new(Suit.Hearts, Rank.Nine),
            new(Suit.Clubs, Rank.Ace)
        ];

        var winningIndex = _service.FindWinningCardIndex(
            playedCards,
            Suit.Clubs,
            Suit.Hearts,
            trumpRevealed: true);

        Assert.Equal(2, winningIndex);
    }

    [Theory]
    [InlineData(160, 150, true)]
    [InlineData(170, 160, true)]
    [InlineData(304, 300, true)]
    [InlineData(160, 160, false)]
    [InlineData(165, 160, false)]
    [InlineData(310, 304, false)]
    public void IsValidBid_WhenBidIsEvaluated_ReturnsExpectedResult(
        int bid,
        int highestBid,
        bool expectedResult)
    {
        var result = _service.IsValidBid(bid, highestBid);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(0, 160)]
    [InlineData(150, 160)]
    [InlineData(160, 170)]
    [InlineData(290, 300)]
    public void GetNextBid_WhenHighestBidIsProvided_ReturnsNextLegalIncrement(
        int highestBid,
        int expectedBid)
    {
        var nextBid = _service.GetNextBid(highestBid);

        Assert.Equal(expectedBid, nextBid);
    }
}