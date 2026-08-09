namespace ThreeZeroFour.Services;

interface IPlayerDecisionService
{
    int? ChooseBid(Player player, int highestBid);
    Suit ChooseTrump(Player bidder);
    Card ChooseCard(Player player, IReadOnlyList<Card> legalCards, Suit? leadSuit, Suit trumpSuit, bool trumpRevealed);
}