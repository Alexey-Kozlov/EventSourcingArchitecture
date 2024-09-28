using CQRSCore.Events;

namespace CQRSCore.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    public Guid Id => _id;
    public int Version { get; set; } = -1;
    public IEnumerable<BaseEvent> GetUncommitedChanges()
    {
        return _changes;
    }
    public void ClearUncommitedChanges()
    {
        _changes.Clear();
    }
    private void ApplyChanges(BaseEvent _event, bool isNew)
    {
        var method = this.GetType().GetMethod("Apply", new Type[] { _event.GetType() });
        if (method == null)
        {
            throw new ArgumentNullException($"Apply-метод не найден в Aggregate для {_event.GetType()}");
        }
        method.Invoke(this, new object[] { _event });
        if (isNew)
        {
            _changes.Add(_event);
        }
    }
    protected void RaiseEvent(BaseEvent _event)
    {
        ApplyChanges(_event, true);
    }
    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var _event in events)
        {
            ApplyChanges(_event, false);
        }
    }
}