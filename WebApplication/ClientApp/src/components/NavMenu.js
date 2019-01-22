import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Nav, Navbar, NavItem} from 'react-bootstrap';
import {LinkContainer} from 'react-router-bootstrap';
import './NavMenu.css';
import {connect} from "react-redux";
import {bindActionCreators} from "redux";
import {actionCreators} from "../store/App";

class NavMenu extends Component {
    render() {
        return (
            <Navbar inverse fixedTop fluid collapseOnSelect>
                <Navbar.Header>
                    <Navbar.Brand>
                        <Link to={'/'}>Foodtrucker</Link>
                    </Navbar.Brand>
                    <Navbar.Toggle/>
                </Navbar.Header>
                <Navbar.Collapse>
                    <Nav>
                        <LinkContainer to={'/'} exact>
                            <NavItem>
                                <Glyphicon glyph='home'/> Home
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/counter'}>
                            <NavItem>
                                <Glyphicon glyph='education'/> Counter
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/fetchdata'}>
                            <NavItem>
                                <Glyphicon glyph='th-list'/> Fetch data
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
                                    <Glyphicon glyph='th-list'/> Log out
                                </NavItem>
                            </ul>
                        )
                        :
                        (
                            <ul className='pull-down nav navbar-nav'>
                                <LinkContainer to={'/login'}>
                                    <NavItem>
                                        <Glyphicon glyph='th-list'/> Sing in
                                    </NavItem>
                                </LinkContainer>
                                <LinkContainer to={'/register'}>
                                    <NavItem>
                                        <Glyphicon glyph='th-list'/> Sing up
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