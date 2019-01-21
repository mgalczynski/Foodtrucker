import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import './Map.css';
import 'leaflet/dist/leaflet.css';
import {FormGroup, ControlLabel, FormControl, Checkbox, Button, Alert} from 'react-bootstrap';
import {Map, TileLayer, Marker, Popup} from 'react-leaflet';
import {actionCreators} from '../store/Map';
import L from 'leaflet';
import iconRetinaUrl from 'leaflet/dist/images/marker-icon-2x.png';
import iconUrl from 'leaflet/dist/images/marker-icon.png';
import shadowUrl from 'leaflet/dist/images/marker-shadow.png';


delete L.Icon.Default.prototype._getIconUrl;

L.Icon.Default.mergeOptions({
    iconRetinaUrl: iconRetinaUrl,
    iconUrl: iconUrl,
    shadowUrl: shadowUrl
});

class MapComponent extends Component {
    constructor(props) {
        super(props);
        this.map = React.createRef();
    }

    containerSizeChanged = () => {
        this.map.current.leafletElement.invalidateSize(false);
    };
    componentDidMount = () => {
        this.props.watchPosition();
        window.addEventListener('resize', this.containerSizeChanged);
        this.containerSizeChanged();
    };
    componentWillUnmount = () => {
        window.removeEventListener('resize', this.containerSizeChanged);
        this.props.clearWatch();
    };

    render() {
        return (
            <div className='map-container'>
                <Map ref={this.map} center={[this.props.latitude, this.props.longitude]} zoom={this.props.zoom}>
                    <TileLayer
                        url='https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png'
                        attribution='&copy; <a href=&quot;http://osm.org/copyright&quot;>OpenStreetMap</a> contributors'
                    />
                    {this.props.position &&
                    <Marker position={[this.props.position.latitude, this.props.position.longitude]}>
                        <Popup>Your position</Popup>
                    </Marker>}
                </Map>
            </div>
        );
    }
}

export default connect(
    state => state.map,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(MapComponent);
