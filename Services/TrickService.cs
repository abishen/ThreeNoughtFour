namespace ThreeZeroFour.Services;

sealed class TrickService(
    IGameConsole console,
    IGameRulesService rules,
    IPlayerDecisionService decisions) : ITrickService
{
    public int[] PlayTricks(IReadOnlyList<Player> players, Player bidder, Suit trumpSuit)
    {
        var teamPoints = new int[2];
        var leaderSeat = bidder.Seat;
        var trumpRevealed = false;

        for (var trickNumber = 1; trickNumber <= 8; trickNumber++)
        {
            console.WriteLine($"\nTrick {trickNumber}");
            var plays = new List<(Player Player, Card Card)>();
            Suit? leadSuit = null;

            for (var offset = 0; offset < players.Count; offset++)
            {
                var player = players[(leaderSeat + offset) % players.Count];
                var legalCards = rules.GetLegalCards(player.Hand, leadSuit);
                if (leadSuit is not null && legalCards.Count == player.Hand.Count &&
                    player.Hand.All(card => card.Suit != leadSuit) && !trumpRevealed)
                {
                    trumpRevealed = true;
                    console.WriteLine($"Trump is revealed: {trumpSuit}.");
                }

                var card = decisions.ChooseCard(player, legalCards, leadSuit, trumpSuit, trumpRevealed);
                player.Hand.Remove(card);
                leadSuit ??= card.Suit;
                plays.Add((player, card));
                console.WriteLine($"{player.Name} plays {card}.");
            }

            var cards = plays.Select(play => play.Card).ToList();
            var winningPlay = rules.FindWinningCardIndex(cards, leadSuit!.Value, trumpSuit, trumpRevealed);
            var winner = plays[winningPlay].Player;
            var trickPoints = cards.Sum(card => card.Points);
            teamPoints[winner.Team] += trickPoints;
            leaderSeat = winner.Seat;
            console.WriteLine($"{winner.Name} wins {trickPoints} points.");
        }

        return teamPoints;
    }
}