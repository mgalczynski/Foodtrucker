import React, { Component } from 'react';
import './Foodtruck.css'
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Modal } from 'react-bootstrap';
import { actionCreators } from '../store/Foodtruck';
import Loader from 'react-loader'
import SmallMap from './SmallMap';

class Foodtruck extends Component {
    componentDidMount = () => {
        this.props.loadFoodtruck(this.props.match.params.foodtruckSlug);
    };
    componentWillReceiveProps = (props) => {
        this.props.loadFoodtruck(props.match.params.foodtruckSlug);
    };
    componentWillUnmount = () => {
        this.props.clear();
    }
    render() {
        return this.props.foodtruck === null ?
            <Modal show={true} className='foodtruck'>
                <Modal.Body className='foodtruck-loader'>
                    <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                        corners={1} rotate={0} direction={1} color="#000" speed={1}
                        trail={60} shadow={false} hwaccel={false}
                        zIndex={2e9} top="50%" left="50%" scale={1.00} />
                </Modal.Body>
            </Modal>
            :
            <Modal show={true} className='foodtruck'>
                <Modal.Header closeButton>
                    <Modal.Title>{this.props.foodtruck.name}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {this.props.foodtruck.defaultLocation != null &&
                        <SmallMap
                            position={this.props.position}
                            latitude={this.props.foodtruck.defaultLocation.latitude}
                            longitude={this.props.foodtruck.defaultLocation.longitude}
                        />
                    }
                </Modal.Body>
            </Modal>;
    }
}

export default connect(
    state => ({ ...state.foodtruckModal, position: state.map.position }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Foodtruck);
