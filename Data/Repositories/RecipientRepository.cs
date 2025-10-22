using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using BloodBankSystem.Data.Factories;
using BloodBankSystem.Data.Abstractions;
using BloodBankSystem.Models;
using System;

namespace BloodBankSystem.Data.Repositories
{
    public class RecipientRepository : IRecipientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RecipientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> RegisterRecipientAsync(Recipient recipient)
        {
            using var connection = _connectionFactory.CreateConnection();

            var hasher = new PasswordHasher<Recipient>();
            recipient.Password = hasher.HashPassword(recipient, recipient.Password);

            if (string.IsNullOrWhiteSpace(recipient.ApplicationUserId))
            {
                recipient.ApplicationUserId = Guid.NewGuid().ToString();
                Console.WriteLine("[WARN] ApplicationUserId was empty. Generated new GUID.");
            }

            var query = @"
                INSERT INTO Recipients 
                (FullName, Email, Password, ApplicationUserId)
                VALUES 
                (@FullName, @Email, @Password, @ApplicationUserId);
            ";

            var parameters = new
            {
                recipient.FullName,
                recipient.Email,
                recipient.Password,
                recipient.ApplicationUserId
            };

            try
            {
                Console.WriteLine("[DEBUG] Attempting to insert recipient:");
                Console.WriteLine($"  FullName: {recipient.FullName}");
                Console.WriteLine($"  Email: {recipient.Email}");
                Console.WriteLine($"  Password (hashed): {recipient.Password}");
                Console.WriteLine($"  ApplicationUserId: {recipient.ApplicationUserId}");

                var rowsAffected = await connection.ExecuteAsync(query, parameters);
                Console.WriteLine($"[DEBUG] Rows affected: {rowsAffected}");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Registration failed: {ex.Message}");
                return false;
            }
        }

        public async Task<int> AddAsync(Recipient recipient)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                INSERT INTO Recipients (
                    FullName, BloodGroup, Gender, DateOfBirth, ContactNumber,
                    Email, Address, Password, RequiredDate, UrgencyLevel, ApplicationUserId
                )
                VALUES (
                    @FullName, @BloodGroup, @Gender, @DateOfBirth, @ContactNumber,
                    @Email, @Address, @Password, @RequiredDate, @UrgencyLevel, @ApplicationUserId
                );
            ";

            try
            {
                Console.WriteLine("[DEBUG] Inserting full recipient record:");
                Console.WriteLine($"  Email: {recipient.Email}");
                Console.WriteLine($"  BloodGroup: {recipient.BloodGroup}");
                Console.WriteLine($"  RequiredDate: {recipient.RequiredDate}");
                Console.WriteLine($"  ApplicationUserId: {recipient.ApplicationUserId}");

                var rowsAffected = await connection.ExecuteAsync(query, recipient);
                Console.WriteLine($"[DEBUG] Rows affected: {rowsAffected}");
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddAsync failed: {ex.Message}");
                return 0;
            }
        }

        public async Task<Recipient?> LoginAsync(string email, string password)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                SELECT * FROM Recipients
                WHERE Email = @Email;
            ";

            var recipient = await connection.QuerySingleOrDefaultAsync<Recipient>(query, new { Email = email });
            if (recipient == null)
                return null;

            var hasher = new PasswordHasher<Recipient>();
            var result = hasher.VerifyHashedPassword(recipient, recipient.Password, password);

            return result == PasswordVerificationResult.Success ? recipient : null;
        }

        public async Task<Recipient?> GetRecipientByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                SELECT * FROM Recipients
                WHERE RecipientId = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Recipient>(query, new { Id = id });
        }

        public async Task<List<Recipient>> GetAllRecipientsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                SELECT * FROM Recipients
                ORDER BY RecipientId DESC;
            ";

            var recipients = await connection.QueryAsync<Recipient>(query);
            return recipients.AsList();
        }

        public async Task<bool> PostBloodRequestAsync(BloodRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                INSERT INTO BloodRequests 
                (RecipientId, BloodGroup, Quantity, Location, Message, RequestedAt, RequestDate, Status)
                VALUES 
                (@RecipientId, @BloodGroup, @Quantity, @Location, @Message, @RequestedAt, @RequestDate, @Status);
            ";

            var rowsAffected = await connection.ExecuteAsync(query, request);
            return rowsAffected > 0;
        }

        public async Task<List<BloodRequest>> GetBloodRequestsByRecipientIdAsync(int recipientId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                SELECT * FROM BloodRequests
                WHERE RecipientId = @RecipientId
                ORDER BY RequestDate DESC;
            ";

            var requests = await connection.QueryAsync<BloodRequest>(query, new { RecipientId = recipientId });
            return requests.AsList();
        }

        public async Task<BloodRequest?> GetBloodRequestByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = "SELECT * FROM BloodRequests WHERE Id = @Id;";
            return await connection.QuerySingleOrDefaultAsync<BloodRequest>(query, new { Id = id });
        }

        public async Task<bool> UpdateBloodRequestAsync(BloodRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var original = await GetBloodRequestByIdAsync(request.Id);
            if (original == null) return false;

            var query = @"
                UPDATE BloodRequests
                SET BloodGroup = @BloodGroup,
                    Quantity = @Quantity,
                    Location = @Location,
                    Message = @Message,
                    RequestDate = @RequestDate,
                    Status = @Status
                WHERE Id = @Id;
            ";

            var parameters = new
            {
                request.Id,
                request.BloodGroup,
                request.Quantity,
                request.Location,
                request.Message,
                request.RequestDate,
                Status = original.Status 
            };

            var rowsAffected = await connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteBloodRequestAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = "DELETE FROM BloodRequests WHERE Id = @Id;";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<Recipient?> GetRecipientByUserIdAsync(string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
                SELECT * FROM Recipients
                WHERE ApplicationUserId = @UserId;
            ";

            return await connection.QuerySingleOrDefaultAsync<Recipient>(query, new { UserId = userId });
        }
    }
}
