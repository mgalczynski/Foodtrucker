import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Glyphicon, Table, FormControl, InputGroup, Modal} from 'react-bootstrap';
import {staffPrefix} from '../../Helpers';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {actionCreators} from '../store/Foodtruck';
import Loader from 'react-loader';
import SmallMap from '../../components/SmallMap';
import './Foodtruck.css'

class Foodtruck extends Component {
    componentDidMount = () => {
        this.props.loadFoodtruck(this.props.match.params.foodtruckSlug);
        this.props.watchPosition();
    };
    componentWillReceiveProps = (props) => {
        this.props.loadFoodtruck(props.match.params.foodtruckSlug);
    };
    componentWillUnmount = () => {
        this.props.clear();
    };
    render() {
        return this.props.foodtruck === null?
            <Loader loaded={false} lines={13} length={20} width={10} radius={30}
                    corners={1} rotate={0} direction={1} color='#000' speed={1}
                    trail={60} shadow={false} hwaccel={false}
                    zIndex={2e9} top='50%' left='50%' scale={1.00} />
                    :
            <div className='foodtruck'>
                <h1>{this.props.foodtruck.name}</h1>
                <h3>{this.props.foodtruck.displayName}</h3>
                {this.props.foodtruck.defaultLocation !==null&&
                <SmallMap
                    mapId={'foodtruck-map'}
                    latitude={this.props.foodtruck.defaultLocation.latitude}
                    longitude={this.props.foodtruck.defaultLocation.longitude}
                    position={this.props.position}
                />}
            </div>
        ;
    }
}

export default connect(
    state => state.foodtruckForStaff,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Foodtruck);