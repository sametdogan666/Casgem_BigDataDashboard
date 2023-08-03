﻿using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace BigDataDashboard.DAL;

public class CarRepository
{
    private readonly string _connectionString;
    private readonly IMemoryCache _memoryCache;


    public CarRepository(IConfiguration configuration, IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    
    public async Task<IEnumerable<Car>> SearchCarsAsync(string searchString)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        var query =
            $"SELECT * FROM PLATES WHERE BRAND LIKE @search";
        return await connection.QueryAsync<Car>(query, new { search = $"%{searchString}%" });
    }

    public async Task<int> GetBenzinliAracSayisiAsync()
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var cacheKey = "BenzinliAracSayisi";
        if (_memoryCache.TryGetValue(cacheKey, out int count))
        {
            return count;
        }

        connection.Open();
        count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM PLATES WHERE FUEL = 'Benzin'");

        // Veriyi önbelleğe al
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Önbellekte 5 dakika tut
        };

        _memoryCache.Set(cacheKey, count, cacheEntryOptions);

        return count;
    }

}
