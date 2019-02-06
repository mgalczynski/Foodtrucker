const open = 'staff/foodtruckForm/OPEN';
const close = 'staff/foodtruckForm/CLOSE';
const nameChanged = 'staff/addNewOwnership/NAME_CHANGED';
const displayNameChanged = 'staff/addNewOwnership/DISPLAY_NAME_CHANGED';
const locationChanged = 'staff/addNewOwnership/LOCATION_CHANGED';

const initialState = {
    isOpen: false,
    foodtruck: {
        name: '',
        displayName: '',
        defaultLocation: null
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
    save: (query) => async (dispatch, getState) => {
        // dispatch({ type: queryChanged, query });
        // if (!query.split(' ').some(q => q.length >= 3)) {
        //     dispatch({ type: previewOfUsersChanged, users: [] });
        //     return;
        // }
        // const state = getState();
        // dispatch({ type: loadingStarted });
        // const response = await fetch(`api${staffPrefix}/user/find`,
        //     {
        //         credentials: 'same-origin',
        //         method: 'POST',
        //         headers: {
        //             'Content-Type': 'application/json'
        //         },
        //         body: JSON.stringify({
        //             query,
        //             except: state.addNewOwnership.exceptUsers
        //         }),
        //     });
        // const users = (await response.json()).result;
        // if (query === getState().addNewOwnership.query)
        //     dispatch({ type: previewOfUsersChanged, users });
    },
    createNewOwnership: (foodtruckSlug) => async (dispatch, getState) => {
        // const state = getState().addNewOwnership;
        // if (state.user === null)
        //     return;
        // dispatch({ type: createRequestSent });
        // await fetch(`api${staffPrefix}/foodtruck/${foodtruckSlug}/createNewOwnership`,
        //     {
        //         credentials: 'same-origin',
        //         method: 'POST',
        //         headers: {
        //             'Content-Type': 'application/json'
        //         },
        //         body: JSON.stringify({
        //             email: state.user.email,
        //             type: state.type
        //         }),
        //     });
        // await foodtruck.loadFoodtruck(foodtruckSlug, true)(dispatch, getState);
        // dispatch({ type: close });
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
        case locationChanged:
            return {
                ...state,
                foodtruck: {...state.foodtruck, defaultLocation: {latitude: action.latitude, longitude: action.longitude}}
            };
        default:
            return state;
    }
};