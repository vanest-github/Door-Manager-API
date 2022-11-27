using System.Linq.Expressions;

namespace DoorManager.Storage.Interface.Base;

public interface ISearchable<TValue>
    where TValue : class
{
    Task<IEnumerable<TValue>> SearchByPredicateAsync(Expression<Func<TValue, bool>> predicate, CancellationToken cancellationToken = default);
}
