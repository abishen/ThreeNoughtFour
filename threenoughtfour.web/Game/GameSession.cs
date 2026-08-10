using ThreeZeroFour.Services;

namespace ThreeZeroFour.Web.Game;

enum GamePhase
{
    Bidding,
    ChoosingTrump,
    Playing,
    Complete
}

readonly record struct CardPlay(Player Player, Card Card);

sealed class GameSession(
    IDeckService deckService,
    IGameRulesService rules,
    IPlayerDecisionService botDecisions)
{
    private List<Card> _deck = [];
    private int _leaderSeat;

    public IReadOnlyList<Player> Players { get; } =
    [
        new("You", true, 0),
        new("Banda", false, 1),
        new("Maya", false, 2),
        new("Ruban", false, 3)
    ];

    public Player Human => Players[0];
    public GamePhase Phase { get; private set; }
    public Contract? Contract { get; private set; }
    public Suit? TrumpSuit { get; private set; }
    public bool TrumpRevealed { get; private set; }
    public int TrickNumber { get; private set; }
    public int[] TeamPoints { get; } = new int[2];
    public List<CardPlay> CurrentTrick { get; } = [];
    public string StatusMessage { get; private set; } = "Choose your opening bid.";

    public int MinimumBid => rules.GetNextBid(150);

    public IEnumerable<int> BidOptions => Enumerable.Range(16, 15).Select(value => value * 10).Append(304);

    public void StartNewRound()
    {
        foreach (var player in Players)
        {
            player.Hand.Clear();
        }

        Array.Clear(TeamPoints);
        CurrentTrick.Clear();
        Contract = null;
        TrumpSuit = null;
        TrumpRevealed = false;
        TrickNumber = 1;
        Phase = GamePhase.Bidding;
        StatusMessage = "Choose your opening bid, or pass.";

        _deck = deckService.CreateShuffledDeck();
        deckService.DealCards(_deck, Players, 4);
        deckService.SortPlayerHands(Players);
    }

    public void SubmitHumanBid(int? humanBid)
    {
        if (Phase != GamePhase.Bidding)
        {
            return;
        }

        Player? bidder = null;
        var highestBid = 150;

        if (humanBid is not null && rules.IsValidBid(humanBid.Value, highestBid))
        {
            highestBid = humanBid.Value;
            bidder = Human;
        }

        foreach (var bot in Players.Skip(1))
        {
            var bid = botDecisions.ChooseBid(bot, highestBid);
            if (bid is null)
            {
                continue;
            }

            highestBid = bid.Value;
            bidder = bot;
        }

        if (bidder is null)
        {
            StartNewRound();
            StatusMessage = "Everyone passed. A fresh hand was dealt.";
            return;
        }

        Contract = new Contract(bidder, highestBid);
        if (bidder.IsHuman)
        {
            Phase = GamePhase.ChoosingTrump;
            StatusMessage = $"You won the auction at {highestBid}. Choose trump.";
            return;
        }

        TrumpSuit = botDecisions.ChooseTrump(bidder);
        CompleteDealAndStartPlay();
    }

    public void SelectTrump(Suit suit)
    {
        if (Phase != GamePhase.ChoosingTrump || Contract is null)
        {
            return;
        }

        TrumpSuit = suit;
        CompleteDealAndStartPlay();
    }

    public bool CanPlay(Card card) =>
        Phase == GamePhase.Playing &&
        CurrentPlayer == Human &&
        rules.GetLegalCards(Human.Hand, LeadSuit).Contains(card);

    public void PlayHumanCard(Card card)
    {
        if (!CanPlay(card))
        {
            return;
        }

        PlayCard(Human, card);
        AdvanceBotsUntilHumanTurn();
    }

    public string GetContractText() => Contract is null
        ? "Auction open"
        : $"{Contract.Value.Bidder.Name} · {Contract.Value.Bid}";

    public string GetTrumpText() => TrumpSuit is null
        ? "Hidden"
        : TrumpRevealed || Contract?.Bidder.IsHuman == true ? TrumpSuit.Value.ToString() : "Hidden";

    private Player CurrentPlayer => Players[(_leaderSeat + CurrentTrick.Count) % Players.Count];
    private Suit? LeadSuit => CurrentTrick.Count == 0 ? null : CurrentTrick[0].Card.Suit;

    private void CompleteDealAndStartPlay()
    {
        deckService.DealCards(_deck, Players, 4);
        deckService.SortPlayerHands(Players);
        Phase = GamePhase.Playing;
        _leaderSeat = Contract!.Value.Bidder.Seat;
        StatusMessage = $"{Contract.Value.Bidder.Name} leads trick 1.";
        AdvanceBotsUntilHumanTurn();
    }

    private void AdvanceBotsUntilHumanTurn()
    {
        while (Phase == GamePhase.Playing && CurrentPlayer != Human)
        {
            var bot = CurrentPlayer;
            var legalCards = rules.GetLegalCards(bot.Hand, LeadSuit);
            RevealTrumpIfPlayerCannotFollow(bot, legalCards);
            var card = botDecisions.ChooseCard(bot, legalCards, LeadSuit, TrumpSuit!.Value, TrumpRevealed);
            PlayCard(bot, card);
        }

        if (Phase == GamePhase.Playing && CurrentPlayer == Human)
        {
            StatusMessage = CurrentTrick.Count == 0
                ? "Your lead."
                : $"Your turn. Follow {LeadSuit}.";
        }
    }

    private void PlayCard(Player player, Card card)
    {
        var legalCards = rules.GetLegalCards(player.Hand, LeadSuit);
        RevealTrumpIfPlayerCannotFollow(player, legalCards);
        player.Hand.Remove(card);
        CurrentTrick.Add(new CardPlay(player, card));

        if (CurrentTrick.Count == Players.Count)
        {
            ResolveTrick();
        }
    }

    private void RevealTrumpIfPlayerCannotFollow(Player player, IReadOnlyList<Card> legalCards)
    {
        if (LeadSuit is not null && !TrumpRevealed && legalCards.Count == player.Hand.Count &&
            player.Hand.All(card => card.Suit != LeadSuit))
        {
            TrumpRevealed = true;
        }
    }

    private void ResolveTrick()
    {
        var cards = CurrentTrick.Select(play => play.Card).ToList();
        var winnerIndex = rules.FindWinningCardIndex(cards, LeadSuit!.Value, TrumpSuit!.Value, TrumpRevealed);
        var winner = CurrentTrick[winnerIndex].Player;
        var points = cards.Sum(card => card.Points);
        TeamPoints[winner.Team] += points;
        _leaderSeat = winner.Seat;
        CurrentTrick.Clear();

        if (TrickNumber == 8)
        {
            Phase = GamePhase.Complete;
            var contract = Contract!.Value;
            var madeContract = TeamPoints[contract.Bidder.Team] >= contract.Bid;
            var bidderTeam = contract.Bidder.IsHuman ? "Your team" : $"{contract.Bidder.Name}'s team";
            StatusMessage = $"{bidderTeam} {(madeContract ? "made" : "missed")} the {contract.Bid} contract.";
            return;
        }

        TrickNumber++;
        StatusMessage = $"{winner.Name} won {points} points and leads trick {TrickNumber}.";
    }
}