﻿import {push} from 'react-router-redux';
import {actionCreators as App, userChanged} from './App';
import {staffPrefix} from '../Helpers';

const emailChanged = 'register/EMAIL_CHANGED';
const firstNameChanged = 'register/FIRSTNAME_CHANGED';
const lastNameChanged = 'register/LASTNAME_CHANGED';
const passwordChanged = 'register/PASSWORD_CHANGED';
const failedAttempt = 'register/FAILED_ATTEMPT';
const requestStarted = 'register/REQUEST_STARTED';
const initialState = {cause: null, email: '', firstName: '', lastName: '', password: '', ongoingRequest: false};

export const actionCreators = {
    emailChanged: value => async (dispatch) => {
        dispatch({type: emailChanged, value});
    },
    lastNameChanged: value => async (dispatch) => {
        dispatch({type: lastNameChanged, value});
    },
    firstNameChanged: value => async (dispatch) => {
        dispatch({type: firstNameChanged, value});
    },
    passwordChanged: value => async (dispatch) => {
        dispatch({type: passwordChanged, value});
    },
    submit: (staff) => async (dispatch, getState) => {
        const state = getState().register;
        if (!state.email.trim() || !state.firstName.trim() || !state.lastName.trim() || !state.password.trim())
            return;
        dispatch({type: requestStarted});
        const response = await fetch(staff ? 'api/auth/registerStaff' : 'api/auth/register',
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: state.email,
                    firstName: state.firstName,
                    lastName: state.lastName,
                    password: state.password
                })
            });
        const result = await response.json();
        if (result.successful) {
            await App.changeUser(result.user, result.roles)(dispatch, getState);
            dispatch(push(staff ? staffPrefix : '/'));
        } else
            dispatch({type: failedAttempt, cause: result.errors});
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case emailChanged:
            return {...state, email: action.value};
        case firstNameChanged:
            return {...state, firstName: action.value};
        case lastNameChanged:
            return {...state, lastName: action.value};
        case passwordChanged:
            return {...state, password: action.value};
        case requestStarted:
            return {...state, ongoingRequest: true, password: ''};
        case failedAttempt:
            return {...state, ongoingRequest: false, cause: action.cause};
        case userChanged:
            return initialState;
        default:
            return state;
    }
};