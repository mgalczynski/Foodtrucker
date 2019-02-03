using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Identity;

namespace Persistency.Entities
{
    public class FoodtruckerRole : IdentityRole<Guid>
    {
        public const string Customer = "CUSTOMER";
        public const string FoodtruckStaff = "FOODTRUCK_STAFF";
        public const string ServiceStaff = "SERVICE_STAFF";
        public static readonly ISet<string> Roles = ImmutableHashSet.Create(Customer, FoodtruckStaff, ServiceStaff);

        internal FoodtruckerRole()
        {
        }
    }
}