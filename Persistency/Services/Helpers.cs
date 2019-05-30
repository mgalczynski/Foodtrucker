using Persistency.Dtos;

namespace Persistency.Services
{
    public static class Helpers
    {
        public static ResponseWithStatus<PresenceOrUnavailability> MapToResponse(this ValidationException<PresenceOrUnavailability> ex) =>
            new ResponseWithStatus<PresenceOrUnavailability>
            {
                Dto = ex.Dto,
                Successful = false,
                Description = "Conflict with"
            };

        public static ResponseWithStatus<PresenceOrUnavailability> MapToResponse(this PresenceOrUnavailability result) =>
            new ResponseWithStatus<PresenceOrUnavailability>
            {
                Dto = result,
                Successful = true
            };
    }
}