import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import './Map.css';
import 'leaflet/dist/leaflet.css';
import 'leaflet.markercluster/dist/MarkerCluster.Default.css';
import 'leaflet-easybutton/src/easy-button.css';
import { Route, Router, Link } from 'react-router-dom';
import { actionCreators } from '../store/Map';
import L from 'leaflet';
import MarkerClusterGroup from 'leaflet.markercluster';
import EasyButton from 'leaflet-easybutton';
import iconRetinaUrl from 'leaflet/dist/images/marker-icon-2x.png';
import iconUrl from 'leaflet/dist/images/marker-icon.png';
import shadowUrl from 'leaflet/dist/images/marker-shadow.png';
import Foodtruck from './Foodtruck';
import ErrorBoundary from './ErrorBoundary';
import Login from './Login';
import Layout from '../App';


delete L.Icon.Default.prototype._getIconUrl;

L.Icon.Default.mergeOptions({
    iconRetinaUrl: iconRetinaUrl,
    iconUrl: iconUrl,
    shadowUrl: shadowUrl
});

class ParentRouter extends Component {
    getChildContext() {
        return this.props.context;
    }

    render() {
        return this.props.children;
    }
}

ParentRouter.childContextTypes = Link.contextTypes;

class MapComponent extends Component {
    constructor(props) {
        super(props);
        this.foodtruckMarkers = new Map();
        this.presenceOrUnavailabilityMarkers = new Map();
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
                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
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
        this.updateFoodtrucks(this.props);
        this.updatePresencesOrUnavailabilities(this.props);
    };
    disableMap = () => {
        this.map.zoomControl.disable();
        this.map.dragging.disable();
        this.map.touchZoom.disable();
        this.map.doubleClickZoom.disable();
        this.map.scrollWheelZoom.disable();
        this.map.boxZoom.disable();
        this.map.keyboard.disable();
        if (this.map.tap)
            this.map.tap.disable();
    };
    enableMap = () => {
        this.map.zoomControl.enable();
        this.map.dragging.enable();
        this.map.touchZoom.enable();
        this.map.doubleClickZoom.enable();
        this.map.scrollWheelZoom.enable();
        this.map.boxZoom.enable();
        this.map.keyboard.enable();
        if (this.map.tap)
            this.map.tap.enable();
    };
    componentWillReceiveProps = (props) => {
        if (this.props.disabled && !props.disabled)
            this.enableMap();
        else if (props.disabled && !this.props.disabled)
            this.disableMap();
        let changed = false;
        if (props.latitude !== this.mapLatitidue || props.longitude !== this.mapLongitude) {
            this.mapLongitude = props.longitude;
            this.mapLatitidue = props.latitude;
            changed = true;
        }
        if (props.zoom !== this.mapZoom) {
            this.mapZoom = props.zoom;
            changed = true;
        }
        if (changed)
            this.map.flyTo([this.mapLatitidue, this.mapLongitude], this.mapZoom);

        if (props.position !== null) {
            if (this.userMarker == null) {
                this.userMarker = L.marker(
                    [props.position.latitude, props.position.longitude],
                    {
                        icon: L.icon({
                            iconUrl: 'icons/location.svg',
                            iconSize: [24, 24], // size of the icon
                            iconAnchor: [12, 12] // point of the icon which will correspond to marker's location
                        })
                    }
                );
                this.map.addLayer(this.userMarker);
            } else
                this.userMarker.setLatLng([props.position.latitude, props.position.longitude]);
        }

        this.updateFoodtrucks(props);
        this.updatePresencesOrUnavailabilities(props);
    };

    updateFoodtrucks = (props) => {
        const allNewSlugs = new Set(props.foodtrucks.map(f => f.slug));
        const newMarkers = new Map(props.foodtrucks
            .filter(f => !this.foodtruckMarkers.has(f.slug) && f.defaultLocation)
            .map(f => {
                const div = document.createElement('div');
                const pair = [f.slug, L.marker([f.defaultLocation.latitude, f.defaultLocation.longitude])
                    .bindPopup(div)];
                ReactDOM.render(
                    <ErrorBoundary>
                        <ParentRouter context={this.context}>
                            <Link to={`/foodtruck/${f.slug}`}>{f.displayName}</Link>
                        </ParentRouter>
                    </ErrorBoundary>, div);
                return pair;
            }));
        this.markers.addLayers(Array.from(newMarkers.values()));
        const markersToRemove = [];
        this.foodtruckMarkers.forEach((marker, slug) => {
            if (allNewSlugs.has(slug))
                newMarkers.set(slug, marker);
            else
                markersToRemove.push(marker);
        });
        this.markers.removeLayers(markersToRemove);
        this.foodtruckMarkers = newMarkers;
    };

    updatePresencesOrUnavailabilities = (props) => {
        const allNewIds = new Set(props.presencesOrUnavailabilities.map(p => p.id));
        const newMarkers = new Map(props.presencesOrUnavailabilities
            .filter(p => !this.presenceOrUnavailabilityMarkers.has(p.id))
            .map(p => {
                const div = document.createElement('div');
                const pair = [p.id, L.marker([p.location.latitude, p.location.longitude])
                    .bindPopup(div)];
                ReactDOM.render(
                    <ErrorBoundary>
                        <ParentRouter context={this.context}>
                            <Link to={`/foodtruck/${p.foodtruckSlug}/${p.id}`}>{p.title}</Link>
                        </ParentRouter>
                    </ErrorBoundary>, div);
                return pair;
            }));
        this.markers.addLayers(Array.from(newMarkers.values()));
        const markersToRemove = [];
        this.presenceOrUnavailabilityMarkers.forEach((marker, id) => {
            if (allNewIds.has(id))
                newMarkers.set(id, marker);
            else
                markersToRemove.push(marker);
        });
        this.markers.removeLayers(markersToRemove);
        this.presenceOrUnavailabilityMarkers = newMarkers;
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
                <div id='map' className='map' />
                <Route path='/foodtruck/:foodtruckSlug/:presenceOrUnavailabilityId?' component={Foodtruck} />
            </div>
        );
    }
}

MapComponent.contextTypes = Link.contextTypes;

export default connect(
    state => state.map,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(MapComponent);
