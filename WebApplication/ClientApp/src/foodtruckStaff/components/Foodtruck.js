import React, {Component, Fragment} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Table, Button, InputGroup, Modal} from 'react-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/Foodtruck';
import Loader from 'react-loader';
import SmallMap from '../../components/SmallMap';
import {format} from '../../components/Helpers';
import './Foodtruck.css';

const OWNER = 'OWNER';
const ADMIN = 'ADMIN';
const REPORTER = 'REPORTER';
const ownershipMap = new Map([
    [OWNER, new Set([OWNER, ADMIN, REPORTER])],
    [ADMIN, new Set([REPORTER])],
    [REPORTER, new Set()],
]);

class Foodtruck extends Component {
    componentDidMount = () => {
        this.props.loadFoodtruck(this.props.match.params.foodtruckSlug);
        this.props.watchPosition();
    };
    componentWillReceiveProps = (props) => {
        this.props.loadFoodtruck(props.match.params.foodtruckSlug);
    };
    componentWillUnmount = () => {
        this.props.clear();
    };
    canManipulate = (ownership) =>
        ownership.user.email !== this.props.user.email &&
        ownershipMap.get(this.props.foodtruck.ownerships.find(o => o.user.email = this.props.user.email).type)
            .has(ownership.type)
    ;

    render() {
        return this.props.foodtruck === null ?
            <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                    corners={1} rotate={0} direction={1} color='#000' speed={1}
                    trail={60} shadow={false} hwaccel={false}
                    zIndex={2e9} top='50%' left='50%' scale={1.00}/>
            :
            <div className='foodtruck'>
                <h1>{this.props.foodtruck.name}</h1>
                <h3>{this.props.foodtruck.displayName}</h3>
                {this.props.foodtruck.defaultLocation !== null &&
                <SmallMap
                    mapId={'foodtruck-map'}
                    latitude={this.props.foodtruck.defaultLocation.latitude}
                    longitude={this.props.foodtruck.defaultLocation.longitude}
                    position={this.props.position}
                />}
                <Table striped bordered hover>
                    <thead>
                    <tr>
                        <td>First name</td>
                        <td>Last name</td>
                        <td>E-mail</td>
                        <td>Type</td>
                        <td>Change type</td>
                        <td>Remove</td>
                    </tr>
                    </thead>
                    <tbody>
                    {this.props.foodtruck.ownerships.map(o =>
                        <tr key={o.user.email}>
                            <td>{o.user.firstName}</td>
                            <td>{o.user.lastName}</td>
                            <td><a
                                href={`mailto:${o.user.firstName} ${o.user.lastName}\<${o.user.email}\>`}>{o.user.email}</a>
                            </td>
                            <td>{o.type}</td>
                            {this.canManipulate(o) ?
                                <Fragment>
                                    <td><Button variant='primary'>Change Type</Button></td>
                                    <td><Button variant='primary'
                                                onClick={() => this.props.removeOwnership(
                                                    this.props.match.params.foodtruckSlug,
                                                    o.user.email
                                                )}>
                                        Remove
                                    </Button>
                                    </td>
                                </Fragment>
                                :
                                <Fragment>
                                    <td></td>
                                    <td></td>
                                </Fragment>
                            }
                        </tr>
                    )}
                    </tbody>
                </Table>
                <Table striped bordered hover>
                    <thead>
                    <tr>
                        <td>Start time</td>
                        <td>End time</td>
                        <td>Title</td>
                        <td>Description</td>
                    </tr>
                    </thead>
                    <tbody>
                    {this.props.foodtruck.presences.map(p =>
                        <tr key={p.id}>
                            <td>{p.startTime.format(format)}</td>
                            <td>{p.startTime == null ? '----' : p.startTime.format(format)}</td>
                            <td>{p.title}</td>
                            <td>{p.description}</td>
                        </tr>
                    )}
                    </tbody>
                </Table>
            </div>
            ;
    }
}

export default connect(
    state => ({...state.foodtruckForStaff, user: state.app.user}),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Foodtruck);