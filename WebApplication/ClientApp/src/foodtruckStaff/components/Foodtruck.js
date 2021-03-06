import React, {Component, Fragment} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Table, Button, FormControl, Modal, Alert} from 'react-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/Foodtruck';
import {ADMIN, OWNER, ownershipMap} from '../Permisions';
import Loader from 'react-loader';
import SmallMap from '../../components/SmallMap';
import PresencesOrUnavailabilitiesList from './PresencesOrUnavailabilitiesList';
import './Foodtruck.css';
import AddNewOwnership from './AddNewOwnership';
import PresenceOrUnavailabilityForm from './PresenceOrUnavailabilityForm';
import FoodtruckForm from "./FoodtruckForm";


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
    
    ownershipType = () =>
        this.props.foodtruck.ownerships.find(o => o.user.email === this.props.user.email).type;
    
    ownershipsList = () =>
        ownershipMap.get(this.ownershipType());

    canManipulate = (ownership) =>
        ownership.user.email !== this.props.user.email &&
        this.ownershipsList().includes(ownership.type);

    render() {
        if (this.props.notFound)
            return <Alert bsStyle='danger'>Not found</Alert>;
        else if (this.props.foodtruck === null)
            return <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                           corners={1} rotate={0} direction={1} color='#000' speed={1}
                           trail={60} shadow={false} hwaccel={false}
                           zIndex={2e9} top='50%' left='50%' scale={1.00}/>
        else return this.renderFoodtruckDetails();
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
                <AddNewOwnership
                    ownershipsList={ownershipsList}
                    foodtruckSlug={this.props.foodtruck.slug}
                />
                <PresenceOrUnavailabilityForm
                    position={this.props.position}
                />
                {/*<Button onClick={this.props.openNewPresenceModal}>Modify foodtruck</Button>*/}
                <Button onClick={this.props.openNewOwnershipModal}>Add new ownership</Button>
                <Button onClick={this.props.openNewPresenceOrUnavailabilityModal}>Add new presence</Button>
                {[ADMIN, OWNER].includes(this.ownershipType()) &&
                <Button onClick={() => this.props.modifyFoodtruck(this.props.foodtruck)}>Modify</Button>}
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
                    {this.props.foodtruck.ownerships.map(ownership =>
                        <tr key={ownership.user.email}>
                            <td>{ownership.user.firstName}</td>
                            <td>{ownership.user.lastName}</td>
                            <td><a
                                href={`mailto:${ownership.user.firstName} ${ownership.user.lastName}\<${ownership.user.email}\>`}>{ownership.user.email}</a>
                            </td>
                            {this.canManipulate(ownership) ?
                                <Fragment>
                                    <td>
                                        <select className='form-control' value={ownership.type}
                                                onChange={(e) => this.props.changeOwnership(
                                                    ownership.user.email,
                                                    e.target.value
                                                )}>
                                            {ownershipsList.map(o =>
                                                <option key={o} value={o}>{o}</option>
                                            )}
                                        </select>
                                    </td>
                                    <td>
                                        <Button
                                            variant='primary'
                                            onClick={() => this.props.removeOwnership(ownership.user.email)}
                                        >
                                            Remove
                                        </Button>
                                    </td>
                                </Fragment>
                                :
                                <Fragment>
                                    <td>{ownership.type}</td>
                                    <td/>
                                </Fragment>
                            }
                        </tr>
                    )}
                    </tbody>
                </Table>
                <FoodtruckForm reload={this.props.updateFoodtruck}/>
                <PresencesOrUnavailabilitiesList
                    presencesOrUnavailabilities={this.props.foodtruck.presencesOrUnavailabilities}
                    modifyPresenceOrUnavailability={this.props.openModifyPresenceOrUnavailabilityModal}
                    removePresenceOrUnavailability={this.props.removePresenceOrUnavailability}
                />
            </div>;
    }
}

export default connect(
    state => ({...state.foodtruckForStaff, user: state.app.user}),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Foodtruck);