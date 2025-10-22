using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBankSystem.Models;

namespace BloodBankSystem.Data.Abstractions
{
    public interface IRecipientRepository
    {
        Task<bool> RegisterRecipientAsync(Recipient recipient);
        Task<Recipient?> LoginAsync(string email, string password);
        Task<Recipient?> GetRecipientByIdAsync(int id);
        Task<bool> PostBloodRequestAsync(BloodRequest request);

        Task<List<BloodRequest>> GetBloodRequestsByRecipientIdAsync(int recipientId);

        Task<BloodRequest?> GetBloodRequestByIdAsync(int id);

        Task<bool> UpdateBloodRequestAsync(BloodRequest request);

        Task<bool> DeleteBloodRequestAsync(int id);

        Task<List<Recipient>> GetAllRecipientsAsync();

        Task<int> AddAsync(Recipient recipient);

        Task<Recipient?> GetRecipientByUserIdAsync(string userId);
    }
}
