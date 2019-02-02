import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Table, FormControl, InputGroup} from 'react-bootstrap';
import {LinkContainer} from 'react-router-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/StaffHome';

class StaffHome extends Component {
    componentDidMount = () => {
        this.props.updateFoodtrucks();
    };

    render() {
        return <div>
            <InputGroup>
                <FormControl
                    type='query'
                    placeholder='Filter foodtrucks'
                    onChange={e => this.props.changeQuery(e.target.value)}
                    value={this.props.query}
                />
            </InputGroup>
            <Table striped bordered hover>
                <thead>
                <tr>
                    <td>Name</td>
                    <td>Long name</td>
                    <td>Has default location</td>
                    <td>Your permissions</td>
                </tr>
                </thead>
                <tbody>
                {this.props.filteredFoodtrucks.map(f =>
                    <tr key={f.foodtruck.id}>
                        <td>{f.foodtruck.name}</td>
                        <td>{f.foodtruck.displayName}</td>
                        <td>{f.foodtruck.defaultLocation === null ? 'No' : 'Yes'}</td>
                        <td>{f.type}</td>
                    </tr>
                )}
                </tbody>
            </Table>
        </div>;
    }
}

export default connect(
    state => state.staffHome,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(StaffHome);