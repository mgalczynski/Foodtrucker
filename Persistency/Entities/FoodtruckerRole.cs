using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Persistency.Entities
{
    public class FoodtruckerRole : IdentityRole<Guid>
    {
        public const string Customer = "CUSTOMER";
        public const string FoodtruckStaff = "FOODTRUCK_STAFF";
        public const string ServiceStaff = "SERVICE_STAFF";
        public static readonly ISet<string> Roles = new HashSet<string> {Customer, FoodtruckStaff, ServiceStaff};

        internal FoodtruckerRole()
        {
        }
    }
}