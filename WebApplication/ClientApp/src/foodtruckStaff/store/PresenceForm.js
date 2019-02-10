import {staffPrefix} from '../../Helpers';
import {actionCreators as Foodtruck} from './Foodtruck';

const open = 'staff/presenceForm/OPEN';
const openWithPresence = 'staff/presenceForm/OPEN_WITH_PRESENCE';
const close = 'staff/presenceForm/CLOSE';
const titleChanged = 'staff/presenceForm/TITLE_CHANGED';
const descriptionChanged = 'staff/presenceForm/DESCRIPTION_CHANGED';
const locationChanged = 'staff/presenceForm/LOCATION_CHANGED';
const startTimeChanged = 'staff/presenceForm/START_TIME_CHANGED';
const endTimeChanged = 'staff/presenceForm/END_TIME_CHANGED';
const shouldHasEndTimeChanged = 'staff/presenceForm/SHOULD_HAS_END_TIME_CHANGED';
const requestSent = 'staff/presenceForm/REQUEST_SENT';

const initialState = {
    foodtruckSlug: null,
    id: null,
    requestSent: false,
    isOpen: false,
    shouldHasEndTime: false,
    presence: {
        title: '',
        description: '',
        startTime: null,
        endTime: null,
        location: null,
    }
};

export const actionCreators = {
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
    openWithPresence: (presence) => async (dispatch) => {
        dispatch({type: openWithPresence, presence, id: presence.id, foodtruckSlug: presence.foodtruckSlug});
    },
    close: () => async (dispatch) => {
        dispatch({type: close});
    },
    changeShouldHasEndTime: (value) => async (dispatch) => {
        dispatch({type: shouldHasEndTimeChanged, value});
    },
    save: () => async (dispatch, getState) => {
        const state = getState().presenceForm;
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
        case openWithPresence:
            return {
                ...state,
                isOpen: true,
                foodtruckSlug: action.foodtruckSlug,
                id: action.id,
                shouldHasEndTime: action.presence.endTime !== null,
                presence: {
                    title: action.presence.title,
                    description: action.presence.description,
                    startTime: action.presence.startTime,
                    endTime: action.presence.endTime,
                    location: {
                        latitude: action.presence.location.latitude,
                        longitude: action.presence.location.longitude
                    }
                }
            };
        case close:
            return initialState;
        case titleChanged:
            return {...state, presence: {...state.presence, title: action.title}};
        case descriptionChanged:
            return {...state, presence: {...state.presence, description: action.description}};
        case startTimeChanged:
            return {...state, presence: {...state.presence, startTime: action.startTime}};
        case endTimeChanged:
            return {...state, presence: {...state.presence, endTime: action.endTime}};
        case shouldHasEndTimeChanged:
            return {...state, shouldHasEndTime: action.value};
        case locationChanged:
            return {
                ...state,
                presence: {
                    ...state.presence,
                    location: {latitude: action.latitude, longitude: action.longitude}
                }
            };
        case requestSent:
            return {...state, requestSent: true};
        default:
            return state;
    }
};