namespace ThreeZeroFour.Services;

interface IGameConsole
{
    void Write(string message);
    void WriteLine(string message = "");
    string? ReadLine();
}