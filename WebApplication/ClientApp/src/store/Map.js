import {push} from 'react-router-redux';

const locationChanged = 'map/LOCATION_CHANGED';
const initialState = {latitude: 51.110254, longitude: 17.031626, zoom: 13};

export const actionCreators = {
    locationChanged: (longitude, latitude) => async (dispatch) => {
        dispatch({type: locationChanged, longitude, latitude});
    },
    moveToCurrentPosition: () => async (dispatch) => {
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition((e) => {
                dispatch({type: locationChanged, longitude: e.coords.longitude, latitude: e.coords.latitude});
                console.log(e);
            }, null, {enableHighAccurency: true});
        }
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case locationChanged:
            return {...state, longitude: action.longitude, latitude: action.latitude};
        default:
            return state;
    }
};