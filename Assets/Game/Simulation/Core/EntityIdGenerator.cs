public sealed class EntityIdGenerator
{
    private int _nextId = 1;

    public int Next()
    {
        var id = _nextId;
        _nextId++;
        return id;
    }
}

