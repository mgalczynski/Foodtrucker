import React, {Component} from 'react';
import L from 'leaflet';
import {positionWatch} from '../../Helpers';

export default class LocationPicker extends Component {
    constructor(props) {
        super(props);
        this.state = {
            wentToPositionAlready: false,
            watchId: null
        };
        this.userMarker = null;
        this.marker = null;
    }

    componentDidMount = () => {
        this.map = L.map(this.props.mapId, {
            center: [0, 0],
            zoom: 3,
            layers: [
                L.tileLayer('//{s}.tile.osm.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="//osm.org/copyright">OpenStreetMap</a> contributors',
                    detectRetina: true
                })
            ]
        });
        if (this.props.disabled)
            this.disableMap();
        this.map.on('click', this.onClick);
        if (this.props.selection !== null) {
            this.marker = L.marker([this.props.selection.latitude, this.props.selection.longitude]);
            this.map.addLayer(this.marker);
        }
        document.getElementById(this.props.mapId).style.cursor = 'default';
        window.addEventListener('resize', this.containerSizeChanged);
        positionWatch(this.updatePosition, null, watchId => this.setState({watchId}));
    };

    componentWillUnmount = () => {
        if(this.state.watchId!==null)
            navigator.geolocation.clearWatch(this.state.watchId)
    };


    onClick = (e) => {
        if (!this.props.disabled)
            this.props.onSelectionChanged({
                latitude: e.latlng.lat,
                longitude: e.latlng.lng
            });
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
    containerSizeChanged = () => {
        this.map.invalidateSize(false);
    };
    componentWillReceiveProps = (props) => {
        if (props.selection !== null) {
            if (this.marker === null) {
                this.marker = L.marker([props.selection.latitude, props.selection.longitude]);
                this.map.addLayer(this.marker);
            } else
                this.marker.setLatLng([props.selection.latitude, props.selection.longitude]);
        }
        if (this.props.disabled && !props.disabled)
            this.enableMap();
        else if (props.disabled && !this.props.disabled)
            this.disableMap();
    };
    updatePosition = (e) => {
        if (!this.state.wentToPositionAlready) {
            this.setState({wentToPositionAlready: true});
            this.map.flyTo([e.coords.latitude, e.coords.longitude], 17);
        }
        if (this.userMarker === null) {
            this.userMarker = L.marker(
                [e.coords.latitude, e.coords.longitude],
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
            this.userMarker.setLatLng([e.coords.latitude, e.coords.longitude]);
    };

    render() {
        return (
            <div id={this.props.mapId} className='map'/>
        );
    }
}

