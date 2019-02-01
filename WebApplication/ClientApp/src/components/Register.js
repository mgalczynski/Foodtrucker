import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { FormGroup, ControlLabel, FormControl, HelpBlock, Button, Alert } from 'react-bootstrap';
import { actionCreators } from '../store/Register';

class Register extends Component {
    submit = event => {
        event.preventDefault();
        this.props.submit(this.props.staff);
    };

    render() {
        return (
            <div>
                <form onSubmit={this.submit}>
                    <FormGroup
                        controlId="formBasicText"
                    >
                        <ControlLabel>First name</ControlLabel>
                        <FormControl
                            disabled={this.props.ongoingRequest}
                            autoComplete="given-name"
                            type="text"
                            value={this.props.firstName}
                            placeholder="First name"
                            onChange={e => this.props.firstNameChanged(e.target.value)}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup
                        controlId="formBasicText"
                    >
                        <ControlLabel>Last name</ControlLabel>
                        <FormControl
                            disabled={this.props.ongoingRequest}
                            autoComplete="family-name"
                            type="text"
                            value={this.props.lastName}
                            placeholder="Last name"
                            onChange={e => this.props.lastNameChanged(e.target.value)}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup
                        controlId="formBasicText"
                    >
                        <ControlLabel>Email</ControlLabel>
                        <FormControl
                            disabled={this.props.ongoingRequest}
                            autoComplete="email"
                            type="email"
                            value={this.props.email}
                            placeholder="Email"
                            onChange={e => this.props.emailChanged(e.target.value)}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup
                        controlId="formBasicText"
                    >
                        <ControlLabel>Password</ControlLabel>
                        <FormControl
                            disabled={this.props.ongoingRequest}
                            autoComplete="password"
                            type="password"
                            value={this.props.password}
                            placeholder="Password"
                            onChange={e => this.props.passwordChanged(e.target.value)}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    {this.props.cause &&
                        this.props.cause.map(cause => <Alert bsStyle="danger" key={cause}>{cause}</Alert>)}
                    <Button type="submit">Sign up</Button>
                </form>
            </div>
        );
    }
}

export default connect(
    (state, ownProps) => ({ ...state.register, staff: ownProps.staff === undefined ? false : ownProps.staff }),
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Register);
