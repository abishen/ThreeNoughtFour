namespace ThreeZeroFour.Services;

sealed class PlayerDecisionService(
    IGameConsole console,
    IGameRulesService rules) : IPlayerDecisionService
{
    public int? ChooseBid(Player player, int highestBid) =>
        player.IsHuman ? ReadHumanBid(highestBid) : ChooseBotBid(player, highestBid);

    public Suit ChooseTrump(Player bidder)
    {
        if (!bidder.IsHuman)
        {
            return bidder.Hand
                .GroupBy(card => card.Suit)
                .OrderByDescending(group => group.Sum(card => card.Points) + group.Count() * 5)
                .First().Key;
        }

        while (true)
        {
            console.Write("Choose hidden trump (C/D/H/S): ");
            var suit = console.ReadLine()?.Trim().ToUpperInvariant() switch
            {
                "C" => Suit.Clubs,
                "D" => Suit.Diamonds,
                "H" => Suit.Hearts,
                "S" => Suit.Spades,
                _ => (Suit?)null
            };

            if (suit is not null)
            {
                return suit.Value;
            }
        }
    }

    public Card ChooseCard(
        Player player,
        IReadOnlyList<Card> legalCards,
        Suit? leadSuit,
        Suit trumpSuit,
        bool trumpRevealed) =>
        player.IsHuman
            ? ReadHumanCard(player, legalCards, leadSuit)
            : ChooseBotCard(legalCards, leadSuit, trumpSuit, trumpRevealed);

    private int? ReadHumanBid(int highestBid)
    {
        while (true)
        {
            console.Write($"Your bid (pass, {rules.GetNextBid(highestBid)}-300, or 304): ");
            var input = console.ReadLine()?.Trim();
            if (string.Equals(input, "pass", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "p", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (int.TryParse(input, out var bid) && rules.IsValidBid(bid, highestBid))
            {
                return bid;
            }

            console.WriteLine("Enter 'pass' or a valid bid above the current bid.");
        }
    }

    private int? ChooseBotBid(Player player, int highestBid)
    {
        var strongestSuit = player.Hand
            .GroupBy(card => card.Suit)
            .Max(group => group.Sum(card => card.Points) + group.Count() * 8);
        var estimate = player.Hand.Sum(card => card.Points) * 2 + strongestSuit;
        var maximumBid = Math.Min(250, estimate / 10 * 10);
        var nextBid = rules.GetNextBid(highestBid);
        return nextBid <= maximumBid ? nextBid : null;
    }

    private Card ReadHumanCard(Player player, IReadOnlyList<Card> legalCards, Suit? leadSuit)
    {
        while (true)
        {
            console.WriteLine($"Your hand: {string.Join("  ", player.Hand.Select((card, index) => $"{index + 1}:{card}"))}");
            if (leadSuit is not null && legalCards.Count != player.Hand.Count)
            {
                console.WriteLine($"You must follow {leadSuit}.");
            }

            console.Write("Choose a card number: ");
            if (int.TryParse(console.ReadLine(), out var choice) && choice >= 1 && choice <= player.Hand.Count)
            {
                var card = player.Hand[choice - 1];
                if (legalCards.Contains(card))
                {
                    return card;
                }
            }

            console.WriteLine("That card cannot be played.");
        }
    }

    private static Card ChooseBotCard(
        IReadOnlyList<Card> legalCards,
        Suit? leadSuit,
        Suit trumpSuit,
        bool trumpRevealed) =>
        legalCards
            .OrderByDescending(card => trumpRevealed && card.Suit == trumpSuit)
            .ThenByDescending(card => leadSuit is not null && card.Suit == leadSuit)
            .ThenByDescending(card => card.TrickStrength)
            .First();
}