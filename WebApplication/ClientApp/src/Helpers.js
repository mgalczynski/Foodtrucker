export const staffPrefix = '/foodtruckStaff';

export const positionWatch = (positionCallback, errorCallback, watchIdCallback) => {
    if ('geolocation' in navigator) {
        const watchId = navigator.geolocation.watchPosition(
            positionCallback,
            errorCallback,
            {enableHighAccurency: true}
        );
        watchIdCallback(watchId);
    }
};