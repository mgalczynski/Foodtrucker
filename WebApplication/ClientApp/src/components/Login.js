import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {Link} from 'react-router-dom';
import {FormGroup, ControlLabel, FormControl, Checkbox, Button, Alert} from 'react-bootstrap';
import {actionCreators} from '../store/Login';

class Login extends Component {
    submit = event => {
        event.preventDefault();
        this.props.submit();
    };

    render() {
        return (
            <div>
                <form onSubmit={this.submit}>
                    {this.props.failed &&  <Alert bsStyle="danger">Wrong user or password</Alert>}
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
                        <FormControl.Feedback/>
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
                        <FormControl.Feedback/>
                    </FormGroup>
                    <Button type="submit">Sign up</Button>
                    <Checkbox value={this.props.rememberMe} onChange={e=>this.props.rememberMeChanged(e.target.value)}>
                        Remember me
                    </Checkbox>
                </form>
            </div>
        );
    }
}

export default connect(
    state => state.login,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Login);
