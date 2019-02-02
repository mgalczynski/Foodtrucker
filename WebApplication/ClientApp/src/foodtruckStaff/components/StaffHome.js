import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Nav, Navbar, NavItem} from 'react-bootstrap';
import {LinkContainer} from 'react-router-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from "react-redux";
import {bindActionCreators} from "redux";
import {actionCreators} from '../store/StaffHome';

class StaffHome extends Component {
    componentDidMount = () => {
        this.props.updateFoodtrucks();
    };

    render() {
        return null;
    }
}

export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(StaffHome);