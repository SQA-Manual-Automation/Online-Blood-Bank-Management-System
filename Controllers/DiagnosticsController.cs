using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BloodBankSystem.Data.Factories;

namespace BloodBankSystem.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DiagnosticsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<IActionResult> PingDb()
        {
            await using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            var result = await cmd.ExecuteScalarAsync();

            return Content(result?.ToString() == "1" ? "DB OK" : "DB FAIL");
        }
    }
}