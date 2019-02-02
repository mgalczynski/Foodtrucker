import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/StaffHome';
import Login from '../components/Login';
import Register from '../components/Register';
import { staffPrefix } from '../Helpers'
import { actionCreators } from '../store/App';
import Foodtruck from './components/Foodtruck';

class App extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return this.props.app.user === null ?
            <Layout>
                <Route exact path={`${staffPrefix}`} render={(props) => <Login {...props} staff/>}/>
                <Route path={`${staffPrefix}/register`} render={(props) => <Register {...props} staff/>}/>
            </Layout>
            :
            <Layout>
                <Route exact path={`${staffPrefix}`} component={Home}/>
                <Route exact path={`${staffPrefix}/foodtruck/:foodtruckSlug`} component={Foodtruck}/>
            </Layout>;
    }
}

export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(App);