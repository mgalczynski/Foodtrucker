import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import './Map.css';
import 'leaflet/dist/leaflet.css';
import {FormGroup, ControlLabel, FormControl, Checkbox, Button, Alert} from 'react-bootstrap';
import {Map, TileLayer} from 'react-leaflet';
import {actionCreators} from '../store/Map';

class MapComponent extends Component {
    constructor(props) {
        super(props);
        this.map = React.createRef();
    }
    containerSizeChanged = () => {
        this.map.current.leafletElement.invalidateSize(false);
    };
    componentDidMount = () => {
        this.props.moveToCurrentPosition();
        window.addEventListener('resize', this.containerSizeChanged);
        this.containerSizeChanged();
    };
    componentWillUnmount = () => {
        window.addEventListener('resize', this.containerSizeChanged);
    };

    render() {
        return (
            <div className='map-container'>
                <Map ref={this.map} center={[this.props.latitude, this.props.longitude]} zoom={this.props.zoom}>
                    <TileLayer
                        url='https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png'
                        attribution='&copy; <a href=&quot;http://osm.org/copyright&quot;>OpenStreetMap</a> contributors'
                    />
                </Map>
            </div>
        );
    }
}

export default connect(
    state => state.map,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(MapComponent);
