import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {Route} from 'react-router';
import Layout from './components/Layout';
import Map from './components/Map';
import Counter from './components/Counter';
import Login from './components/Login';
import Register from './components/Register';
import FetchData from './components/FetchData';
import {actionCreators} from './store/App';

class App extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return <Layout>
            <Route exact path={['/', '/foodtruck/:foodtruckSlug/:presenceId?']} component={Map}/>
            <Route path='/counter' component={Counter}/>
            <Route path='/login' component={Login}/>
            <Route path='/register' component={Register}/>
            <Route path='/fetchdata/:startDateIndex?' component={FetchData}/>
        </Layout>;
    }
}


export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(App);