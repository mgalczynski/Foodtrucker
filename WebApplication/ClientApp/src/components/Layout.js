import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {Col, Grid, Row} from 'react-bootstrap';
import NavMenu from './NavMenu';
import {actionCreators} from '../store/App';

class Layout extends Component {
    componentDidMount = () => {
        this.props.checkUser();
    };

    render() {
        return (
            <Grid fluid>
                <Row>
                    <Col sm={3}>
                        <NavMenu/>
                    </Col>
                    <Col sm={9}>
                        {this.props.children}
                    </Col>
                </Row>
            </Grid>
        );
    }
}

export default connect(
    state => state,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Layout);
