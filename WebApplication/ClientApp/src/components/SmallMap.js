import React, { Component } from 'react';
import L from 'leaflet';

export default class SmallMap extends Component {
    constructor(props) {
        super(props);
        this.userMarker = null;
        this.marker = null;
    }
    componentDidMount = () => {
        this.map = L.map(this.props.mapId, {
            center: [this.props.latitude, this.props.longitude],
            zoom: 15,
            layers: [
                L.tileLayer('//{s}.tile.osm.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="//osm.org/copyright">OpenStreetMap</a> contributors',
                    detectRetina: true
                })
            ],
            zoomControl: false,
            dragging: false,
            touchZoom: false,
            doubleClickZoom: false,
            scrollWheelZoom: false,
            boxZoom: false,
            keyboard: false
        });
        if (this.map.tap)
            this.map.tap.disable();
        this.marker = L.marker([this.props.latitude, this.props.longitude]);
        this.map.addLayer(this.marker);
        document.getElementById(this.props.mapId).style.cursor = 'default';
        window.addEventListener('resize', this.containerSizeChanged);
        this.updatePosition(this.props.position);
    };
    containerSizeChanged = () => {
        this.map.invalidateSize(false);
    };
    componentWillReceiveProps = (props) => {
        this.updatePosition(props.position);
        this.marker.setLatLng([props.latitude, props.longitude]);
    };
    updatePosition = (position) => {
        if (position === null)
            return;
        if (this.userMarker === null) {
            this.userMarker = L.marker(
                [position.latitude, position.longitude],
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
            this.userMarker.setLatLng([position.latitude, position.longitude]);
    };
    render() {
        return (
            <div id={this.props.mapId} className='map' />
        );
    }
}
