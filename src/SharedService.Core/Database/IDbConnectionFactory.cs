using System.Data.Common;

namespace SharedService.Core.Database;

public interface IDbConnectionFactory
{
    DbConnection GetDbConnection();
}