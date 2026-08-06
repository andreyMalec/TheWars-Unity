public sealed class Frame
{
    public int Tick;
    public World World;

    public Frame()
    {
        Tick = 0;
        World = new World();
    }
}

