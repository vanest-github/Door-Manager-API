namespace DoorManager.Storage.Interface.Base;

public interface IReadable<TKey, TValue>
    where TValue : class
{
    Task<TValue> GetAsync(TKey key, CancellationToken cancellationToken = default);
}
