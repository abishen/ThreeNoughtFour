namespace ThreeZeroFour;

static class GameRules
{
    public static List<Card> LegalCards(IReadOnlyCollection<Card> hand, Suit? leadSuit)
    {
        if (leadSuit is null)
        {
            return [.. hand];
        }

        var followingSuit = hand.Where(card => card.Suit == leadSuit).ToList();
        return followingSuit.Count > 0 ? followingSuit : [.. hand];
    }

    public static int WinningPlayIndex(
        IReadOnlyList<Card> playedCards,
        Suit leadSuit,
        Suit trumpSuit,
        bool trumpRevealed)
    {
        var winningIndex = 0;
        for (var index = 1; index < playedCards.Count; index++)
        {
            if (Beats(playedCards[index], playedCards[winningIndex], leadSuit, trumpSuit, trumpRevealed))
            {
                winningIndex = index;
            }
        }

        return winningIndex;
    }

    private static bool Beats(Card challenger, Card current, Suit leadSuit, Suit trumpSuit, bool trumpRevealed)
    {
        var challengerIsTrump = trumpRevealed && challenger.Suit == trumpSuit;
        var currentIsTrump = trumpRevealed && current.Suit == trumpSuit;
        if (challengerIsTrump != currentIsTrump)
        {
            return challengerIsTrump;
        }

        if (challenger.Suit != current.Suit)
        {
            return challenger.Suit == leadSuit && current.Suit != leadSuit;
        }

        return challenger.TrickStrength > current.TrickStrength;
    }
}