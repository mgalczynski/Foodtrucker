import React from 'react';
import { connect } from 'react-redux';
import { format } from '../../components/Helpers';
import './AddNewOwnership.css';
import { Modal, Grid, Row, Col, Button } from 'react-bootstrap';
import { bindActionCreators } from 'redux';
import { actionCreators } from '../store/AddNewOwnership';
import Select from 'react-select';
import { REPORTER } from '../Permisions';
import CloseButton from '../../components/CloseButton';

const mapUserToString = (user) =>
    `${user.firstName} ${user.lastName} <${user.email}>`;

const mapUserToSelect = (user) =>
    ({ value: user, label: mapUserToString(user) });

const labelToSelect = (label) =>
    ({ value: label, label });

const AddNewOwnership = props => (
    <Modal show={props.open} dialogClassName='add-new-ownership' bsSize='large'>
        <Modal.Header>
            <CloseButton onClick={props.close} />
            Add new ownership
        </Modal.Header>
        <Modal.Body>
            <Grid>
                <Row>
                    <Col sm={6} xs={6}>
                        <Select
                            isDisabled={props.isOngoingCreateRequest}
                            isLoading={props.loading}
                            options={props.foundUsers.map(mapUserToSelect)}
                            onInputChange={props.onQueryChanged}
                            inputValue={props.query}
                            value={props.user === null ? null : mapUserToSelect(props.user)}
                            onChange={o => props.userChanged(o.value)}
                        />
                    </Col>
                    <Col sm={6} xs={6}>
                        <Select
                            isDisabled={props.isOngoingCreateRequest}
                            options={props.ownershipsList.map(labelToSelect)}
                            onChange={props.changeAddNewOwnerType}
                            defaultValue={labelToSelect(REPORTER)}
                        />
                    </Col>
                </Row>
                <Row>
                    <span className='pull-right'>
                        <Button
                            type='submit'
                            onClick={() => props.createNewOwnership(props.foodtruckSlug)}
                            className='add-new-ownership-submit'
                        >
                            Create new ownership
                        </Button>
                    </span>
                </Row>
            </Grid>
        </Modal.Body>
    </Modal>
);

export default connect(
    (state, ownProps) => ({
        ...state.addNewOwnership,
        ownershipsList: ownProps.ownershipsList,
        foodtruckSlug: ownProps.foodtruckSlug
    }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(AddNewOwnership);
