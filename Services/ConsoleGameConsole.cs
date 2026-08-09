namespace ThreeZeroFour.Services;

sealed class ConsoleGameConsole : IGameConsole
{
    public void Write(string message) => Console.Write(message);
    public void WriteLine(string message = "") => Console.WriteLine(message);
    public string? ReadLine() => Console.ReadLine();
}