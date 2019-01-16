export const userChanged = 'app/USER_CHANGED';

const initialState = {user: null};

export const actionCreators = {
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