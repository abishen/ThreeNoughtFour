using ThreeZeroFour.Services;

namespace ThreeZeroFour.Web.Game;

sealed class SilentGameConsole : IGameConsole
{
    public void Write(string message)
    {
    }

    public void WriteLine(string message = "")
    {
    }

    public string? ReadLine() => null;
}