export const userChanged = 'app/USER_CHANGED';

const initialState = {user: null};

export const actionCreators = {
    checkUser: () => async (dispatch) => {
        const response = await fetch('api/auth/check',
            {
                credentials: 'same-origin',
                method: 'GET'
            });
        const result = await response.json();
        if (result.isSignedIn)
            dispatch({type: userChanged, user: result.user});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case userChanged:
            return {...state, user: action.user};
        default:
            return state;
    }
};