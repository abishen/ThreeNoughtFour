namespace ThreeZeroFour.Services;

interface IGameRulesService
{
    IReadOnlyList<Card> GetLegalCards(IReadOnlyCollection<Card> hand, Suit? leadSuit);
    int FindWinningCardIndex(
        IReadOnlyList<Card> playedCards,
        Suit leadSuit,
        Suit trumpSuit,
        bool trumpRevealed);
    bool IsValidBid(int bid, int highestBid);
    int GetNextBid(int highestBid);
}