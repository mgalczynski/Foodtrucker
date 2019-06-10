import {push, LOCATION_CHANGE as urlChanged} from 'react-router-redux';
import {staffPrefix} from '../../Helpers';
import {actionCreators as FoodtruckForm} from './FoodtruckForm';

const foodtrucksChanged = 'staff/home/FOODTRUCKS_CHANGED';

const initialState = {
    foodtrucks: [],
    query: '',
    filteredFoodtrucks: [],
    allAreVisible: true,
};

const mapQueryToArgs = (query) =>
    query.split(' ').filter(v => v.length > 0).map(v => v.toLowerCase());

const filter = (foodtrucks, args) => {
    const visibleFoodtrucks = (args.length === 0 ? foodtrucks : foodtrucks.filter(f => args.some(a => f.foodtruck.name.toLowerCase().includes(a) || f.foodtruck.displayName.toLowerCase().includes(a))));
    return {filteredFoodtrucks: visibleFoodtrucks.slice(0, 10), allAreVisible: visibleFoodtrucks.length <= 10};
};

const getQuery = (search) =>
    decodeURIComponent(new URLSearchParams(search).get('q') || '');

export const actionCreators = {
    updateFoodtrucks: () => async (dispatch, getState) => {
        const response = await fetch(`api${staffPrefix}/foodtruck`);
        const result = (await response.json()).result;
        result.sort((f1, f2) => f1.foodtruck.name.localeCompare(f2.foodtruck.name));
        dispatch({
            type: foodtrucksChanged,
            foodtrucks: result,
            search: getState()
        });
    },
    changeQuery: (query) => async (dispatch) => {
        const q = encodeURIComponent(query);
        dispatch(push({
            search: q.length === 0 ? '' : `q=${q}`
        }));
    },
    openAddNewFoodtruckModal: () => async (dispatch) => {
        await FoodtruckForm.open()(dispatch);
    },
    modifyFoodtruck: (foodtruck) => async (dispatch) => {
        await FoodtruckForm.openWithFoodtruck(foodtruck)(dispatch);
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtrucksChanged:
            return {
                ...state,
                foodtrucks: action.foodtrucks,
                ...filter(action.foodtrucks, mapQueryToArgs(state.query)),
            };
        case urlChanged: {
            const query = getQuery(action.payload.search);
            return {
                ...state,
                query,
                ...filter(state.foodtrucks, mapQueryToArgs(query)),
            };
        }
        default:
            return state;
    }
};