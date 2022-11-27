namespace DoorManager.Storage.Interface.Base;

public interface IDeletable<TKey>
{
    Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default);
}
