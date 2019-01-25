﻿import { push } from 'react-router-redux';

const locationChanged = 'map/LOCATION_CHANGED';
const zoomChanged = 'map/ZOOM_CHANGED';
const boundsChanged = 'map/BOUNDS_CHANGED';
const positionChanged = 'map/POSITION_CHANGED';
const locationZoomChanged = 'map/LOCATION_ZOOM_CHANGED';
const positionRemoved = 'map/POSITION_REMOVED';
const foodtrucksPresencesUpdate = 'map/FOODTRUCKS_AND PRESENCES_UPDATE';
const infoFoodtrucksUpdate = 'map/INFO_FOODTRUCKS_UPDATE';
const locationWatchCreated = 'map/LOCATION_WATCH_CREATED';
const locationWatchDeleted = 'map/LOCATION_WATCH_DELETED';
const initialState = {
    latitude: 51.110254,
    longitude: 17.031626,
    zoom: 13,
    bounds: null,
    watchId: null,
    position: null,
    foodtrucks: [],
    presences: [],
    infoFoodtrucks: [],
    allFoodtrucks: new Map()
};

export const actionCreators = {
    locationChanged: (longitude, latitude, bounds) => async (dispatch, getState) => {
        dispatch({ type: locationChanged, longitude, latitude, bounds });
        actionCreators.loadPoi(dispatch, getState);
    },
    boundsChanged: (bounds) => async (dispatch, getState) => {
        dispatch({ type: boundsChanged, bounds });
        actionCreators.loadPoi(dispatch, getState);
    },
    zoomChanged: (zoom, bounds) => async (dispatch, getState) => {
        dispatch({ type: zoomChanged, zoom, bounds });
        actionCreators.loadPoi(dispatch, getState);
    },
    goToLocation: async (dispatch, getState) => {
        const state = getState();
        if (state.map.position !== null)
            dispatch({ type: locationZoomChanged, ...state.map.position, zoom: 16 });
    },
    goToLocationExtended: () => async (dispatch, getState) => {
        if (getState().map.watchId === null)
            actionCreators.watchPosition()(dispatch, getState);
        else
            actionCreators.goToLocation(dispatch, getState);
    },
    watchPosition: () => async (dispatch, getState) => {
        if ("geolocation" in navigator) {
            const watchId = navigator.geolocation.watchPosition((e) => {
                const goToLocationDecision = getState().map.position === null;
                dispatch({ type: positionChanged, longitude: e.coords.longitude, latitude: e.coords.latitude });
                if (goToLocationDecision) {
                    actionCreators.goToLocation(dispatch, getState);
                }
            },
                () => dispatch({ type: locationWatchDeleted }),
                { enableHighAccurency: true });
            dispatch({ type: locationWatchCreated, watchId });
        }
    },
    loadPoi: async (dispatch, getState) => {
        const state = getState().map;
        const foodtrucksResponse = await fetch('api/foodtruck/find',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    topLeft: {
                        longitude: state.bounds.getWest(),
                        latitude: state.bounds.getNorth()
                    },
                    bottomRight: {
                        longitude: state.bounds.getEast(),
                        latitude: state.bounds.getSouth()
                    }
                }),
            });
        const foodtruckContent = await foodtrucksResponse.json();
        dispatch({ type: foodtrucksPresencesUpdate, foodtrucks: foodtruckContent.foodtrucks, presences: foodtruckContent.presences });
        const infoResponse = await fetch('api/foodtruck/findByIds',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    ids: Array.from(new Set(foodtruckContent.presences.map(p => p.foodtruckId)))
                }),
            });
        const infoContent = await infoResponse.json();
        dispatch({ type: infoFoodtrucksUpdate, foodtrucks: infoContent })
    },
    clearWatch: () => async (dispatch, getState) => {
        const watchId = getState().map.watchId;
        if (watchId !== null) {
            navigator.geolocation.clearWatch(watchId);
            dispatch({ type: locationWatchDeleted });
            dispatch({ type: positionRemoved });
        }
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case boundsChanged:
            return { ...state, bounds: action.bounds };
        case locationZoomChanged:
            return { ...state, longitude: action.longitude, latitude: action.latitude, zoom: action.zoom };
        case locationChanged:
            return {
                ...state,
                longitude: action.longitude,
                latitude: action.latitude,
                bounds: action.bounds || state.bounds
            };
        case zoomChanged:
            return { ...state, zoom: action.zoom, bounds: action.bounds };
        case positionChanged:
            return { ...state, position: { longitude: action.longitude, latitude: action.latitude } };
        case positionRemoved:
            return { ...state, position: null };
        case locationWatchCreated:
            return { ...state, watchId: action.watchId };
        case locationWatchDeleted:
            return { ...state, watchId: null };
        case foodtrucksPresencesUpdate:
            return { ...state, foodtrucks: action.foodtrucks, presences: action.presences, allFoodtrucks: new Map(action.foodtrucks.concat(state.infoFoodtrucks).map(f => [f.id, f])) };
        case infoFoodtrucksUpdate:
            return { ...state, infoFoodtrucks: action.foodtrucks, allFoodtrucks: new Map(action.foodtrucks.concat(state.foodtrucks).map(f => [f.id, f])) };
        default:
            return state;
    }
};