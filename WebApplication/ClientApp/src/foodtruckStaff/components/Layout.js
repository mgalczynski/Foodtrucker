import React from 'react';
import '../../components/Layout.css';
import { Col, Grid, Row } from 'react-bootstrap';
import NavMenu from './NavMenu';

export default props => (
  <Grid fluid>
    <Row>
      <Col sm={3} className='col-nav-menu'>
        <NavMenu />
      </Col>
      <Col sm={9} className={`col-main${props.withoutMargin ? '' : ' col-main-custom-margin'}`}>
        {props.children}
      </Col>
    </Row>
  </Grid>
);
