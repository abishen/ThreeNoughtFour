using ThreeZeroFour.Services;
using ThreeZeroFour.Verification;

namespace ThreeZeroFour;

class Program
{
    static void Main(string[] args)
    {
        IGameConsole console = new ConsoleGameConsole();
        IDeckService deckService = new DeckService(new Random());
        IGameRulesService rules = new GameRulesService();

        if (args.Contains("--self-test", StringComparer.OrdinalIgnoreCase))
        {
            RulesVerifier.Run(deckService, rules);
            return;
        }

        var simulation = args.Contains("--simulate", StringComparer.OrdinalIgnoreCase);
        IPlayerDecisionService decisions = new PlayerDecisionService(console, rules);
        IAuctionService auctionService = new AuctionService(console, decisions);
        ITrickService trickService = new TrickService(console, rules, decisions);
        IGameService gameService = new GameService(
            simulation,
            console,
            deckService,
            auctionService,
            decisions,
            trickService);
        gameService.Run();
    }
}
