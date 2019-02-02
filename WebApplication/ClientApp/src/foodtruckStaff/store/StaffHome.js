import {push, LOCATION_CHANGE as urlChanged} from 'react-router-redux';

const foodtrucksChanged = 'staff/home/FOODTRUCKS_CHANGED';

const initialState = {
    foodtrucks: [],
    query: '',
    filteredFoodtrucks: []
};

const mapQueryToArgs = (query) =>
    query.split(' ').filter(v => v.length > 0).map(v => v.toLowerCase());

const filter = (foodtrucks, args) =>
    args.length === 0 ? foodtrucks : foodtrucks.filter(f => args.some(a => f.name.toLowerCase().contains.includes(a) || f.displayName.toLowerCase().contains.includes(a)));

const getQuery = (search) =>
    decodeURIComponent(new URLSearchParams(search).get('q') || '');

export const actionCreators = {
    updateFoodtrucks: () => async (dispatch, getState) => {
        const response = await fetch(`api/staff/foodtruck`);
        const result = await response.json();
        dispatch({type: foodtrucksChanged, foodtrucks: result.result, search: getState()});
    },
    changeQuery: (query) => async (dispatch) => {
        const q = encodeURIComponent(query);
        dispatch(push({
            search: q.length === 0 ? '' : `q=${q}`
        }));
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    switch (action.type) {
        case foodtrucksChanged:
            return {
                ...state,
                foodtrucks: action.foodtrucks,
                filteredFoodtrucks: filter(action.foodtrucks, mapQueryToArgs(state.query))
            };
        case urlChanged: {
            const query = getQuery(action.payload.search);
            return {
                ...state,
                query,
                filteredFoodtrucks: filter(state.foodtrucks, mapQueryToArgs(query))
            };
        }
        default:
            return state;
    }
};