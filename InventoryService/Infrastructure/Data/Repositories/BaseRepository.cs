using Common.Dto;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data.Repositories;

public class Repository<T> : IBaseRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<PaginatedResponse<T>> GetPaginatedData(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _dbSet.AsNoTracking();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<T>
        {
            Items = data,
            TotalRecords = totalCount,
            PageNo = pageNumber,
            PageSize = pageSize,
            HasPrevious = pageNumber > 1,
            HasNext = pageNumber < totalPages
        };
    }


    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
}