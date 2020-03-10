﻿using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Persistency
{
    public interface IPersistencyContext : IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies,
        IDbSetCache, IDbContextPoolable
    {
        DatabaseFacade Database { get; }
    }
}