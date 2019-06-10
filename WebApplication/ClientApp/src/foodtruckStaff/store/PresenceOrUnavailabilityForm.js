import {staffPrefix} from '../../Helpers';
import {actionCreators as Foodtruck} from './Foodtruck';

const open = 'staff/presenceOrUnavailabilityForm/OPEN';
const openWithPresenceOrUnavailability = 'staff/presenceOrUnavailabilityForm/OPEN_WITH_PRESENCE_OR_UNAVAILABILITIES';
const close = 'staff/presenceOrUnavailabilityForm/CLOSE';
const titleChanged = 'staff/presenceOrUnavailabilityForm/TITLE_CHANGED';
const descriptionChanged = 'staff/presenceOrUnavailabilityForm/DESCRIPTION_CHANGED';
const locationChanged = 'staff/presenceOrUnavailabilityForm/LOCATION_CHANGED';
const startTimeChanged = 'staff/presenceOrUnavailabilityForm/START_TIME_CHANGED';
const endTimeChanged = 'staff/presenceOrUnavailabilityForm/END_TIME_CHANGED';
const shouldHasEndTimeChanged = 'staff/presenceOrUnavailabilityForm/SHOULD_HAS_END_TIME_CHANGED';
const requestSent = 'staff/presenceOrUnavailabilityForm/REQUEST_SENT';
const isUnavailabilityChanged = 'staff/presenceOrUnavailabilityForm/IS_UNAVAILABILITY_CHANGED';

const initialState = {
    foodtruckSlug: null,
    id: null,
    requestSent: false,
    isOpen: false,
    shouldHasEndTime: false,
    isUnavailability: false,
    presenceOrUnavailability: {
        title: '',
        description: '',
        startTime: null,
        endTime: null,
        location: null,
    }
};

export const actionCreators = {
    isUnavailabilityChanged: (value) => async (dispatch) => {
        dispatch({type: isUnavailabilityChanged, value});
    },
    changeTitle: (title) => async (dispatch) => {
        dispatch({type: titleChanged, title});
    },
    changeDescription: (description) => async (dispatch) => {
        dispatch({type: descriptionChanged, description});
    },
    changeStartTime: (startTime) => async (dispatch) => {
        dispatch({type: startTimeChanged, startTime});
    },
    changeEndTime: (endTime) => async (dispatch) => {
        dispatch({type: endTimeChanged, endTime});
    },
    changeLocation: (location) => async (dispatch) => {
        dispatch({...location, type: locationChanged});
    },
    open: (foodtruckSlug) => async (dispatch) => {
        dispatch({type: open, foodtruckSlug});
    },
    openWithPresenceOrUnavailability: (presenceOrUnavailability) => async (dispatch) => {
        dispatch({
            type: openWithPresenceOrUnavailability,
            presenceOrUnavailability,
            id: presenceOrUnavailability.id,
            foodtruckSlug: presenceOrUnavailability.foodtruckSlug
        });
    },
    close: () => async (dispatch) => {
        dispatch({type: close});
    },
    changeShouldHasEndTime: (value) => async (dispatch) => {
        dispatch({type: shouldHasEndTimeChanged, value});
    },
    save: () => async (dispatch, getState) => {
        const state = getState().presenceOrUnavailabilityForm;
        const shouldUpdate = state.id !== null;
        if (state.presence.title.length === 0 ||
            state.presence.description.length === 0 ||
            state.presence.startTime === null ||
            state.presence.location === null ||
            (state.shouldHasEndTime && state.presence.endTime == null))
            return;
        dispatch({type: requestSent});
        const response = await fetch(shouldUpdate ? `api${staffPrefix}/presence/${state.id}` : `api${staffPrefix}/presence/${state.foodtruckSlug}`,
            {
                credentials: 'same-origin',
                method: shouldUpdate ? 'PUT' : 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    ...state.presence,
                    endTime: state.shouldHasEndTime ? state.presence.endTime : null
                })
            });
        await Foodtruck.loadFoodtruck(state.foodtruckSlug, true)(dispatch, getState);
        dispatch({type: close});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case open:
            return {...state, isOpen: true, foodtruckSlug: action.foodtruckSlug};
        case openWithPresenceOrUnavailability:
            return {
                ...state,
                isOpen: true,
                foodtruckSlug: action.foodtruckSlug,
                id: action.id,
                shouldHasEndTime: action.presenceOrUnavailability.endTime !== null,
                isUnavailability: action.presenceOrUnavailability.location === null,
                presenceOrUnavailability: {
                    title: action.presenceOrUnavailability.title,
                    description: action.presenceOrUnavailability.description,
                    startTime: action.presenceOrUnavailability.startTime,
                    endTime: action.presenceOrUnavailability.endTime,
                    location: action.presenceOrUnavailability.location === null ? null : {
                        latitude: action.presenceOrUnavailability.location.latitude,
                        longitude: action.presenceOrUnavailability.location.longitude
                    }
                }
            };
        case close:
            return initialState;
        case titleChanged:
            return {...state, presenceOrUnavailability: {...state.presenceOrUnavailability, title: action.title}};
        case descriptionChanged:
            return {
                ...state,
                presenceOrUnavailability: {...state.presenceOrUnavailability, description: action.description}
            };
        case startTimeChanged:
            return {
                ...state,
                presenceOrUnavailability: {...state.presenceOrUnavailability, startTime: action.startTime}
            };
        case endTimeChanged:
            return {...state, presenceOrUnavailability: {...state.presenceOrUnavailability, endTime: action.endTime}};
        case shouldHasEndTimeChanged:
            return {...state, shouldHasEndTime: action.value};
        case locationChanged:
            return {
                ...state,
                presenceOrUnavailability: {
                    ...state.presenceOrUnavailability,
                    location: {latitude: action.latitude, longitude: action.longitude}
                }
            };
        case isUnavailabilityChanged:
            return {
                ...state,
                isUnavailability: action.value,
            };
        case requestSent:
            return {...state, requestSent: true};
        default:
            return state;
    }
};