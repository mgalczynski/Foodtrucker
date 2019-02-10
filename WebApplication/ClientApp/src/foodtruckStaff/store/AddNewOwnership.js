import { REPORTER } from '../Permisions';
import { staffPrefix } from '../../Helpers';
import { actionCreators as Foodtruck } from './Foodtruck';

export const open = 'staff/addNewOwnership/OPEN';
const close = 'staff/addNewOwnership/CLOSE';
const changeType = 'staff/addNewOwnership/CHANGE_TYPE';
const previewOfUsersChanged = 'staff/addNewOwnership/PREVIEW_OF_USERS_CHANGED';
const createRequestSent = 'staff/addNewOwnership/CREATE_REQUEST_SENT';
const userChanged = 'staff/addNewOwnership/USER_CHANGED';
const loadingStarted = 'staff/addNewOwnership/LOADING_STARTED';
const queryChanged = 'staff/addNewOwnership/QUERY_CHANGED';

const initialState = {
    isOngoingCreateRequest: false,
    loading: false,
    open: false,
    type: REPORTER,
    user: null,
    query: '',
    exceptUsers: [],
    foundUsers: []
};

export const actionCreators = {
    changeAddNewOwnerType: (type) => async (dispatch) => {
        dispatch({ type: changeType, newType: type });
    },
    close: () => async (dispatch) => {
        dispatch({ type: close });
    },
    userChanged: (user) => async (dispatch) => {
        dispatch({ type: userChanged, user });
    },
    onQueryChanged: (query) => async (dispatch, getState) => {
        dispatch({ type: queryChanged, query });
        if (!query.split(' ').some(q => q.length >= 3)) {
            dispatch({ type: previewOfUsersChanged, users: [] });
            return;
        }
        const state = getState();
        dispatch({ type: loadingStarted });
        const response = await fetch(`api${staffPrefix}/user/find`,
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    query,
                    except: state.addNewOwnership.exceptUsers
                })
            });
        const users = (await response.json()).result;
        if (query === getState().addNewOwnership.query)
            dispatch({ type: previewOfUsersChanged, users });
    },
    createNewOwnership: (foodtruckSlug) => async (dispatch, getState) => {
        const state = getState().addNewOwnership;
        if (state.user === null)
            return;
        dispatch({ type: createRequestSent });
        await fetch(`api${staffPrefix}/foodtruck/${foodtruckSlug}/createNewOwnership`,
            {
                credentials: 'same-origin',
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: state.user.email,
                    type: state.type
                })
            });
        await Foodtruck.loadFoodtruck(foodtruckSlug, true)(dispatch, getState);
        dispatch({ type: close });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case open:
            return { ...state, open: true, exceptUsers: action.users };
        case changeType:
            return { ...state, type: action.newType };
        case userChanged:
            return { ...state, user: action.user };
        case previewOfUsersChanged:
            return { ...state, foundUsers: action.users, loading: false };
        case queryChanged:
            return { ...state, query: action.query };
        case loadingStarted:
            return { ...state, loading: true };
        case createRequestSent:
            return { ...state, isOngoingCreateRequest: true };
        case close:
            return initialState;
        default:
            return state;
    }
};