using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Persistency.Entities;

namespace Persistency
{
    internal interface IInternalPersistencyContext : IPersistencyContext
    {
        DbSet<Foodtruck> Foodtrucks { get; set; }
        DbSet<Presence> Presences { get; set; }
    }
}