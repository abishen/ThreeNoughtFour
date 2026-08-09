# 304 Console Card Game

A playable C# console version of the four-player 304 card game. You partner with Maya against three computer-controlled players: Nimal and Ravi are the opposing team.

## Run

```sh
dotnet run
```

Enter `p` or `pass` during the auction, or enter a legal bid. Choose trump with `C`, `D`, `H`, or `S`, then play cards by entering their displayed number.

Useful validation commands:

```sh
dotnet run -- --self-test
dotnet run -- --simulate
```

## Implemented Variant

- The 32-card deck uses ranks 7 through Ace in four suits.
- Card points are J=30, 9=20, A=11, 10=10, K=3, Q=2, 8=0, and 7=0. The deck totals 304 points.
- Trick strength is J, 9, A, 10, K, Q, 8, 7 from highest to lowest.
- Four players form fixed opposite-seat partnerships.
- Each player receives four cards before bidding and four after bidding.
- The minimum bid is 160. Bids increase by 10 through 300; 304 is also legal.
- The winning bidder chooses a hidden trump suit and leads the first trick.
- Players must follow the lead suit when possible. Trump is revealed automatically when a player first cannot follow suit, and it applies from that trick onward.
- The bidder's team must collect at least its bid from the eight tricks to make the contract.

Regional 304 rules differ, especially around bidding rounds, trump reveal, and penalties. This project states its variant explicitly so those rules can be adjusted in `Game.cs` and `GameRules.cs`.
