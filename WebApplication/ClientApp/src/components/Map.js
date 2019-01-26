import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import './Map.css';
import 'leaflet/dist/leaflet.css';
import 'leaflet.markercluster/dist/MarkerCluster.Default.css';
import 'leaflet-easybutton/src/easy-button.css'
import { FormGroup, ControlLabel, FormControl, Checkbox, Button, Alert } from 'react-bootstrap';
import { actionCreators } from '../store/Map';
import L from 'leaflet';
import MarkerClusterGroup from 'leaflet.markercluster';
import EasyButton from 'leaflet-easybutton';
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
        this.foodtruckMarkers = new Map();
        this.presenceMarkers = new Map();
        this.userMarker = null;
    }

    containerSizeChanged = () => {
        this.map.invalidateSize(false);
        this.mapLongitude = 0;
        this.mapLatitidue = 0;
        this.mapZoom = 0;
    };
    componentDidMount = () => {
        this.map = L.map('map', {
            center: [this.props.latitude, this.props.longitude],
            zoom: 13,
            layers: [
                L.tileLayer('//{s}.tile.osm.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="//osm.org/copyright">OpenStreetMap</a> contributors',
                    detectRetina: true
                })
            ]
        });
        if (this.props.disabled)
            this.disableMap();
        window.addEventListener('resize', this.containerSizeChanged);
        this.handleLoad(this.map);
        this.props.watchPosition();
        this.markers = L.markerClusterGroup({
            showCoverageOnHover: false,
            removeOutsideVisibleBounds: false
        });
        L.easyButton('<span class="target map-location-icon">&target;</span>', this.props.goToLocationExtended).addTo(this.map);
        this.map.addLayer(this.markers);
        const center = this.map.getCenter();
        this.mapLongitude = center.lng;
        this.mapLatitidue = center.lat;
        this.mapZoom = this.map.getZoom();
        this.props.boundsChanged(this.map.getBounds());
    };
    disableMap = () => {
        this.map.dragging.disable();
        this.map.touchZoom.disable();
        this.map.doubleClickZoom.disable();
        this.map.scrollWheelZoom.disable();
        this.map.boxZoom.disable();
        this.map.keyboard.disable();
        if (this.map.tap)
            this.map.tap.disable();
        document.getElementById('map').style.cursor = 'default';
    };
    enableMap = () => {
        this.map.dragging.enable();
        this.map.touchZoom.enable();
        this.map.doubleClickZoom.enable();
        this.map.scrollWheelZoom.enable();
        this.map.boxZoom.enable();
        this.map.keyboard.enable();
        if (this.map.tap)
            this.map.tap.enable();
    };
    componentDidUpdate = (prevProps, prevState, snapshot) => {
        if (prevProps.disabled && !this.props.disabled)
            this.enableMap();
        else if (this.props.disabled && !prevProps.disabled)
            this.disableMap();
        let changed = false;
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

        if (this.props.position !== null) {
            if (this.userMarker == null) {
                this.userMarker = L.marker(
                    [this.props.position.latitude, this.props.position.longitude],
                    {
                        icon: L.icon({
                            iconUrl: 'icons/location.svg',

                            iconSize: [24, 24], // size of the icon
                            iconAnchor: [12, 12] // point of the icon which will correspond to marker's location
                        })
                    }
                );
                this.map.addLayer(this.userMarker);
            } else {
                this.userMarker.setLatLng([this.props.position.latitude, this.props.position.longitude]);
            }
        }

        this.updateFoodtrucks();
        this.updatePresences();
    };

    updateFoodtrucks = () => {
        const allNewIds = new Set(this.props.foodtrucks.map(f => f.id));
        const newMarkers = new Map(this.props.foodtrucks
            .filter(f => !this.foodtruckMarkers.has(f.id) && f.defaultLocation)
            .map(f => [f.id, L.marker([f.defaultLocation.latitude, f.defaultLocation.longitude]).bindPopup(f.displayName)]));
        this.markers.addLayers(Array.from(newMarkers.values()));
        const markersToRemove = [];
        this.foodtruckMarkers.forEach((marker, id) => {
            if (allNewIds.has(id))
                newMarkers.set(id, marker);
            else
                markersToRemove.push(marker);
        });
        this.markers.removeLayers(markersToRemove);
        this.foodtruckMarkers = newMarkers;
    };

    updatePresences = () => {
        const allNewIds = new Set(this.props.presences.map(f => f.id));
        const newMarkers = new Map(this.props.presences
            .filter(f => !this.presenceMarkers.has(f.id))
            .map(f => [f.id, L.marker([f.location.latitude, f.location.longitude]).bindPopup(f.title)]));
        this.markers.addLayers(Array.from(newMarkers.values()));
        const markersToRemove = [];
        this.presenceMarkers.forEach((marker, id) => {
            if (allNewIds.has(id))
                newMarkers.set(id, marker);
            else
                markersToRemove.push(marker);
        });
        this.markers.removeLayers(markersToRemove);
        this.presenceMarkers = newMarkers;
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
            <div className={`map-container${this.props.disabled ? ' disabled' : ''}`}>
                <div id='map' />
            </div>
        );
    }
}

export default connect(
    state => state.map,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(MapComponent);
