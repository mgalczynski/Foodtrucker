import React, { Component } from 'react';
import './PresenceOrUnavailability.css'
import { Modal } from 'react-bootstrap';
import Loader from 'react-loader'
import SmallMap from './SmallMap';
import CloseButton from './CloseButton';
import { format } from './Helpers';
import moment from 'moment';

export default class PresenceOrUnavailability extends Component {
    render() {
        return this.props.presenceOrUnavailability === null ?
            <Modal show className='presence-or-unavailability'>
                <Modal.Body className='presence-or-unavailability-loader'>
                    <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                        corners={1} rotate={0} direction={1} color='#000' speed={1}
                        trail={60} shadow={false} hwaccel={false}
                        zIndex={2e9} top='50%' left='50%' scale={1.00} />
                </Modal.Body>
            </Modal>
            :
            <Modal show className='presence-or-unavailability'>
                <Modal.Header>
                    <CloseButton to={`/foodtruck/${this.props.presenceOrUnavailability.foodtruckSlug}`} />
                    <Modal.Title>{this.props.presenceOrUnavailability.title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className='presence-or-unavailability-details'>
                        <h5>{this.props.presenceOrUnavailability.description}</h5>
                        Start time: {moment.utc(this.props.presenceOrUnavailability.startTime).local().format(format)}<br/>
                        End time: {this.props.presenceOrUnavailability.endTime === null ? '-----' : moment.utc(this.props.presenceOrUnavailability.startTime).local().format(format)}
                    </div>
                    {this.props.presenceOrUnavailability.location &&
                        <SmallMap
                            position={this.props.position}
                            latitude={this.props.presenceOrUnavailability.location.latitude}
                            longitude={this.props.presenceOrUnavailability.location.longitude}
                            mapId='presence-or-unavailability-map'
                        />
                    }
                </Modal.Body>
            </Modal>;
    }
}