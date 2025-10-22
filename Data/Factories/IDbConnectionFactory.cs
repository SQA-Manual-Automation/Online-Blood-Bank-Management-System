using System.Data;

namespace BloodBankSystem.Data.Factories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
