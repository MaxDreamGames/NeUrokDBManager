using System.Drawing;
using System.Linq.Expressions;
using NeUrokDBManager.Core.DTOs;
using NeUrokDBManager.Core.Entities;

namespace NeUrokDBManager.Core.Interfaces.Reposoitories
{
    public interface IClientRepository
    {
        Task AddAsync(Client client, CancellationToken ct = default);
        Task DeleteAsync(Client client, CancellationToken ct = default);
        Task DeleteByUserIdAsync(int userId, CancellationToken ct = default);
        Task UpdateAsync(int userId, Expression<Func<Client, object>> selector, object value, CancellationToken ct = default);
        Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default);
        Task<Client?> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<List<Client>> GetAllAsync(CancellationToken ct = default);
        Task<int> GetNextUserIdAsync(CancellationToken ct = default);
        Task<List<Client>> SearchAsync(ClientSearchDTO request, CancellationToken ct = default);
        Task<bool> ChangeColorAsync(int userId, ClientColor clientColor, CancellationToken ct = default);
        Task<Dictionary<int, Color>> GetColorsAsync(CancellationToken ct = default);
        Task<bool> UpdateClassesAsync(CancellationToken ct = default);
        List<BirthdayDTO> GetClosestBirthdays();

    }
}
