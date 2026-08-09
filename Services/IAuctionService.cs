namespace ThreeZeroFour.Services;

interface IAuctionService
{
    Contract? Run(IReadOnlyList<Player> players);
}

readonly record struct Contract(Player Bidder, int Bid);