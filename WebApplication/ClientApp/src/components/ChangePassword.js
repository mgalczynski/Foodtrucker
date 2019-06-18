import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { FormGroup, ControlLabel, FormControl, Checkbox, Button, Alert } from 'react-bootstrap';
import { actionCreators } from '../store/ChangePassword';

class ChangePassword extends Component {
    submit = event => {
        event.preventDefault();
        this.props.submit(this.props.staff);
    };

    render() {
        return (
            <div>
                <form onSubmit={this.submit}>
                    {this.props.failed && <Alert bsStyle='danger'>Try again</Alert>}
                    <FormGroup
                        controlId='current-password'
                    >
                        <ControlLabel>Current password</ControlLabel>
                        <FormControl
                            disabled={this.props.ongoingRequest}
                            autoComplete='current-password'
                            type='password'
                            value={this.props.currentPassword}
                            placeholder='Current password'
                            onChange={e => this.props.currentPasswordChanged(e.target.value)}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup
                        controlId='new-password'
                    >
                        <ControlLabel>New password</ControlLabel>
                        <FormControl
                            disabled={this.props.ongoingRequest}
                            autoComplete='new-password'
                            type='password'
                            value={this.props.newPassword}
                            placeholder='New password'
                            onChange={e => this.props.newPasswordChanged(e.target.value)}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <Button type='submit'>Change password</Button>
                </form>
            </div>
        );
    }
}

export default connect(
    (state, ownProps) => ({ ...state.changePassword, staff: ownProps.staff === undefined ? false : ownProps.staff }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(ChangePassword);
