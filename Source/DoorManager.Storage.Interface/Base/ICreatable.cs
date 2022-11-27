namespace DoorManager.Storage.Interface.Base;

public interface ICreatable<TValue>
    where TValue : class
{
    Task<TValue> CreateAsync(TValue value, CancellationToken cancellationToken = default);
}
