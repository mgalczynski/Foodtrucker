import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import './Map.css';
import 'leaflet/dist/leaflet.css';
import {FormGroup, ControlLabel, FormControl, Checkbox, Button, Alert} from 'react-bootstrap';
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

    containerSizeChanged = () => {
        this.map.invalidateSize(false);
        this.mapLongitude = 0;
        this.mapLatitidue = 0;
        this.mapZoom = 0;
    };
    componentDidMount = () => {
        this.props.watchPosition();
        this.map = L.map('map', {
            center: [this.props.latitude, this.props.longitude],
            zoom: 13,
            layers: [
                L.tileLayer('//{s}.tile.osm.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="//osm.org/copyright">OpenStreetMap</a> contributors'
                })
            ]
        });
        window.addEventListener('resize', this.containerSizeChanged);
        const center = this.map.getCenter();
        this.mapLongitude = center.lng;
        this.mapLatitidue = center.lat;
        this.mapZoom = this.map.getZoom();
        this.handleLoad(this.map);
        this.props.boundsChanged(this.map.getBounds());
    };
    componentDidUpdate = (prevProps, prevState, snapshot) => {
        var changed = false;
        if (this.props.latitude !== this.mapLatitidue || this.props.longitude !== this.mapLongitude) {
            this.mapLongitude = this.props.longitude;
            this.mapLatitidue = this.props.latitude;
            changed = true;
        }
        if (this.props.zoom !== this.mapZoom) {
            this.mapZoom = this.props.zoom;
            changed = true;
        }
        if (changed)
            this.map.flyTo([this.mapLatitidue, this.mapLongitude], this.mapZoom);
    };

    componentWillUnmount = () => {
        window.removeEventListener('resize', this.containerSizeChanged);
        this.map.remove();
        this.map = null;
        this.props.clearWatch();
    };
    handleLoad = (map) => {
        map.on('zoomend', e => this.handleZoomChanged(e.target.getZoom()));
        map.on('dragend', e => this.handleLocationChanged(e.target.getCenter()));
    };

    handleZoomChanged = (zoom) => {
        this.mapZoom = zoom;
        const bounds = this.map.getBounds();
        this.props.zoomChanged(zoom, bounds);
    };

    handleLocationChanged = (latLng) => {
        this.mapLongitude = latLng.lng;
        this.mapLatitidue = latLng.lat;
        const bounds = this.map.getBounds();
        this.props.locationChanged(latLng.lng, latLng.lat, bounds);
    };

    render() {
        return (
            <div className='map-container'>
                <div id='map'/>
            </div>
        );
    }
}

export default connect(
    state => state.map,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(MapComponent);
