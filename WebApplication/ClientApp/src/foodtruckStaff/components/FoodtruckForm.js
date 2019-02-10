import React, {Component} from 'react';
import {connect} from 'react-redux';
import {format} from '../../components/Helpers';
import './FoodtruckForm.css';
import {Modal, Grid, Row, Col, Button, Form, FormGroup, FormControl, ControlLabel, Checkbox} from 'react-bootstrap';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/FoodtruckForm';
import Select from 'react-select';
import {REPORTER} from '../Permisions';
import CloseButton from '../../components/CloseButton';
import LocationPicker from './LocationPicker';

class FoodtruckForm extends Component {
    onSubmit = (e) => {
        e.preventDefault();
        this.props.save(this.props.foodtruckSlug);
    };

    render() {
        return <Modal show={this.props.isOpen} dialogClassName='foodtruck-form' bsSize='large'>
            <Modal.Header>
                <CloseButton onClick={this.props.close}/>
                {this.props.slug === null ? 'Add new foodtruck' : `Modify ${this.props.foodtruck.name}`}
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={this.onSubmit}>
                    <FormGroup controlId='name'>
                        <ControlLabel>Short name</ControlLabel>
                        <FormControl
                            disabled={this.props.requestSent}
                            placeholder='Short name'
                            value={this.props.foodtruck.name}
                            onChange={e => this.props.changeName(e.target.value)}
                        />
                    </FormGroup>
                    <FormGroup controlId='displayName'>
                        <ControlLabel>Long name</ControlLabel>
                        <FormControl
                            disabled={this.props.requestSent}
                            placeholder='Long name'
                            value={this.props.foodtruck.displayName}
                            onChange={e => this.props.changeDisplayName(e.target.value)}
                        />
                    </FormGroup>
                    <FormGroup controlId='shouldHasDefaultLocation'>
                        <Checkbox
                            disabled={this.props.requestSent}
                            checked={this.props.shouldHasDefaultLocation}
                            onChange={e => this.props.changeShouldHasDefaultLocation(e.target.checked)}
                        >
                            Default location
                        </Checkbox>
                    </FormGroup>
                    <LocationPicker
                        disabled={!this.props.shouldHasDefaultLocation || this.props.requestSent}
                        mapId='foodtruck-form-map'
                        selection={this.props.foodtruck.defaultLocation}
                        onSelectionChanged={this.props.changeLocation}
                    />
                    <FormGroup className='foodtruck-form-submit-form-group'>
                        <Button
                            type='submit'
                            className='foodtruck-form-submit'
                        >
                            Save
                        </Button>
                    </FormGroup>
                </Form>
            </Modal.Body>
        </Modal>;
    }
}

export default connect(
    state => state.foodtruckForm,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(FoodtruckForm);
