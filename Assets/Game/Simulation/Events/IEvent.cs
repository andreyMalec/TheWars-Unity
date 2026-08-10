public interface IEvent {
}

public interface IEventListener<in T> where T : IEvent {
    public void OnEvent(T e);
}