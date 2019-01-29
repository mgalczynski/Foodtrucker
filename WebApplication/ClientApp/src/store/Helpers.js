import moment from 'moment';

export const mapPresence = (presence) => ({
    ...presence,
    startTime: moment(presence.startTime),
    endTime: (presence.endTime == null) ? null : moment(presence.endTime)
});