﻿const foodtruckChanged = 'foodtruck/FOODTRUCK_CHANGED';
const resetState = 'foodtruck/RESET_STATE';

const initialState = {
    foodtruck: null,
    presences: []
};

export const actionCreators = {
    loadFoodtruck: (foodtruckSlug) => async (dispatch, getState) => {
        if (getState().foodtruckModal.foodtruck !== null &&
            getState().foodtruckModal.foodtruck.slug === foodtruckSlug)
            return;
        var response = await fetch(`api/foodtruck/${foodtruckSlug}`);
        var result = await response.json();
        dispatch({ type: foodtruckChanged, foodtruck: result.foodtruck, presences: result.presences });
    },
    clear: () => async (dispatch) => {
        dispatch({ type: resetState })
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtruckChanged:
            return { ...state, foodtruck: action.foodtruck, presences: action.presences };
        case resetState:
            return initialState;
        default:
            return state;
    }
};