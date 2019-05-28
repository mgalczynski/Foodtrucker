﻿import { mapPresenceOrUnavailability } from './Helpers';
const foodtruckChanged = 'foodtruck/FOODTRUCK_CHANGED';
const resetState = 'foodtruck/RESET_STATE';

const initialState = {
    foodtruck: null,
    presencesOrUnavailabilities: []
};

export const actionCreators = {
    loadFoodtruck: (foodtruckSlug) => async (dispatch, getState) => {
        if (getState().foodtruckModal.foodtruck !== null &&
            getState().foodtruckModal.foodtruck.slug === foodtruckSlug)
            return;
        const response = await fetch(`api/foodtruck/${foodtruckSlug}`);
        const result = await response.json();
        dispatch({ type: foodtruckChanged, foodtruck: result.foodtruck, presencesOrUnavailabilities: result.presencesOrUnavailabilities });
    },
    clear: () => async (dispatch) => {
        dispatch({ type: resetState })
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtruckChanged:
            return { ...state, foodtruck: action.foodtruck, presencesOrUnavailabilities: action.presencesOrUnavailabilities.map(mapPresenceOrUnavailability) };
        case resetState:
            return initialState;
        default:
            return state;
    }
};