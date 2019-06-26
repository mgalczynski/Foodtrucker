import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {Route, Switch} from 'react-router';
import {I18nextProvider} from 'react-i18next';
import Layout from './components/Layout';
import Map from './components/Map';
import Login from './components/Login';
import Register from './components/Register';
import {actionCreators} from './store/App';
import ChangePassword from './components/ChangePassword';
import i18n from './i18n';

const WithMargin = () => (
    <Layout>
        <Route path='/login' component={Login}/>
        <Route path='/register' component={Register}/>
        <Route path='/changePassword' component={ChangePassword}/>
    </Layout>
);

const WithoutMargin = () => (
    <Layout withoutMargin>
        <Map/>
    </Layout>
);


class App extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return <Switch>
            <Route exact path='/' component={WithoutMargin}/>
            <Route exact path='/foodtruck/:foodtruckSlug/:presenceOrUnavailabilityId?' component={WithoutMargin}/>
            <Route component={WithMargin}/>
        </Switch>;
    }
}


export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(App);