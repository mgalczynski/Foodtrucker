import {mapPresence} from '../../store/Helpers';
import {positionWatch, staffPrefix} from '../../Helpers';

const foodtruckChanged = 'staff/foodtruck/FOODTRUCK_CHANGED';
const positionChanged = 'staff/foodtruck/POSITION_CHANGED';
const resetState = 'staff/foodtruck/RESET_STATE';
const locationWatchCreated = 'staff/foodtruck/LOCATION_WATCH_CREATED';
const updateSlug = 'staff/foodtruck/UPDATE_SLUG';
const locationWatchDeleted = 'staff/foodtruck/LOCATION_WATCH_DELETED';

const initialState = {
    loadedSlug: null,
    foodtruck: null,
    watchId: null,
    position: null
};

export const actionCreators = {
    loadFoodtruck: (foodtruckSlug) => async (dispatch, getState) => {
        if (getState().foodtruckForStaff.loadedSlug === foodtruckSlug)
            return;
        dispatch({type: updateSlug, slug: foodtruckSlug});
        const response = await fetch(`api${staffPrefix}/foodtruck/${foodtruckSlug}`);
        const foodtruck = await response.json();
        dispatch({type: foodtruckChanged, foodtruck});
    },
    clear: () => async (dispatch, getState) => {
        const watchId = getState().map.watchId;
        if (watchId !== null)
            navigator.geolocation.clearWatch(watchId);
        dispatch({type: resetState});
    },
    watchPosition: () => async (dispatch) => positionWatch(
        (e) => dispatch({type: positionChanged, longitude: e.coords.longitude, latitude: e.coords.latitude}),
        () => dispatch({type: locationWatchDeleted}),
        (watchId) => dispatch({type: locationWatchCreated, watchId})
    )
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtruckChanged:
            return {...state, foodtruck: {...action.foodtruck, presences: action.foodtruck.presences.map(mapPresence)}};
        case resetState:
            return initialState;
        case positionChanged:
            return {...state, position: {longitude: action.longitude, latitude: action.latitude}};
        case locationWatchCreated:
            return {...state, watchId: action.watchId};
        case locationWatchDeleted:
            return {...state, watchId: null};
        case updateSlug:
            return {...state, loadedSlug: action.slug};
        default:
            return state;
    }
};