﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistency.Dtos;

namespace Persistency.Services
{
    public interface IPresenceOrUnavailabilityService : IBaseService<PresenceOrUnavailability>
    {
        Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilitiesWithin(Coordinate coordinate, double distance, DateTime startEndTime);
        Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilities(string slug);
        Task<IDictionary<string, IList<PresenceOrUnavailability>>> FindPresencesOrUnavailabilities(ICollection<string> foodtruckSlug);
        Task<IList<PresenceOrUnavailability>> FindPresencesOrUnavailabilitiesWithin(Coordinate topLeft, Coordinate bottomRight, DateTime startEndTime);
        Task<PresenceOrUnavailability> CreatePresenceOrUnavailability(string foodtruckSlug, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability);
        Task<PresenceOrUnavailability> ModifyPresenceOrUnavailability(Guid presenceId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability);
        Task<ResponseWithStatus<PresenceOrUnavailability>> ValidatePresenceOrUnavailability(string foodtruckSlug, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability);
        Task<ResponseWithStatus<PresenceOrUnavailability>> ValidatePresenceOrUnavailability(Guid presenceId, CreateModifyPresenceOrUnavailability createModifyPresenceOrUnavailability);
    }
}