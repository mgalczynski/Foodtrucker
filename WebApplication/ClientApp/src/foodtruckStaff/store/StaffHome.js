import {push, LOCATION_CHANGE as urlChanged} from 'react-router-redux';
import {staffPrefix} from '../../Helpers';
import {actionCreators as FoodtruckForm} from './FoodtruckForm';

const foodtrucksChanged = 'staff/home/FOODTRUCKS_CHANGED';
const foodtruckDeleted = 'staff/home/FOODTRUCK_DELETED';

const initialState = {
    foodtrucks: [],
    query: '',
    filteredFoodtrucks: [],
    allAreVisible: true,
};

const mapQueryToArgs = (query) =>
    query.split(' ').filter(v => v.length > 2).map(v => v.toLowerCase());

const filter = (foodtrucks, args) => {
    const visibleFoodtrucks = (args.length === 0 ? foodtrucks : foodtrucks.filter(f => args.every(a => f.foodtruck.name.toLowerCase().includes(a) || f.foodtruck.displayName.toLowerCase().includes(a))));
    return {filteredFoodtrucks: visibleFoodtrucks.slice(0, 10), allAreVisible: visibleFoodtrucks.length <= 10};
};

const getQuery = (search) =>
    decodeURIComponent(new URLSearchParams(search).get('q') || '');

export const actionCreators = {
    updateFoodtrucks: () => async (dispatch, getState) => {
        const response = await fetch(`api${staffPrefix}/foodtruck`, {
            credentials: 'same-origin',
        });
        const result = (await response.json()).result;
        result.sort((f1, f2) => f1.foodtruck.name.localeCompare(f2.foodtruck.name));
        dispatch({
            type: foodtrucksChanged,
            foodtrucks: result,
            search: getState()
        });
    },
    deleteFoodtruck: (foodtruck) => async (dispatch, getState) => {
        dispatch({type: foodtruckDeleted, slug: foodtruck});
        const response = await fetch(`api${staffPrefix}/foodtruck/${foodtruck}`,
            {
                credentials: 'same-origin',
                method: 'DELETE',
            }
        );
        if (response.status !== 200)
            await actionCreators.updateFoodtrucks()(dispatch, getState);
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
        case foodtruckDeleted:
            const foodtrucks = state.foodtrucks.filter(f => f.foodtruck.slug !== action.slug);
            return {
                ...state,
                foodtrucks,
                ...filter(foodtrucks, mapQueryToArgs(state.query)),
            };
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