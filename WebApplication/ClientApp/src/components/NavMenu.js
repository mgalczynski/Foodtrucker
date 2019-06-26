import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Nav, Navbar, NavItem} from 'react-bootstrap';
import {LinkContainer} from 'react-router-bootstrap';
import './NavMenu.css';
import {connect} from 'react-redux';
import {useTranslation} from 'react-i18next';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/App';
import {staffPrefix} from '../Helpers';

class NavMenu extends Component {
    i18nFragments(i18n) {
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
        const i18nFragments = i18nFragments(i18n);
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
                    </Nav>
                    {this.props.app.user ?
                        (
                            <ul className='pull-down nav navbar-nav'>
                                <i18nFragments/>
                                <Navbar.Text>
                                    Welcome {this.props.app.user.firstName}&nbsp;{this.props.app.user.lastName}!
                                </Navbar.Text>
                                <LinkContainer to={`/changePassword`}>
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
                            <ul className='pull-down nav navbar-nav'>
                                <i18nFragments/>
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
                                <LinkContainer to={staffPrefix} className='navbar-small'>
                                    <NavItem>
                                        <Glyphicon
                                            glyph='th-list'/>{t('Are you an employee or owner of foodtruck, click here to add your foodtruck to this app')}
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