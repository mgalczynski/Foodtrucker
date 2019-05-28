import React, {Component} from 'react';
import {connect} from 'react-redux';
import {dateFormat, timeFormat} from '../../components/Helpers';
import * as Datetime from 'react-datetime';
import 'react-datetime/css/react-datetime.css';
import './PresenceOrUnavailabilityForm.css';
import {Modal, Grid, Row, Col, Button, Form, FormGroup, FormControl, ControlLabel, Checkbox} from 'react-bootstrap';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/PresenceOrUnavailabilityForm';
import Select from 'react-select';
import {REPORTER} from '../Permisions';
import CloseButton from '../../components/CloseButton';
import LocationPicker from './LocationPicker';

class PresenceForm extends Component {
    onSubmit = (e) => {
        e.preventDefault();
        this.props.save();
    };

    render() {
        return <Modal show={this.props.isOpen} dialogClassName='presence-form' bsSize='large'>
            <Modal.Header>
                <CloseButton onClick={this.props.close}/>
                {this.props.id === null ? 'Add new presence' : `Modify ${this.props.presence.title}`}
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={this.onSubmit}>
                    <FormGroup controlId='title'>
                        <ControlLabel>Short name</ControlLabel>
                        <FormControl
                            disabled={this.props.requestSent}
                            placeholder='Title'
                            value={this.props.presence.title}
                            onChange={e => this.props.changeTitle(e.target.value)}
                        />
                    </FormGroup>
                    <FormGroup controlId='description'>
                        <ControlLabel>Long name</ControlLabel>
                        <FormControl
                            disabled={this.props.requestSent}
                            placeholder='Description'
                            value={this.props.presence.description}
                            onChange={e => this.props.changeDescription(e.target.value)}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Datetime
                            value={this.props.presence.startTime}
                            onChange={this.props.changeStartTime}
                            dateFormat={dateFormat}
                            timeFormat={timeFormat}
                            inputProps={{
                                id: 'startTime'
                            }}
                        />
                    </FormGroup>
                    <FormGroup controlId='shouldHasEndTime'>
                        <Checkbox
                            disabled={this.props.requestSent}
                            checked={this.props.shouldHasEndTime}
                            onChange={e => this.props.changeShouldHasEndTime(e.target.checked)}
                        >
                            Should has end time
                        </Checkbox>
                    </FormGroup>
                    <FormGroup>
                        <Datetime
                            value={this.props.presence.endTime}
                            onChange={this.props.changeEndTime}
                            dateFormat={dateFormat}
                            timeFormat={timeFormat}
                            inputProps={{
                                disabled: !this.props.shouldHasEndTime,
                                id: 'endTime'
                            }}
                        />
                    </FormGroup>
                    <LocationPicker
                        disabled={this.props.requestSent}
                        position={this.props.position}
                        mapId='presence-form-map'
                        selection={this.props.presence.location}
                        onSelectionChanged={this.props.changeLocation}
                    />
                    <FormGroup className='presence-form-submit-form-group'>
                        <Button
                            type='submit'
                            className='presence-form-submit'
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
    (state, ownProps) => ({...state.presenceForm, position: ownProps.position || null}),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(PresenceForm);
