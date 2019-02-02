const foodtrucksChanged = 'staff/home/FOODTRUCKS_CHANGED';

const initialState = {
    foodtruck: null,
    presences: []
};

export const actionCreators = {
    updateFoodtrucks: () => async (dispatch, getState) => {
        const response = await fetch(`api/staff/foodtruck`);
        const result = await response.json();
        dispatch({type: foodtrucksChanged, foodtrucks: result.result});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtrucksChanged:
            return {...state, foodtrucks: action.foodtrucks};
        default:
            return state;
    }
};