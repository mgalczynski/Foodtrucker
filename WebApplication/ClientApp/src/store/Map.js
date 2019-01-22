import {push} from 'react-router-redux';

const locationChanged = 'map/LOCATION_CHANGED';
const zoomChanged = 'map/ZOOM_CHANGED';
const boundsChanged = 'map/BOUNDS_CHANGED';
const positionChanged = 'map/POSITION_CHANGED';
const positionRemoved = 'map/POSITION_REMOVED';
const locationWatchCreated = 'map/LOCATION_WATCH_CREATED';
const locationWatchDeleted = 'map/LOCATION_WATCH_DELETED';
const initialState = {latitude: 51.110254, longitude: 17.031626, zoom: 13, bounds: null, watchId: null, position: null};

export const actionCreators = {
    locationChanged: (longitude, latitude, bounds) => async (dispatch) => {
        dispatch({type: locationChanged, longitude, latitude, bounds});
    },
    boundsChanged: (bounds) => async (dispatch, getState) => {
        dispatch({type: boundsChanged, bounds});
    },
    zoomChanged: (zoom, bounds) => async (dispatch, getState) => {
        dispatch({type: zoomChanged, zoom, bounds});
    },
    watchPosition: () => async (dispatch) => {
        if ("geolocation" in navigator) {
            const watchId = navigator.geolocation.watchPosition((e) => {
                    dispatch({type: locationChanged, longitude: e.coords.longitude, latitude: e.coords.latitude});
                    dispatch({type: positionChanged, longitude: e.coords.longitude, latitude: e.coords.latitude});
                },
                null,
                {enableHighAccurency: true});
            dispatch({type: locationWatchCreated, watchId});
        }
    },
    clearWatch: () => async (dispatch, getState) => {
        const watchId = getState().map.watchId;
        if (watchId !== null) {
            navigator.geolocation.clearWatch(watchId);
            dispatch({type: locationWatchDeleted});
            dispatch({type: positionRemoved});
        }
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case locationChanged:
            return {...state, longitude: action.longitude, latitude: action.latitude, bounds: action.bounds};
        case zoomChanged:
            return {...state, zoom: action.zoom, bounds: action.bounds};
        case positionChanged:
            return {...state, position: {longitude: action.longitude, latitude: action.latitude}};
        case positionRemoved:
            return {...state, position: null};
        case locationWatchCreated:
            return {...state, watchId: action.watchId};
        case locationWatchDeleted:
            return {...state, watchId: null, position: null};
        default:
            return state;
    }
};