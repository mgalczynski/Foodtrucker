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
      <Col sm={9} className={`${this.props.withoutMargin ? '' : 'col-main-custom-padding '}col-main`}>
        {props.children}
      </Col>
    </Row>
  </Grid>
);
