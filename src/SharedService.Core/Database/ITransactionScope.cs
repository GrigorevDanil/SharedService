using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace SharedService.Core.Database;

/// <summary>
/// Интерфейс обертки транзакции.
/// </summary>
public interface ITransactionScope : IDisposable
{
    /// <summary>
    /// Сохраняет изменения в базу данных.
    /// </summary>
    /// <returns>Ошибка <see cref="Error"/>.</returns>
    public UnitResult<Error> Commit();

    /// <summary>
    /// Откатывает изменения в транзакции.
    /// </summary>
    /// <returns>Ошибка <see cref="Error"/>.</returns>
    public UnitResult<Error> Rollback();
}