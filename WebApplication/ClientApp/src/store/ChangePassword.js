import {push} from 'react-router-redux';
import {staffPrefix} from '../Helpers';

const currentPasswordChanged = 'changePassword/CURRENT_PASSWORD_CHANGED';
const newPasswordChanged = 'changePassword/NEW_PASSWORD_CHANGED';
const failedAttempt = 'changePassword/FAILED_ATTEMPT';
const requestStarted = 'changePassword/REQUEST_STARTED';
const reset = 'changePassword/RESET';
const initialState = {failed: false, currentPassword: '', newPassword: '', ongoingRequest: false, reason: null};

export const actionCreators = {
    currentPasswordChanged: value => async (dispatch) => {
        dispatch({type: currentPasswordChanged, value});
    },
    newPasswordChanged: value => async (dispatch) => {
        dispatch({type: newPasswordChanged, value});
    },
    submit: (staff) => async (dispatch, getState) => {
        const state = getState().changePassword;
        if (!state.currentPassword.trim() || !state.newPassword.trim() || state.ongoingRequest)
            return;
        dispatch({type: requestStarted});
        const response = await fetch('api/auth/changePassword',
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    currentPassword: state.currentPassword,
                    newPassword: state.newPassword,
                })
            });
        if (response.status === 200) {
            dispatch({type: reset});
            dispatch(push(staff ? staffPrefix : '/'));
        } else if (response.status === 400)
            dispatch({type: failedAttempt, reason: (await response.json()).reason});
        else
            dispatch({type: failedAttempt, reason: null});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case currentPasswordChanged:
            return {...state, currentPassword: action.value};
        case newPasswordChanged:
            return {...state, newPassword: action.value};
        case requestStarted:
            return {...state, ongoingRequest: true, currentPassword: '', newPassword: ''};
        case failedAttempt:
            return {...state, ongoingRequest: false, failed: true, reason: action.reason};
        case reset:
            return initialState;
        default:
            return state;
    }
};