import {push} from 'react-router-redux';
import {history} from '..';
import {staffPrefix} from '../Helpers';

export const userChanged = 'app/USER_CHANGED';

const initialState = {user: null};

export const actionCreators = {
    checkUser: () => async (dispatch, getState) => {
        const response = await fetch('api/auth/check',
            {
                credentials: 'same-origin',
                method: 'GET'
            });
        const result = await response.json();
        if (result.isSignedIn)
            await actionCreators.changeUser(result.user, result.roles)(dispatch, getState);
    },
    changeUser: (user, roles) => async (dispatch, getState) => {
        dispatch({type: userChanged, user: user, roles: roles});
        if (history.location.pathname.startsWith(staffPrefix) && roles.every(r => r !== 'FOODTRUCK_STAFF'))
            dispatch(push('/'));
        else if (roles.every(r => r !== 'CUSTOMER'))
            dispatch(push(staffPrefix));
    },
    logOut: () => async (dispatch) => {
        await fetch('api/auth/logout',
            {
                credentials: 'same-origin',
                method: 'GET'
            });
        dispatch({type: userChanged, user: null, roles: null});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case userChanged:
            return {...state, user: action.user, roles: action.roles};
        default:
            return state;
    }
};