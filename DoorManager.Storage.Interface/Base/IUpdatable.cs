namespace DoorManager.Storage.Interface.Base;

public interface IUpdatable<TKey, TValue>
    where TValue : class
{
    Task<TValue> Update(TKey key, TValue value, CancellationToken cancellationToken = default);

    Task<TValue> Upsert(TKey key, TValue value, CancellationToken cancellationToken = default);
}
