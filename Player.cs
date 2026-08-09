namespace ThreeZeroFour;

sealed class Player(string name, bool isHuman, int seat)
{
    public string Name { get; } = name;
    public bool IsHuman { get; } = isHuman;
    public int Seat { get; } = seat;
    public int Team => Seat % 2;
    public List<Card> Hand { get; } = [];
}