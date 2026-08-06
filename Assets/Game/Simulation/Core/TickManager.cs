using System;

public sealed class TickManager
{
    private readonly float _tickDeltaTime;
    private float _accumulator;

    public TickManager(int tickRate)
    {
        _tickDeltaTime = 1f / tickRate;
        _accumulator = 0f;
    }

    public void Advance(float deltaTime, Action onTick)
    {
        _accumulator += deltaTime;

        while (_accumulator >= _tickDeltaTime)
        {
            _accumulator -= _tickDeltaTime;
            onTick();
        }
    }
}

