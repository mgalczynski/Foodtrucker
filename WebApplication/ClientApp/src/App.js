import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {Route} from 'react-router';
import Layout from './components/Layout';
import Map from './components/Map';
import Login from './components/Login';
import Register from './components/Register';
import {actionCreators} from './store/App';

class App extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return <Layout>
            <Route exact path={['/', '/foodtruck/:foodtruckSlug/:presenceId?']} component={Map}/>
            <Route path='/login' component={Login}/>
            <Route path='/register' component={Register}/>
        </Layout>;
    }
}


export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(App);