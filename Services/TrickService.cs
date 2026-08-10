namespace ThreeZeroFour.Services;

sealed class TrickService(
    IGameConsole console,
    IGameRulesService rules,
    IPlayerDecisionService decisions) : ITrickService
{
    public int[] PlayTricks(IReadOnlyList<Player> players, Player bidder, Suit trumpSuit)
    {
        int[] teamPoints = new int[2];
        int leaderSeat = bidder.Seat;
        bool trumpRevealed = false;

        for (int trickNumber = 1; trickNumber <= 8; trickNumber++)
        {
            console.WriteLine($"\nTrick {trickNumber}");
            List<(Player Player, Card Card)> plays = new();
            Suit? leadSuit = null;

            for (int offset = 0; offset < players.Count; offset++)
            {
                Player player = players[(leaderSeat + offset) % players.Count];
                IReadOnlyList<Card> legalCards = rules.GetLegalCards(player.Hand, leadSuit);
                if (leadSuit is not null && legalCards.Count == player.Hand.Count &&
                    player.Hand.All(card => card.Suit != leadSuit) && !trumpRevealed)
                {
                    trumpRevealed = true;
                    console.WriteLine($"Trump is revealed: {trumpSuit}.");
                }

                Card card = decisions.ChooseCard(player, legalCards, leadSuit, trumpSuit, trumpRevealed);
                player.Hand.Remove(card);
                leadSuit ??= card.Suit;
                plays.Add((player, card));
                console.WriteLine($"{player.Name} plays {card}.");
            }

            List<Card> cards = plays.Select(play => play.Card).ToList();
            int winningPlay = rules.FindWinningCardIndex(cards, leadSuit!.Value, trumpSuit, trumpRevealed);
            Player winner = plays[winningPlay].Player;
            int trickPoints = cards.Sum(card => card.Points);
            teamPoints[winner.Team] += trickPoints;
            leaderSeat = winner.Seat;
            console.WriteLine($"{winner.Name} wins {trickPoints} points.");
        }

        return teamPoints;
    }
}