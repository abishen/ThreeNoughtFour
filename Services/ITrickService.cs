namespace ThreeZeroFour.Services;

interface ITrickService
{
    int[] PlayTricks(IReadOnlyList<Player> players, Player bidder, Suit trumpSuit);
}