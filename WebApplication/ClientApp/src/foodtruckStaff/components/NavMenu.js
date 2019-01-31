import React from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { foodtruckPrefix } from '../../Helpers'
import '../../components/NavMenu.css';

export default props => (
  <Navbar inverse fixedTop fluid collapseOnSelect>
    <Navbar.Header>
      <Navbar.Brand>
        <Link to={foodtruckPrefix}>Foodtrucker - service panel</Link>
      </Navbar.Brand>
      <Navbar.Toggle />
    </Navbar.Header>
    <Navbar.Collapse>
      <Nav>
        <LinkContainer to={foodtruckPrefix} exact>
          <NavItem>
            <Glyphicon glyph='home' /> Home
          </NavItem>
        </LinkContainer>
        <LinkContainer to={`${foodtruckPrefix}/counter`}>
          <NavItem>
            <Glyphicon glyph='education' /> Counter
          </NavItem>
        </LinkContainer>
        <LinkContainer to={`${foodtruckPrefix}/fetchdata`}>
          <NavItem>
            <Glyphicon glyph='th-list' /> Fetch data
          </NavItem>
        </LinkContainer>
      </Nav>
    </Navbar.Collapse>
  </Navbar>
);
