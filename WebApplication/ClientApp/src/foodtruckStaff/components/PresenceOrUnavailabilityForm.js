import classNames from 'classnames';
import React, {Component} from 'react';
import {connect} from 'react-redux';
import {dateFormat, timeFormat} from '../../components/Helpers';
import * as Datetime from 'react-datetime';
import 'react-datetime/css/react-datetime.css';
import './PresenceOrUnavailabilityForm.css';
import {
    Modal,
    Grid,
    Row,
    Col,
    Button,
    Form,
    FormGroup,
    FormControl,
    ControlLabel,
    Checkbox,
    Radio
} from 'react-bootstrap';
import {prefix} from 'react-bootstrap/es/utils/bootstrapUtils';
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
        const radioClassNames = classNames(null, {[prefix({bsClass: 'radio'}, 'inline')]: true});
        return this.props.isOpen ? <Modal show dialogClassName='presence-or-unavailability-form' bsSize='large'>
            <Modal.Header>
                <CloseButton onClick={this.props.close}/>
                {this.props.id === null ? 'Add new presence' : `Modify ${this.props.presenceOrUnavailability.title}`}
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={this.onSubmit}>
                    <FormGroup controlId='isUnavailability'>
                        <Radio inline title='Unavailability' checked={this.props.isUnavailability} onChange={e=>this.props.isUnavailabilityChanged(e.target.value)}>
                            Unavailability
                        </Radio>
                        <Radio inline title='Unavailability' checked={!this.props.isUnavailability} onChange={e=>this.props.isUnavailabilityChanged(!e.target.value)}>
                            Presence
                        </Radio>
                    </FormGroup>
                    <FormGroup controlId='title'>
                        <ControlLabel>Short name</ControlLabel>
                        <FormControl
                            disabled={this.props.requestSent}
                            placeholder='Title'
                            value={this.props.presenceOrUnavailability.title}
                            onChange={e => this.props.changeTitle(e.target.value)}
                        />
                    </FormGroup>
                    <FormGroup controlId='description'>
                        <ControlLabel>Long name</ControlLabel>
                        <FormControl
                            disabled={this.props.requestSent}
                            placeholder='Description'
                            value={this.props.presenceOrUnavailability.description}
                            onChange={e => this.props.changeDescription(e.target.value)}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Datetime
                            value={this.props.presenceOrUnavailability.startTime}
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
                            value={this.props.presenceOrUnavailability.endTime}
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
                        disabled={this.props.requestSent || this.props.isUnavailability}
                        position={this.props.position}
                        mapId='presence-form-map'
                        selection={this.props.presenceOrUnavailability.location}
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
        </Modal> : null;
    }
}

export default connect(
    (state, ownProps) => ({...state.presenceOrUnavailabilityForm, position: ownProps.position || null}),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(PresenceForm);
