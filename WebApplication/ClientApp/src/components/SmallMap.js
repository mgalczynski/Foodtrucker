import React, { Component } from 'react';
import L from 'leaflet';

export default class SmallMap extends Component {
    componentDidMount = () => {
        this.map = L.map('small-map', {
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
        document.getElementById('small-map').style.cursor = 'default';
        window.addEventListener('resize', this.containerSizeChanged);
    };
    containerSizeChanged = () => {
        this.map.invalidateSize(false);
    }
    render() {
        return (
            <div id='small-map' className='map' />
        );
    }
}
