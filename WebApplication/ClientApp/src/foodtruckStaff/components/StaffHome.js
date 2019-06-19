import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Table, FormControl, Alert, Button} from 'react-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/StaffHome';
import FoodtruckForm from './FoodtruckForm';
import {ADMIN, OWNER} from '../Permisions';

class StaffHome extends Component {
    componentDidMount = () => {
        this.props.updateFoodtrucks();
    };

    render() {
        return <div>
            <FormControl
                type='query'
                placeholder='Filter foodtrucks'
                onChange={e => this.props.changeQuery(e.target.value)}
                value={this.props.query}
            />
            <Button onClick={this.props.openAddNewFoodtruckModal}>
                Add new foodtruck
            </Button>
            <Table striped bordered hover>
                <thead>
                <tr>
                    <td>Name</td>
                    <td>Long name</td>
                    <td>Has default location</td>
                    <td>Your permissions</td>
                    <td>Edit</td>
                    <td>Delete</td>
                </tr>
                </thead>
                <tbody>
                {this.props.filteredFoodtrucks.map(f =>
                    <tr key={f.foodtruck.slug}>
                        <td><Link to={`${staffPrefix}/foodtruck/${f.foodtruck.slug}`}>{f.foodtruck.name}</Link></td>
                        <td>
                            <Link to={`${staffPrefix}/foodtruck/${f.foodtruck.slug}`}>{f.foodtruck.displayName}</Link>
                        </td>
                        <td>{f.foodtruck.defaultLocation === null ? 'No' : 'Yes'}</td>
                        <td>{f.type}</td>
                        <td>
                            {[ADMIN, OWNER].includes(f.type) &&
                            <Button onClick={() => this.props.modifyFoodtruck(f.foodtruck)}>Modify</Button>
                            }
                        </td>
                        <td>
                            {f.type === OWNER &&
                            <Button onClick={() => this.props.deleteFoodtruck(f.foodtruck.slug)}>Delete</Button>
                            }
                        </td>
                    </tr>
                )}
                </tbody>
            </Table>
            {this.props.allAreVisible ||
            <Alert bsStyle="warning"><strong>Not all foodtrucks are visible!</strong> Use search box to find not visible
                foodtrucks</Alert>}
            <FoodtruckForm/>
        </div>;
    }
}

export default connect(
    state => state.staffHome,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(StaffHome);