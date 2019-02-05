import React from 'react';
import { connect } from 'react-redux';
import { Modal } from 'react-bootstrap';
import { format } from '../../components/Helpers';
import { Table } from 'react-bootstrap';
import { bindActionCreators } from 'redux';
import { actionCreators } from '../store/AddNewOwnership';
import Select from 'react-select';

const mapUserToString = (user) =>
    `${user.firstName} ${user.lastName} <${user.email}>`;

const mapUserToSelect = (user) =>
    ({ value: user, label: mapUserToString(user) });

const AddNewOwnership = props => (
    <Modal show={props.open}>
        <Modal.Header>
            Add new ownership
        </Modal.Header>
        <Select
            isLoading={props.loading}
            options={props.foundUsers.map(mapUserToSelect)}
            onInputChange={props.onQueryChanged}
            inputValue={props.query}
            value={props.user === null ? null : mapUserToSelect(props.user)}
            onChange={o => props.userChanged(o.value)}
        >
            {/* Add no options component */}

        </Select>
        <select className='form-control' value={props.type}
            onChange={(e) => props.changeAddNewOwnerType(e.target.value)}>
            {props.ownershipsList.map(o =>
                <option key={o} value={o}>{o}</option>
            )}
        </select>
    </Modal>
);

export default connect(
    (state, ownProps) => ({ ...state.addNewOwnership, ownershipsList: ownProps.ownershipsList }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(AddNewOwnership);
