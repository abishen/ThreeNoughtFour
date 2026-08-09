namespace ThreeZeroFour;

class Program
{
    static void Main(string[] args)
    {
        if (args.Contains("--self-test", StringComparer.OrdinalIgnoreCase))
        {
            RulesVerifier.Run();
            return;
        }

        var simulation = args.Contains("--simulate", StringComparer.OrdinalIgnoreCase);
        new Game(simulation).Run();
    }
}
