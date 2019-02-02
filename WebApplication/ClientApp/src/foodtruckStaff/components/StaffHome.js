import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Nav, FormControl, InputGroup} from 'react-bootstrap';
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
                    onChange={e=>this.props.changeQuery(e.target.value)}
                    value={this.props.query}
                />
            </InputGroup>
        </div>;
    }
}

export default connect(
    state => state.staffHome,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(StaffHome);