namespace ThreeZeroFour.Services;

sealed class AuctionService(
    IGameConsole console,
    IPlayerDecisionService decisions) : IAuctionService
{
    public Contract? Run(IReadOnlyList<Player> players)
    {
        console.WriteLine("\nAuction (minimum 160; bids rise in steps of 10, or bid 304):");
        Player? bidder = null;
        var highestBid = 150;

        foreach (var player in players)
        {
            var bid = decisions.ChooseBid(player, highestBid);
            if (bid is null)
            {
                console.WriteLine($"{player.Name} passes.");
                continue;
            }

            highestBid = bid.Value;
            bidder = player;
            console.WriteLine($"{player.Name} bids {highestBid}.");
        }

        return bidder is null ? null : new Contract(bidder, highestBid);
    }
}