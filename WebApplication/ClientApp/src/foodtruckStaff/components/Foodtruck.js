import React, {Component, Fragment} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Table, Button, FormControl, Modal} from 'react-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/Foodtruck';
import {ownershipMap} from '../Permisions';
import Loader from 'react-loader';
import SmallMap from '../../components/SmallMap';
import PresencesList from './PresencesList';
import './Foodtruck.css';
import AddNewOwnership from './AddNewOwnership';


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
    ownershipsList = () =>
        ownershipMap.get(this.props.foodtruck.ownerships.find(o => o.user.email = this.props.user.email).type);

    canManipulate = (ownership) =>
        ownership.user.email !== this.props.user.email &&
        this.ownershipsList().includes(ownership.type);

    render() {
        return this.props.foodtruck === null ?
            <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                    corners={1} rotate={0} direction={1} color='#000' speed={1}
                    trail={60} shadow={false} hwaccel={false}
                    zIndex={2e9} top='50%' left='50%' scale={1.00}/>
            : this.renderFoodtruckDetails();
    }

    renderFoodtruckDetails() {
        const ownershipsList = this.ownershipsList();
        return <div className='foodtruck'>
            <h1>{this.props.foodtruck.name}</h1>
            <h3>{this.props.foodtruck.displayName}</h3>
            {this.props.foodtruck.defaultLocation !== null &&
            <SmallMap
                mapId={'foodtruck-map'}
                latitude={this.props.foodtruck.defaultLocation.latitude}
                longitude={this.props.foodtruck.defaultLocation.longitude}
                position={this.props.position}
            />}
            <AddNewOwnership ownershipsList={ownershipsList}/>
            <Button onClick={this.props.openNewOwnershipModal}>Add new ownership</Button>
            <Table striped bordered hover>
                <thead>
                <tr>
                    <td>First name</td>
                    <td>Last name</td>
                    <td>E-mail</td>
                    <td>Type</td>
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
                        {this.canManipulate(o) ?
                            <Fragment>
                                <td>
                                    <select className='form-control' value={o.type}
                                            onChange={(e) => this.props.changeOwnership(
                                                this.props.match.params.foodtruckSlug,
                                                o.user.email,
                                                e.target.value
                                            )}>
                                        {ownershipsList.map(o =>
                                            <option key={o} value={o}>{o}</option>
                                        )}
                                    </select>
                                </td>
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
                                <td>{o.type}</td>
                                <td/>
                            </Fragment>
                        }
                    </tr>
                )}
                </tbody>
            </Table>
            <PresencesList presences={this.props.foodtruck.presences}/>
        </div>;
    }
}

export default connect(
    state => ({...state.foodtruckForStaff, user: state.app.user}),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Foodtruck);