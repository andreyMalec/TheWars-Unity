using System.Collections.Generic;

public sealed class CommandQueue
{
    private readonly Queue<ICommand> _pending = new Queue<ICommand>();

    public void Enqueue(ICommand command)
    {
        _pending.Enqueue(command);
    }

    public void ExecuteAll(Simulation simulation)
    {
        while (_pending.Count > 0)
        {
            _pending.Dequeue().Execute(simulation);
        }
    }
}

