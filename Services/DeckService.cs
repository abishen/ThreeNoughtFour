namespace ThreeZeroFour.Services;

sealed class DeckService(Random random) : IDeckService
{
    public List<Card> CreateShuffledDeck()
    {
        List<Card> cards = Enum.GetValues<Suit>()
            .SelectMany(suit => Enum.GetValues<Rank>().Select(rank => new Card(suit, rank)))
            .ToList();

        for (int index = cards.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            (cards[index], cards[swapIndex]) = (cards[swapIndex], cards[index]);
        }

        return cards;
    }

    public void DealCards(List<Card> deck, IReadOnlyCollection<Player> players, int cardsPerPlayer)
    {
        for (int cardNumber = 0; cardNumber < cardsPerPlayer; cardNumber++)
        {
            foreach (Player player in players)
            {
                player.Hand.Add(deck[^1]);
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    public void SortPlayerHands(IEnumerable<Player> players)
    {
        foreach (Player player in players)
        {
            player.Hand.Sort((left, right) =>
            {
                int suitComparison = left.Suit.CompareTo(right.Suit);
                return suitComparison != 0 ? suitComparison : right.TrickStrength.CompareTo(left.TrickStrength);
            });
        }
    }
}