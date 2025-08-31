using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NeUrokDBManager.Core.DTOs;
using NeUrokDBManager.Core.Entities;
using NeUrokDBManager.Core.Interfaces.Reposoitories;

namespace NeUrokDBManager.Infrastructure.Reposoitories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Client client, CancellationToken ct = default)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync(ct);
        }
        public async Task<bool> ChangeColorAsync(int userId, ClientColor clientColor, CancellationToken ct = default)
        {
            try
            {
                var client = await _context.Clients
                    .Include(c => c.ClientColor)
                    .FirstOrDefaultAsync(c => c.UserId == userId, ct);

                if (client == null)
                    return false;

                if (client.ClientColor != null)
                {
                    _context.ClientColors.Remove(client.ClientColor);
                }

                if (clientColor.Color.Name != "ffffffff")
                {
                    clientColor.ClientId = client.Id;
                    _context.ClientColors.Add(clientColor);
                }

                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public List<BirthdayDTO> GetClosestBirthdays()
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(3);

            return _context.Clients
                .Where(c => c.Birthday != null)
                .AsEnumerable() // Переключаемся на клиентскую обработку для сложных вычислений
                .Where(c => IsBirthdayInRange(c.Birthday!.Value, today, endDate))
                .Select(c => new BirthdayDTO(
                    c.StudentName,
                    (byte)(today.Year - c.Birthday.Value.Year),
                    (byte)(c.Birthday.Value.Day - today.Day)))

                .ToList();
        }

        private bool IsBirthdayInRange(DateTime birthday, DateTime startDate, DateTime endDate)
        {
            // Сравниваем только день и месяц
            var currentYearBirthday = new DateTime(startDate.Year, birthday.Month, birthday.Day);

            // Учитываем случай, когда день рождения в январе следующего года
            if (currentYearBirthday < startDate)
            {
                currentYearBirthday = currentYearBirthday.AddYears(1);
            }

            return currentYearBirthday >= startDate && currentYearBirthday <= endDate;
        }


        public async Task DeleteAsync(Client client, CancellationToken ct = default)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var client = await GetByUserIdAsync(userId, ct);
            if (client != null)
            {
                await DeleteAsync(client, ct);
                await UpdateUserIds(userId);
            }
        }

        public async Task<List<Client>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default)
        {
            return await _context.Clients.FindAsync(clientId, ct);
        }

        public async Task<Client?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        }

        public async Task<Dictionary<int, Color>> GetColorsAsync(CancellationToken ct = default)
        {
            var dict = new Dictionary<int, Color>();
            foreach (var color in _context.ClientColors)
            {
                var user = await GetByIdAsync(color.ClientId, ct);
                if (user != null)
                    dict.Add(user.UserId, color.Color);
            }
            return dict;
        }

        public async Task<int> GetNextUserIdAsync(CancellationToken ct = default)
        {
            return await _context.Clients.CountAsync(ct) + 1;
        }

        public async Task<List<Client>> SearchAsync(ClientSearchDTO request, CancellationToken ct = default)
        {
            IQueryable<Client> query = _context.Clients.AsQueryable();

            if (request.UserId > 0)
                query = query.Where(c => c.UserId == request.UserId);

            if (!string.IsNullOrEmpty(request.StudentName))
                query = query.Where(c => EF.Functions.Like(c.StudentName, $"%{request.StudentName}%"));

            if (request.BirthdayDay > 0)
                query = query.Where(c => c.Birthday.HasValue && c.Birthday.Value.Day == request.BirthdayDay);
            if (request.BirthdayMonth > 0)
                query = query.Where(c => c.Birthday.HasValue && c.Birthday.Value.Month == request.BirthdayMonth);
            if (request.BirthdayYear > 0)
                query = query.Where(c => c.Birthday.HasValue && c.Birthday.Value.Year == request.BirthdayYear);

            if (request.RegistrationDateDay > 0)
                query = query.Where(c => c.RegistrationDate.Day == request.RegistrationDateDay);
            if (request.RegistrationDateMonth > 0)
                query = query.Where(c => c.RegistrationDate.Month == request.RegistrationDateMonth);
            if (request.RegistrationDateYear > 0)
                query = query.Where(c => c.RegistrationDate.Year == request.RegistrationDateYear);

            if (request.Class.HasValue)
                query = query.Where(c => c.Class == request.Class);

            if (!string.IsNullOrEmpty(request.Courses))
                query = query.Where(c => !string.IsNullOrEmpty(c.Courses) && EF.Functions.Like(c.Courses, $"%{request.Courses}%"));

            if (!string.IsNullOrEmpty(request.ParentName))
                query = query.Where(c => !string.IsNullOrEmpty(c.ParentName) && EF.Functions.Like(c.ParentName, $"%{request.ParentName}%"));

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                query = query.Where(c => !string.IsNullOrEmpty(c.PhoneNumber) && EF.Functions.Like(c.PhoneNumber, $"%{request.PhoneNumber}%"));

            if (!string.IsNullOrEmpty(request.AnotherPhoneNumber))
                query = query.Where(c => !string.IsNullOrEmpty(c.AnotherPhoneNumber) && EF.Functions.Like(c.AnotherPhoneNumber, $"%{request.AnotherPhoneNumber}%"));

            if (!string.IsNullOrEmpty(request.Comments))
                query = query.Where(c => !string.IsNullOrEmpty(c.Comments) && EF.Functions.Like(c.Comments, $"%{request.Comments}%"));

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(int userId, Expression<Func<Client, object>> selector, object value, CancellationToken ct = default)
        {
            var client = await GetByUserIdAsync(userId, ct);
            if (client is null) return;

            var memberExpr = (MemberExpression)(selector.Body is UnaryExpression ue ? ue.Operand : selector.Body);
            var property = (PropertyInfo)memberExpr.Member;

            // Специальная обработка для DateTime?
            if (property.PropertyType == typeof(DateTime?))
            {
                DateTime? dateValue = null;
                if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                {
                    if (DateTime.TryParseExact(strValue, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        dateValue = parsedDate;
                    }
                }
                property.SetValue(client, dateValue);
            }
            else
            {
                property.SetValue(client, Convert.ChangeType(value, property.PropertyType));
            }

            await _context.SaveChangesAsync(ct);
        }

        private async Task UpdateUserIds(int deletedUserId)
        {
            var clientsToUpdate = await _context.Clients
                .Where(c => c.UserId > deletedUserId)
                .OrderBy(c => c.UserId)
                .ToListAsync();

            foreach (var client in clientsToUpdate)
                client.UserId--;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateClassesAsync(CancellationToken ct = default)
        {
            if (_context.Updates.Any(u => u.UpdateingDate.Year == DateTime.Now.Year)) return false;
            await _context.Clients.ForEachAsync(c => c.Class++);
            await _context.Updates.AddAsync(new Update
            {
                Id = Guid.NewGuid(),
                UpdateingDate = DateTime.Now,
            });
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
