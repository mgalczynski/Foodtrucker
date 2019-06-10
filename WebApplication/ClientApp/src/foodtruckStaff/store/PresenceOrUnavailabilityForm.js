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
const canBeSendChanged = 'staff/presenceOrUnavailabilityForm/CAN_BE_SEND_CHANGED';
const validationChanged = 'staff/presenceOrUnavailabilityForm/VALIDATION_CHANGED';

const initialState = {
    foodtruckSlug: null,
    id: null,
    requestSent: false,
    isOpen: false,
    shouldHasEndTime: false,
    isUnavailability: false,
    canBeSend: false,
    reasonWhyNotValid: null,
    dtoWhyNotValid: null,
    presenceOrUnavailability: {
        title: '',
        description: '',
        startTime: null,
        endTime: null,
        location: null,
    },
};

const serializePresence = state =>
    JSON.stringify({
        ...state.presenceOrUnavailability,
        endTime: state.shouldHasEndTime ? state.presenceOrUnavailability.endTime : null
    });

const preCheck = state =>
    state.presenceOrUnavailability.title.length === 0 ||
    state.presenceOrUnavailability.description.length === 0 ||
    state.presenceOrUnavailability.startTime === null ||
    state.presenceOrUnavailability.location === null ||
    (state.shouldHasEndTime && state.presenceOrUnavailability.endTime == null);

export const actionCreators = {
    validate: async (dispatch, getState) => {
        const state = getState().presenceOrUnavailabilityForm;
        const shouldUpdate = state.id !== null;
        console.log(state);
        if (preCheck(state)) {
            dispatch({type: canBeSendChanged, value: false});
            return;
        }
        const result = await (await fetch(shouldUpdate ? `api${staffPrefix}/presenceOrUnavailability/validate/${state.id}` : `api${staffPrefix}/presenceOrUnavailability/${state.foodtruckSlug}/validate`,
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: serializePresence(state),
            })).json();
        dispatch({type: validationChanged, result});
    },
    isUnavailabilityChanged: (value) => async (dispatch, getState) => {
        dispatch({type: isUnavailabilityChanged, value});
        await actionCreators.validate(dispatch, getState);
    },
    changeTitle: (title) => async (dispatch, getState) => {
        dispatch({type: titleChanged, title});
        await actionCreators.validate(dispatch, getState);
    },
    changeDescription: (description) => async (dispatch, getState) => {
        dispatch({type: descriptionChanged, description});
        await actionCreators.validate(dispatch, getState);
    },
    changeStartTime: (startTime) => async (dispatch, getState) => {
        dispatch({type: startTimeChanged, startTime});
        await actionCreators.validate(dispatch, getState);
    },
    changeEndTime: (endTime) => async (dispatch, getState) => {
        dispatch({type: endTimeChanged, endTime});
        await actionCreators.validate(dispatch, getState);
    },
    changeLocation: (location) => async (dispatch, getState) => {
        dispatch({...location, type: locationChanged});
        await actionCreators.validate(dispatch, getState);
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
    changeShouldHasEndTime: (value) => async (dispatch, getState) => {
        dispatch({type: shouldHasEndTimeChanged, value});
        await actionCreators.validate(dispatch, getState);
    },
    save: () => async (dispatch, getState) => {
        const state = getState().presenceOrUnavailabilityForm;
        const shouldUpdate = state.id !== null;
        if (preCheck(state))
            return;
        dispatch({type: requestSent});
        const response = await fetch(shouldUpdate ? `api${staffPrefix}/presenceOrUnavailability/${state.id}` : `api${staffPrefix}/presenceOrUnavailability/${state.foodtruckSlug}`,
            {
                credentials: 'same-origin',
                method: shouldUpdate ? 'PUT' : 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: serializePresence(state),
            });
        await Foodtruck.loadFoodtruck(state.foodtruckSlug, true)(dispatch, getState);
        dispatch({type: close});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case canBeSendChanged:
            return {...state, canBeSend: action.value};
        case validationChanged:
            return {...state, dtoWhyNotValid: action.result.dto, reasonWhyNotValid: action.result.description};
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