namespace ThreeZeroFour;

sealed class Game
{
    private readonly Random _random = new();
    private readonly bool _simulation;
    private readonly List<Player> _players;

    public Game(bool simulation = false)
    {
        _simulation = simulation;
        _players =
        [
            new(simulation ? "Anu" : "You", !simulation, 0),
            new("Nimal", false, 1),
            new("Maya", false, 2),
            new("Ravi", false, 3)
        ];
    }

    public void Run()
    {
        Console.WriteLine("304 - Console Card Game");
        Console.WriteLine($"{_players[0].Name} and Maya play against Nimal and Ravi. Card notation: AS = Ace of Spades.");

        if (_simulation)
        {
            PlayRound();
            return;
        }

        do
        {
            PlayRound();
            Console.Write("Play another round? (y/n): ");
        }
        while (string.Equals(Console.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase));
    }

    private void PlayRound()
    {
        (Player Bidder, int Bid) contract;
        List<Card> deck;

        while (true)
        {
            foreach (var player in _players)
            {
                player.Hand.Clear();
            }

            deck = Deck.Create();
            Deck.Shuffle(deck, _random);
            Deal(deck, 4);
            ShowHumanHand();

            var auction = RunAuction();
            if (auction is not null)
            {
                contract = auction.Value;
                break;
            }

            Console.WriteLine("Everyone passed. Redealing...\n");
        }

        var trumpSuit = ChooseTrump(contract.Bidder);
        Console.WriteLine(contract.Bidder.IsHuman
            ? $"You selected {trumpSuit} as hidden trump."
            : $"{contract.Bidder.Name} selected a hidden trump suit.");

        Deal(deck, 4);
        SortHands();
        ShowHumanHand();

        var teamPoints = PlayTricks(contract.Bidder, trumpSuit);
        var bidderTeamPoints = teamPoints[contract.Bidder.Team];
        var madeContract = bidderTeamPoints >= contract.Bid;

        Console.WriteLine("\nRound result");
        Console.WriteLine($"{_players[0].Name} and Maya: {teamPoints[0]} points");
        Console.WriteLine($"Nimal and Ravi: {teamPoints[1]} points");
        Console.WriteLine($"{contract.Bidder.Name}'s team {(madeContract ? "made" : "failed")} the {contract.Bid} contract.");
    }

    private (Player Bidder, int Bid)? RunAuction()
    {
        Console.WriteLine("\nAuction (minimum 160; bids rise in steps of 10, or bid 304):");
        Player? bidder = null;
        var highestBid = 150;

        foreach (var player in _players)
        {
            var bid = player.IsHuman ? ReadHumanBid(highestBid) : ChooseBotBid(player, highestBid);
            if (bid is null)
            {
                Console.WriteLine($"{player.Name} passes.");
                continue;
            }

            highestBid = bid.Value;
            bidder = player;
            Console.WriteLine($"{player.Name} bids {highestBid}.");
        }

        return bidder is null ? null : (bidder, highestBid);
    }

