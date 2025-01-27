using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Workly.Application.Interfaces;
using Workly.Domain.Entities;
using Workly.Domain.Interfaces;
using Workly.Infrastructure.Persistence.Context;

namespace Workly.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly WorklyDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly IUserContext _userContext;

        public GenericRepository(WorklyDbContext dbContext, IUserContext userContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _userContext = userContext;
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<int> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.InsertedAt = DateTime.UtcNow;
                    baseEntity.InsertedBy = _userContext.UserId; // IUserContext ile kullanıcı ID'si alınıyor
                }

                await _dbSet.AddAsync(entity, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                // BaseEntity'den RecId'yi döndür
                if (entity is BaseEntity resultEntity)
                {
                    return resultEntity.RecId;
                }

                return 0; // Eğer entity BaseEntity değilse
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.UpdatedAt = DateTime.UtcNow;
                    baseEntity.UpdatedBy = _userContext.UserId;

                    _dbContext.Entry(baseEntity).Property(x => x.InsertedAt).IsModified = false;
                    _dbContext.Entry(baseEntity).Property(x => x.InsertedBy).IsModified = false;
                }

                _dbSet.Update(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                throw;
            }
        }

        public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                if (entity == null) throw new Exception($"{id} numaralı Kayıt bulunamadı.");
                
                _dbSet.Remove(entity);
                
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entities is IEnumerable<BaseEntity> baseEntities)
                {
                    foreach (var entity in baseEntities)
                    {
                        entity.InsertedAt = DateTime.UtcNow;
                        entity.InsertedBy = _userContext.UserId;
                    }
                }

                await _dbSet.AddRangeAsync(entities, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                throw;
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entities is IEnumerable<BaseEntity> baseEntities)
                {
                    foreach (var entity in baseEntities)
                    {
                        entity.UpdatedAt = DateTime.UtcNow;
                        entity.UpdatedBy = _userContext.UserId;
                    }
                }

                _dbSet.UpdateRange(entities);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                throw;
            }
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                throw;
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
