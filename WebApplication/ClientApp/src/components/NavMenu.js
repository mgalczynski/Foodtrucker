import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actionCreators } from '../store/App';
import { staffPrefix } from '../Helpers';

class NavMenu extends Component {
    render() {
        return (
            <Navbar inverse fixedTop fluid collapseOnSelect>
                <Navbar.Header>
                    <Navbar.Brand>
                        <Link to={'/'}>Foodtrucker</Link>
                    </Navbar.Brand>
                    <Navbar.Toggle />
                </Navbar.Header>
                <Navbar.Collapse>
                    <Nav>
                        <LinkContainer to={'/'} exact>
                            <NavItem>
                                <Glyphicon glyph='home' /> Home
                            </NavItem>
                        </LinkContainer>
                    </Nav>
                    {this.props.app.user ?
                        (
                            <ul className='pull-down nav navbar-nav'>
                                <Navbar.Text>
                                    Welcome {this.props.app.user.firstName}&nbsp;{this.props.app.user.lastName}!
                                </Navbar.Text>
                                <NavItem onClick={this.props.logOut}>
                                    <Glyphicon glyph='th-list' /> Log out
                                </NavItem>
                            </ul>
                        )
                        :
                        (
                            <ul className='pull-down nav navbar-nav'>
                                <LinkContainer to={'/login'}>
                                    <NavItem>
                                        <Glyphicon glyph='th-list' /> Sing in
                                    </NavItem>
                                </LinkContainer>
                                <LinkContainer to={'/register'}>
                                    <NavItem>
                                        <Glyphicon glyph='th-list' /> Sing up
                                    </NavItem>
                                </LinkContainer>
                                <LinkContainer to={staffPrefix} className='navbar-small'>
                                    <NavItem>
                                        <Glyphicon glyph='th-list' /> Are you an employee or owner of foodtruck, click here to add your foodtruck to this app
                                    </NavItem>
                                </LinkContainer>
                            </ul>
                        )
                    }
                </Navbar.Collapse>
            </Navbar>
        );
    }
}

export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(NavMenu);