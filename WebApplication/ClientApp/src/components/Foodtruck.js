import React, { Component } from 'react';
import './Foodtruck.css'
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Modal } from 'react-bootstrap';
import { actionCreators } from '../store/Foodtruck';
import Loader from 'react-loader'
import SmallMap from './SmallMap';
import CloseButton from './CloseButton';
import Presence from './Presence';

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
        // abstract equality!
        const shouldRenderPresenceModal = this.props.match.params.presenceId != null;
        const presence = this.props.presences.find(p => p.id == this.props.match.params.presenceId) || null;
        return (
            <div>
                {this.props.foodtruck === null ?
                    <Modal show={true} className='foodtruck' bsSize='large'>
                        <Modal.Body className='foodtruck-loader'>
                            <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                                corners={1} rotate={0} direction={1} color='#000' speed={1}
                                trail={60} shadow={false} hwaccel={false}
                                zIndex={2e9} top='50%' left='50%' scale={1.00} />
                        </Modal.Body>
                    </Modal>
                    :
                    <Modal show={true} className='foodtruck' bsSize='large'>
                        <Modal.Header>
                            <CloseButton to='/' />
                            <Modal.Title>{this.props.foodtruck.name}</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {this.props.foodtruck.defaultLocation != null &&
                                <SmallMap
                                    position={this.props.position}
                                    latitude={this.props.foodtruck.defaultLocation.latitude}
                                    longitude={this.props.foodtruck.defaultLocation.longitude}
                                    mapId='foodtruck-map'
                                />
                            }
                        </Modal.Body>
                    </Modal>
                }
                {shouldRenderPresenceModal &&
                    <Presence presence={presence} position={this.props.position} />
                }
            </div>
        );
    }
}

export default connect(
    state => ({ ...state.foodtruckModal, position: state.map.position }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Foodtruck);
