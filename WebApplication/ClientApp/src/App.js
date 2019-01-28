import React from 'react';
import {Route} from 'react-router';
import Layout from './components/Layout';
import Map from './components/Map';
import Counter from './components/Counter';
import Login from './components/Login';
import Register from './components/Register';
import FetchData from './components/FetchData';
import FoodtruckStaff from './foodtruckStaff/App';

export default () => (
    <Layout>
        <Route exact path={['/', '/foodtruck/:foodtruckUrlName/:presenceId?']} component={Map}/>
        <Route path='/counter' component={Counter}/>
        <Route path='/login' component={Login}/>
        <Route path='/register' component={Register}/>
        <Route path='/foodtruckStaff' component={FoodtruckStaff}/>
        <Route path='/fetchdata/:startDateIndex?' component={FetchData}/>
    </Layout>
);
