# Three Nought Four Card Game

A playable C# implementation of the four-player 304 card game with console and browser interfaces. You play with Maya as your partner against the computer-controlled Banda and Ruban. The application includes bidding, hidden trump selection, legal-card validation, eight-trick rounds, bot decisions, contract scoring, automated simulations, and unit tests.

## Prerequisites

The application targets **.NET 10**. Install the .NET 10 SDK, which includes everything required to restore, build, run, and test the project.

### macOS

Install with [Homebrew](https://brew.sh/):

```sh
brew install --cask dotnet-sdk
```

Alternatively, download the macOS SDK from the [.NET 10 download page](https://dotnet.microsoft.com/download/dotnet/10.0).

### Windows

Install with Windows Package Manager:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Alternatively, download the Windows installer from the [.NET 10 download page](https://dotnet.microsoft.com/download/dotnet/10.0).

### Linux

Follow the [.NET installation guide for your Linux distribution](https://learn.microsoft.com/dotnet/core/install/linux) and install the .NET 10 SDK package.

Verify the installation:

```sh
dotnet --version
```

The reported version should begin with `10.`.

## Run the Application

Open a terminal in the repository root, the directory containing `threenoughtfour` and `threenoughtfour.tests`.

Restore dependencies and build the application:

```sh
dotnet restore threenoughtfour/ThreeZeroFour.csproj
dotnet build threenoughtfour/ThreeZeroFour.csproj
```

Start an interactive game:

```sh
dotnet run --project threenoughtfour/ThreeZeroFour.csproj
```

Enter `p` or `pass` during the auction, or enter a legal bid. Choose trump with `C`, `D`, `H`, or `S`, then play cards by entering their displayed number.

### Browser UI

Start the interactive Blazor game:

```sh
dotnet run --project threenoughtfour.web/ThreeZeroFour.Web.csproj
```

Open the local address printed by .NET, usually `http://localhost:5000` or `https://localhost:5001`. Use the on-screen controls to bid, choose trump, and play highlighted legal cards. The layout supports desktop and mobile screens.

## Validation and Tests

Run an automated game with four computer-controlled players:

```sh
dotnet run --project threenoughtfour/ThreeZeroFour.csproj -- --simulate
```

Run the built-in rules checks:

```sh
dotnet run --project threenoughtfour/ThreeZeroFour.csproj -- --self-test
```

Run the xUnit test suite:

```sh
dotnet test threenoughtfour.tests/ThreeZeroFour.Tests.csproj
```

Unit tests use the `Method_WhenCondition_ExpectedResult` naming convention so failures describe the behavior under test without opening the test body.

## Architecture

`Program.cs` is the composition root. It creates the service implementations and supplies their dependencies through constructors.

- `GameService` coordinates rounds without implementing deck, auction, player, or trick policies.
- `DeckService` owns deck creation, shuffling, dealing, and hand sorting.
- `AuctionService` coordinates bidding through the player decision abstraction.
- `TrickService` coordinates legal plays, trick winners, and team points.
- `GameRulesService` contains deterministic game and bidding rules.
- `PlayerDecisionService` contains human input and bot decision policies.
- `IGameConsole` isolates console I/O so another interface can replace it.

Each service implements a focused interface under `Services/`, keeping high-level game flow dependent on abstractions rather than concrete implementations.

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

Regional 304 rules differ, especially around bidding rounds, trump reveal, and penalties. This project states its variant explicitly so those policies can be adjusted in `Services/GameRulesService.cs` and `Services/AuctionService.cs`.
