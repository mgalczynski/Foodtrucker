import React from 'react';
import {connect} from 'react-redux';
import {format} from '../../components/Helpers';
import './FoodtruckForm.css';
import {Modal, Grid, Row, Col, Button, Form, FormGroup, FormControl, ControlLabel} from 'react-bootstrap';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/FoodtruckForm';
import Select from 'react-select';
import {REPORTER} from '../Permisions';
import CloseButton from '../../components/CloseButton';

const FoodtruckForm = props => (
    <Modal show={props.isOpen} dialogClassName='foodtruck-form' bsSize='large'>
        <Modal.Header>
            <CloseButton onClick={props.close}/>
            {props.foodtruckSlug === null ? 'Add new foodtruck' : `Modify ${props.foodtruck.name}`}
        </Modal.Header>
        <Modal.Body>
            <Form>
                <FormGroup controlId='name'>
                    <ControlLabel>Short name</ControlLabel>
                    <FormControl
                        placeholder='Short name'
                        value={props.foodtruck.name}
                        onChange={e => props.changeName(e.target.value)}
                    />
                </FormGroup>
                <FormGroup controlId='displayName'>
                    <ControlLabel>Long name</ControlLabel>
                    <FormControl
                        placeholder='Long name'
                        value={props.foodtruck.displayName}
                        onChange={e => props.changeDisplayName(e.target.value)}
                    />
                </FormGroup>
                <FormGroup className='foodtruck-form-submit-form-group'>
                    <Button
                        type='submit'
                        onClick={props.save}
                        className='foodtruck-form-submit'
                    >
                        Save
                    </Button>
                </FormGroup>
            </Form>
        </Modal.Body>
    </Modal>
);

export default connect(
    (state, ownProps) => ({
        ...state.foodtruckForm, foodtruckSlug: ownProps.foodtruckSlug || null
    }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(FoodtruckForm);
