import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {Route, Switch} from 'react-router';
import Layout from './components/Layout';
import Map from './components/Map';
import Login from './components/Login';
import Register from './components/Register';
import {actionCreators} from './store/App';

const WithMargin = () => (
    <Layout>
        <Route path='/login' component={Login} />
        <Route path='/register' component={Register} />
    </Layout>
);

const WithoutMargin = () => (
    <Layout withoutMargin>
        <Map />
    </Layout>
);


class App extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return <Switch>
            <Route exact path='/' component={WithoutMargin} />
            <Route exact path='/foodtruck/:foodtruckSlug/:presenceId?' component={WithoutMargin} />
            <Route component={WithMargin} />
        </Switch>;
    }
}


export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(App);