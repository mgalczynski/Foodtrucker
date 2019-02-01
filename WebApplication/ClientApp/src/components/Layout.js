import React, {Component} from 'react';
import {Col, Grid, Row} from 'react-bootstrap';
import NavMenu from './NavMenu';

export default class Layout extends Component {
    render() {
        return (
            <Grid fluid>
                <Row>
                    <Col sm={3} className='col-nav-menu'>
                        <NavMenu/>
                    </Col>
                    <Col sm={9} className='col-main'>
                        {this.props.children}
                    </Col>
                </Row>
            </Grid>
        );
    }
}