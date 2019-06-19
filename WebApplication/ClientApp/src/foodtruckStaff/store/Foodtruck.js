import {positionWatch, staffPrefix} from '../../Helpers';
import {actionCreators as StaffHome} from './StaffHome';
import {actionCreators as PresenceOrUnavailabilityForm} from './PresenceOrUnavailabilityForm';
import {open} from './AddNewOwnership';
import {actionCreators as FoodtruckForm} from "./FoodtruckForm";

const foodtruckChanged = 'staff/foodtruck/FOODTRUCK_CHANGED';
const notFound = 'staff/foodtruck/NOT_FOUND';
const foodtruckWithOwnershipsChanged = 'staff/foodtruck/FOODTRUCK_WITH_OWNERSHIPS_CHANGED';
const positionChanged = 'staff/foodtruck/POSITION_CHANGED';
const resetState = 'staff/foodtruck/RESET_STATE';
const locationWatchCreated = 'staff/foodtruck/LOCATION_WATCH_CREATED';
const updateSlug = 'staff/foodtruck/UPDATE_SLUG';
const locationWatchDeleted = 'staff/foodtruck/LOCATION_WATCH_DELETED';

const initialState = {
    loadedSlug: null,
    foodtruck: null,
    watchId: null,
    position: null,
    notFound: false,
};

export const actionCreators = {
    loadFoodtruck: (foodtruckSlug, force = false) => async (dispatch, getState) => {
        if (!force && getState().foodtruckForStaff.loadedSlug === foodtruckSlug)
            return;
        dispatch({type: updateSlug, slug: foodtruckSlug});
        const response = await fetch(`api${staffPrefix}/foodtruck/${foodtruckSlug}`);
        if (response.status === 404) {
            dispatch({type: notFound});
            return;
        }
        const foodtruck = await response.json();
        dispatch({type: foodtruckWithOwnershipsChanged, foodtruck});
    },
    openNewOwnershipModal: () => async (dispatch, getState) => {
        const state = getState();
        dispatch({type: open, users: state.foodtruckForStaff.foodtruck.ownerships.map(o => o.user.email)});
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
    ),
    updateFoodtruck: (foodtruck) => async (dispatch) => {
        dispatch({type: foodtruckChanged, foodtruck});
    },
    removeOwnership: (email) => async (dispatch, getState) => {
        const foodtruckSlug = getState().foodtruckForStaff.loadedSlug;
        await fetch(`api${staffPrefix}/foodtruck/${foodtruckSlug}/deleteOwnership`,
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email
                })
            });
        actionCreators.loadFoodtruck(foodtruckSlug, true)(dispatch, getState);
        await StaffHome.updateFoodtrucks()(dispatch, getState);
    },
    removePresenceOrUnavailability: (presenceOrUnavailabilityId) => async (dispatch, getState) => {
        const foodtruckSlug = getState().foodtruckForStaff.loadedSlug;
        await fetch(`api${staffPrefix}/presenceOrUnavailability/${presenceOrUnavailabilityId}`,
            {
                credentials: 'same-origin',
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
        actionCreators.loadFoodtruck(foodtruckSlug, true)(dispatch, getState);
        await StaffHome.updateFoodtrucks()(dispatch, getState);
    },
    changeOwnership: (email, type) => async (dispatch, getState) => {
        const foodtruckSlug = getState().foodtruckForStaff.loadedSlug;
        await fetch(`api${staffPrefix}/foodtruck/${foodtruckSlug}/changeOwnership`,
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email,
                    type
                })
            });
        actionCreators.loadFoodtruck(foodtruckSlug, true)(dispatch, getState);
        StaffHome.updateFoodtrucks()(dispatch, getState);
    },
    openNewPresenceOrUnavailabilityModal: () => async (dispatch, getState) => {
        await PresenceOrUnavailabilityForm.open(getState().foodtruckForStaff.loadedSlug)(dispatch, getState);
    },
    openModifyPresenceOrUnavailabilityModal: (presenceOrUnavailability) => async (dispatch, getState) => {
        await PresenceOrUnavailabilityForm.openWithPresenceOrUnavailability(presenceOrUnavailability)(dispatch, getState);
    },
    modifyFoodtruck: (foodtruck) => async (dispatch) => {
        await FoodtruckForm.openWithFoodtruck(foodtruck)(dispatch);
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtruckWithOwnershipsChanged:
            return {
                ...state,
                notFound: false,
                foodtruck: {
                    ...action.foodtruck,
                },
            };
        case notFound:
            return {
                ...state,
                notFound: true,
            };
        case foodtruckChanged:
            return {
                ...state,
                notFound: false,
                foodtruck: {
                    ...action.foodtruck,
                    ownerships: state.foodtruck.ownerships
                },
            };
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