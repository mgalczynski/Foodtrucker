import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Login from '../components/Login';
import Register from '../components/Register';
import { staffPrefix } from '../Helpers'
import { actionCreators } from '../store/App';

class App extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return <Layout>
            <Route exact path={`${staffPrefix}`} component={Home} />
            <Route path={`${staffPrefix}/login`} render={(props) => <Login {...props} staff />} />
            <Route path={`${staffPrefix}/register`} render={(props) => <Register {...props} staff />} />
        </Layout>;
    }
}

export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(App);