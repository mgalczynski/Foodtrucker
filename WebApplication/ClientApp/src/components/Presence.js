import React, { Component } from 'react';
import './Presence.css'
import { Modal } from 'react-bootstrap';
import Loader from 'react-loader'
import SmallMap from './SmallMap';
import CloseButton from './CloseButton';
import { format } from './Helpers';

export default class Presence extends Component {
    render() {
        return this.props.presence === null ?
            <Modal show className='presence'>
                <Modal.Body className='presence-loader'>
                    <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                        corners={1} rotate={0} direction={1} color='#000' speed={1}
                        trail={60} shadow={false} hwaccel={false}
                        zIndex={2e9} top='50%' left='50%' scale={1.00} />
                </Modal.Body>
            </Modal>
            :
            <Modal show className='presence'>
                <Modal.Header>
                    <CloseButton to={`/foodtruck/${this.props.presence.foodtruckSlug}`} />
                    <Modal.Title>{this.props.presence.title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className='presence-details'>
                        <h5>{this.props.presence.description}</h5>
                        Start time: {this.props.presence.startTime.format(format)}<br/>
                        End time: {this.props.presence.endTime === null ? "-----" : this.props.presence.startTime.format(format)}
                    </div>
                    <SmallMap
                        position={this.props.position}
                        latitude={this.props.presence.location.latitude}
                        longitude={this.props.presence.location.longitude}
                        mapId='presence-map'
                    />
                </Modal.Body>
            </Modal>;
    }
}