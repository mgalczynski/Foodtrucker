import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { routerReducer, routerMiddleware } from 'react-router-redux';
import * as Counter from './Counter';
import * as WeatherForecasts from './WeatherForecasts';
import * as Login from './Login';
import * as MapStore from './Map';
import * as Register from './Register';
import * as Foodtruck from './Foodtruck';
import * as App from './App';
import foodtruckStaff from '../foodtruckStaff/store/configureStore';

export default function configureStore(history, initialState) {
    const reducers = {
        counter: Counter.reducer,
        weatherForecasts: WeatherForecasts.reducer,
        register: Register.reducer,
        login: Login.reducer,
        app: App.reducer,
        map: MapStore.reducer,
        foodtruckModal: Foodtruck.reducer,
        ...foodtruckStaff
    };

    const middleware = [
        thunk,
        routerMiddleware(history)
    ];

    // In development, use the browser's Redux dev tools extension if installed
    const enhancers = [];
    const isDevelopment = process.env.NODE_ENV === 'development';
    if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
        enhancers.push(window.devToolsExtension());
    }

    const rootReducer = combineReducers({
        ...reducers,
        foodtruckStaff: foodtruckStaff,
    });

    return createStore(
        rootReducer,
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers)
    );
}
