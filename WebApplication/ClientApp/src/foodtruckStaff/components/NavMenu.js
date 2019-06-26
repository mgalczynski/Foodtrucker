import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Nav, Navbar, NavItem} from 'react-bootstrap';
import {LinkContainer} from 'react-router-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {useTranslation} from 'react-i18next';
import {actionCreators} from '../../store/App';
import '../../components/NavMenu.css';

class NavMenu extends Component {
    i18nFragment(i18n) {
        return () => <React.Fragment>
            <NavItem onClick={() => i18n.changeLanguage('en')}>
                <Glyphicon glyph='th-list'/> English
            </NavItem>
            <NavItem onClick={() => i18n.changeLanguage('pl')}>
                <Glyphicon glyph='th-list'/> Polski
            </NavItem>
        </React.Fragment>;
    }

    render() {
        const {t, i18n} = useTranslation();
        const I18nFragment = this.i18nFragment(i18n);
        return <Navbar inverse fixedTop fluid collapseOnSelect>
            <Navbar.Header>
                <Navbar.Brand>
                    <Link to={staffPrefix}>Foodtrucker - service panel</Link>
                </Navbar.Brand>
                <Navbar.Toggle/>
            </Navbar.Header>
            <Navbar.Collapse>
                {this.props.app.user ?
                    (
                        <ul className='pull-down nav navbar-nav'>
                            <I18nFragment/>
                            <Navbar.Text>
                                Welcome {this.props.app.user.firstName}&nbsp;{this.props.app.user.lastName}!
                            </Navbar.Text>
                            <LinkContainer to={`${staffPrefix}/changePassword`}>
                                <NavItem>
                                    <Glyphicon glyph='th-list'/> Change password
                                </NavItem>
                            </LinkContainer>
                            <NavItem onClick={this.props.logOut}>
                                <Glyphicon glyph='th-list'/> Log out
                            </NavItem>
                        </ul>
                    )
                    :
                    (
                        <Nav>
                            <I18nFragment/>
                            <LinkContainer exact to={`${staffPrefix}/`}>
                                <NavItem>
                                    <Glyphicon glyph='th-list'/> Sing in
                                </NavItem>
                            </LinkContainer>
                            <LinkContainer to={`${staffPrefix}/register`}>
                                <NavItem>
                                    <Glyphicon glyph='th-list'/> Sing up
                                </NavItem>
                            </LinkContainer>
                        </Nav>
                    )
                }
            </Navbar.Collapse>
        </Navbar>
    }
}

export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(NavMenu);