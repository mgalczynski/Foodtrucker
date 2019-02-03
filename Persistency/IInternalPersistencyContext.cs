using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistency.Entities;

namespace Persistency
{
    internal interface IInternalPersistencyContext : IPersistencyContext
    {
        DbSet<Foodtruck> Foodtrucks { get; set; }
        DbSet<Presence> Presences { get; set; }
        DbSet<FoodtruckOwnership> FoodtruckOwnerships { get; set; }
        DbSet<FoodtruckerUser> Users { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}