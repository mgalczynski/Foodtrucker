import {push} from 'react-router-redux';
import {userChanged} from './App';
import {staffPrefix} from '../Helpers';

const emailChanged = 'login/EMAIL_CHANGED';
const passwordChanged = 'login/PASSWORD_CHANGED';
const rememberMeChanged = 'login/REMEMBER_ME_CHANGED';
const failedAttempt = 'login/FAILED_ATTEMPT';
const requestStarted = 'login/REQUEST_STARTED';
const initialState = {failed: false, email: '', password: '', rememberMe: false, ongoingRequest: false};

export const actionCreators = {
    emailChanged: value => async (dispatch) => {
        dispatch({type: emailChanged, value});
    },
    passwordChanged: value => async (dispatch) => {
        dispatch({type: passwordChanged, value});
    },
    rememberMeChanged: value => async (dispatch) => {
        dispatch({type: rememberMeChanged, value});
    },
    submit: (staff) => async (dispatch, getState) => {
        const state = getState().login;
        if (!state.email.trim() || !state.password.trim() || state.ongoingRequest)
            return;
        dispatch({type: requestStarted});
        const response = await fetch(staff ? 'api/auth/loginStaff' : 'api/auth/login',
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: state.email,
                    password: state.password,
                    rememberMe: state.rememberMe
                })
            });
        const result = await response.json();
        if (result.successful) {
            dispatch({type: userChanged, user: result.user});
            dispatch(push(staff ? staffPrefix : '/'));
        } else
            dispatch({type: failedAttempt});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case rememberMeChanged:
            return {...state, rememberMe: action.value};
        case emailChanged:
            return {...state, email: action.value};
        case passwordChanged:
            return {...state, password: action.value};
        case requestStarted:
            return {...state, ongoingRequest: true, password: ''};
        case failedAttempt:
            return {...state, ongoingRequest: false, failed: true};
        case userChanged:
            return initialState;
        default:
            return state;
    }
};