    private int? ReadHumanBid(int highestBid)
    {
        while (true)
        {
            Console.Write($"Your bid (pass, {NextBid(highestBid)}-300, or 304): ");
            var input = Console.ReadLine()?.Trim();
            if (string.Equals(input, "pass", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "p", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (int.TryParse(input, out var bid) && IsValidBid(bid, highestBid))
            {
                return bid;
            }

            Console.WriteLine("Enter 'pass' or a valid bid above the current bid.");
        }
    }

    private int? ChooseBotBid(Player player, int highestBid)
    {
        var strongestSuit = player.Hand
            .GroupBy(card => card.Suit)
            .Max(group => group.Sum(card => card.Points) + (group.Count() * 8));
        var estimate = player.Hand.Sum(card => card.Points) * 2 + strongestSuit;
        var maximumBid = Math.Min(250, estimate / 10 * 10);
        var nextBid = NextBid(highestBid);
        return nextBid <= maximumBid ? nextBid : null;
    }

    private Suit ChooseTrump(Player bidder)
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
            Console.Write("Choose hidden trump (C/D/H/S): ");
            var input = Console.ReadLine()?.Trim().ToUpperInvariant();
            var suit = input switch
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

    private int[] PlayTricks(Player bidder, Suit trumpSuit)
    {
        var teamPoints = new int[2];
        var leaderSeat = bidder.Seat;
        var trumpRevealed = false;

        for (var trickNumber = 1; trickNumber <= 8; trickNumber++)
        {
            Console.WriteLine($"\nTrick {trickNumber}");
            var plays = new List<(Player Player, Card Card)>();
            Suit? leadSuit = null;

            for (var offset = 0; offset < 4; offset++)
            {
                var player = _players[(leaderSeat + offset) % 4];
                var legalCards = GameRules.LegalCards(player.Hand, leadSuit);
                if (leadSuit is not null && legalCards.Count == player.Hand.Count &&
                    player.Hand.All(card => card.Suit != leadSuit) && !trumpRevealed)
                {
                    trumpRevealed = true;
                    Console.WriteLine($"Trump is revealed: {trumpSuit}.");
                }

                var card = player.IsHuman
                    ? ReadHumanCard(legalCards, leadSuit)
                    : ChooseBotCard(legalCards, leadSuit, trumpSuit, trumpRevealed);
                player.Hand.Remove(card);
                leadSuit ??= card.Suit;
                plays.Add((player, card));
                Console.WriteLine($"{player.Name} plays {card}.");
            }

            var cards = plays.Select(play => play.Card).ToList();
            var winningPlay = GameRules.WinningPlayIndex(cards, leadSuit!.Value, trumpSuit, trumpRevealed);
            var winner = plays[winningPlay].Player;
            var trickPoints = cards.Sum(card => card.Points);
            teamPoints[winner.Team] += trickPoints;
            leaderSeat = winner.Seat;
            Console.WriteLine($"{winner.Name} wins {trickPoints} points.");
        }

        return teamPoints;
    }

    private Card ReadHumanCard(List<Card> legalCards, Suit? leadSuit)
    {
        while (true)
        {
            Console.WriteLine($"Your hand: {string.Join("  ", _players[0].Hand.Select((card, index) => $"{index + 1}:{card}"))}");
            if (leadSuit is not null && legalCards.Count != _players[0].Hand.Count)
            {
                Console.WriteLine($"You must follow {leadSuit}.");
            }

            Console.Write("Choose a card number: ");
            if (int.TryParse(Console.ReadLine(), out var choice) &&
                choice >= 1 && choice <= _players[0].Hand.Count)
            {
                var card = _players[0].Hand[choice - 1];
                if (legalCards.Contains(card))
                {
                    return card;
                }
            }

            Console.WriteLine("That card cannot be played.");
        }
    }

    private static Card ChooseBotCard(
        List<Card> legalCards,
        Suit? leadSuit,
        Suit trumpSuit,
        bool trumpRevealed) =>
        legalCards
            .OrderByDescending(card => trumpRevealed && card.Suit == trumpSuit)
            .ThenByDescending(card => leadSuit is not null && card.Suit == leadSuit)
            .ThenByDescending(card => card.TrickStrength)
            .First();

    private void Deal(List<Card> deck, int cardsPerPlayer)
    {
        for (var cardNumber = 0; cardNumber < cardsPerPlayer; cardNumber++)
        {
            foreach (var player in _players)
            {
                player.Hand.Add(deck[^1]);
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    private void SortHands()
    {
        foreach (var player in _players)
        {
            player.Hand.Sort((left, right) =>
            {
                var suitComparison = left.Suit.CompareTo(right.Suit);
                return suitComparison != 0 ? suitComparison : right.TrickStrength.CompareTo(left.TrickStrength);
            });
        }
    }

    private void ShowHumanHand() =>
        Console.WriteLine($"\n{(_players[0].IsHuman ? "Your" : $"{_players[0].Name}'s")} hand: {string.Join("  ", _players[0].Hand)}");

    private static bool IsValidBid(int bid, int highestBid) =>
        bid > highestBid && ((bid >= 160 && bid <= 300 && bid % 10 == 0) || bid == 304);

    private static int NextBid(int highestBid) => Math.Max(160, highestBid + 10);
}