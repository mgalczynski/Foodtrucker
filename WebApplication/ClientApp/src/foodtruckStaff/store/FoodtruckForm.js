import {push} from 'react-router-redux';
import {staffPrefix} from '../../Helpers';

const open = 'staff/foodtruckForm/OPEN';
const close = 'staff/foodtruckForm/CLOSE';
const nameChanged = 'staff/foodtruckForm/NAME_CHANGED';
const displayNameChanged = 'staff/foodtruckForm/DISPLAY_NAME_CHANGED';
const locationChanged = 'staff/foodtruckForm/LOCATION_CHANGED';
const shouldHasDefaultLocationChanged = 'staff/foodtruckForm/SHOULD_HAVE_DEFAULT_LOCATION_CHANGED';
const requestSent = 'staff/foodtruckForm/REQUEST_SENT';

const initialState = {
    requestSent: false,
    isOpen: false,
    shouldHasDefaultLocation: false,
    foodtruck: {
        name: '',
        displayName: '',
        defaultLocation: null,
    }
};

export const actionCreators = {
    changeName: (name) => async (dispatch) => {
        dispatch({type: nameChanged, name});
    },
    changeDisplayName: (displayName) => async (dispatch) => {
        dispatch({type: displayNameChanged, displayName});
    },
    changeLocation: (location) => async (dispatch) => {
        dispatch({...location, type: locationChanged});
    },
    open: () => async (dispatch) => {
        dispatch({type: open});
    },
    close: () => async (dispatch) => {
        dispatch({type: close});
    },
    changeShouldHasDefaultLocation: (value) => async (dispatch) => {
        dispatch({type: shouldHasDefaultLocationChanged, value});
    },
    save: (foodtruckSlug) => async (dispatch, getState) => {
        const state = getState().foodtruckForm;
        console.log(state);
        if (state.foodtruck.name.length === 0 || state.foodtruck.displayName.length === 0)
            return;
        dispatch({type: requestSent});
        const response = await fetch(`api${staffPrefix}/foodtruck/`,
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(state.foodtruck)
            });
        const result = await response.json();
        dispatch({type: close});
        dispatch(push(`${staffPrefix}/foodtruck/${result.slug}`));
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case open:
            return {...state, isOpen: true};
        case close:
            return initialState;
        case nameChanged:
            return {...state, foodtruck: {...state.foodtruck, name: action.name}};
        case displayNameChanged:
            return {...state, foodtruck: {...state.foodtruck, displayName: action.displayName}};
        case shouldHasDefaultLocationChanged:
            return {...state, shouldHasDefaultLocation: action.value};
        case requestSent:
            return {...state, requestSent: true};
        case locationChanged:
            return {
                ...state,
                foodtruck: {
                    ...state.foodtruck,
                    defaultLocation: {latitude: action.latitude, longitude: action.longitude}
                }
            };
        default:
            return state;
    }
};