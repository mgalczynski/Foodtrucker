const userSignedIn = 'USER_SIGNED_IN';
const initialState = {user: null};

export const actionCreators = {
    increment: user => ({type: userSignedIn, user}),
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === userSignedIn) {
        return {...state, user: action.user};
    }

    return state;
};