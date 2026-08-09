namespace ThreeZeroFour.Services;

interface IDeckService
{
    List<Card> CreateShuffledDeck();
    void DealCards(List<Card> deck, IReadOnlyCollection<Player> players, int cardsPerPlayer);
    void SortPlayerHands(IEnumerable<Player> players);
}