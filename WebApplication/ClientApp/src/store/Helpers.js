import moment from 'moment';

export const mapPresenceOrUnavailability = (presenceOrUnavailability) => ({
    ...presenceOrUnavailability,
    startTime: moment(presenceOrUnavailability.startTime),
    endTime: (presenceOrUnavailability.endTime == null) ? null : moment(presenceOrUnavailability.endTime)
});