namespace ThreeZeroFour.Services;

sealed class GameService(
    bool simulation,
    IGameConsole console,
    IDeckService deckService,
    IAuctionService auctionService,
    IPlayerDecisionService decisions,
    ITrickService trickService) : IGameService
{
    private readonly List<Player> _players =
    [
        new(simulation ? "Anu" : "You", !simulation, 0),
        new("Banda", false, 1),
        new("Maya", false, 2),
        new("Ruban", false, 3)
    ];

    public void Run()
    {
        console.WriteLine("304 - Console Card Game");
        console.WriteLine($"{_players[0].Name} and Maya play against Banda and Ruban . Card notation: AS = Ace of Spades.");

        if (simulation)
        {
            PlayGameRound();
            return;
        }

        do
        {
            PlayGameRound();
            console.Write("Play another round? (y/n): ");
        }
        while (string.Equals(console.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase));
    }

    private void PlayGameRound()
    {
        Contract contract;
        List<Card> deck;

        while (true)
        {
            foreach (Player player in _players)
            {
                player.Hand.Clear();
            }

            deck = deckService.CreateShuffledDeck();
            deckService.DealCards(deck, _players, 4);
            ShowFirstPlayerHand();

            Contract? auction = auctionService.Run(_players);
            if (auction is not null)
            {
                contract = auction.Value;
                break;
            }

            console.WriteLine("Everyone passed. Redealing...\n");
        }

        Suit trumpSuit = decisions.ChooseTrump(contract.Bidder);
        console.WriteLine(contract.Bidder.IsHuman
            ? $"You selected {trumpSuit} as hidden trump."
            : $"{contract.Bidder.Name} selected a hidden trump suit.");

        deckService.DealCards(deck, _players, 4);
        deckService.SortPlayerHands(_players);
        ShowFirstPlayerHand();

        int[] teamPoints = trickService.PlayTricks(_players, contract.Bidder, trumpSuit);
        bool madeContract = teamPoints[contract.Bidder.Team] >= contract.Bid;

        console.WriteLine("\nRound result");
        console.WriteLine($"{_players[0].Name} and Maya: {teamPoints[0]} points");
        console.WriteLine($"Banda and Ruban: {teamPoints[1]} points");
        console.WriteLine($"{contract.Bidder.Name}'s team {(madeContract ? "made" : "failed")} the {contract.Bid} contract.");
    }

    private void ShowFirstPlayerHand() =>
        console.WriteLine($"\n{(_players[0].IsHuman ? "Your" : $"{_players[0].Name}'s")} hand: {string.Join("  ", _players[0].Hand)}");
}