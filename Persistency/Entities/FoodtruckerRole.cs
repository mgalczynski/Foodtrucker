using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Persistency.Entities
{
    public class FoodtruckerRole : IdentityRole<Guid>
    {
        public static readonly string Customer = "CUSTOMER";
        public static readonly string FoodtruckStaff = "FOODTRUCK_STAFF";
        public static readonly string ServiceStaff = "SERVICE_STAFF";
        public static readonly ISet<string> Roles = new HashSet<string> {Customer, FoodtruckStaff, ServiceStaff};

        internal FoodtruckerRole()
        {
        }
    }
}