---
output:
  rmarkdown::pdf_document:
    fig_caption: yes        
    includes:  
      in_header: figure_placement.tex
---

# Foodtrucker - Dokumentacja powykonawcza

## Zakres applikacji wykonany podczas realizacji projektu

Podczas realizacji kursu został zrealizowany mniejszy niż zostało to przewidziane. Zrealizowane przypadki użycia można podzielić na przypadki użycia dla klientów foodtrucków i dla pracowników foodtrucków. Jest on dostępny na [foodtrucker.miroslawgalczynski.com](https://foodtrucker.miroslawgalczynski.com/)

### Zrealizowane przypadki użycia dostępnych dla klientów foodtrucków

![Diagram](Diagrams/Diagram_przejsc_klienci.jpg?raw=true "Zaplanowane przypadki użycia klientów foodtrucków")

Zrealizowane zostały wszystkie przypadki użycia z wyjątkiem zarządzania ulubionymi subskrybcjami. Dodatkowo podczas wytwarzania aplikacji okazało się konieczne roszrzenie przypadku użycia **Przeglądanie foodtrucków** do **Przeglądanie foodtrucków oraz obecności** oraz **Przeglądanie foodtrukców, będąc zalogowanych** do **Przeglądanie foodtrucków oraz obecności, będąc zalogowanym**.

![Widok po wejściu na stronę główną](Screenshots/WidokDomyslny.png?raw=true "Widok po wejściu na stronę główną")

![Widok po szczegółów obecności](Screenshots/SzczegolyObecnosci.png?raw=true "Widok po szczegółów obecności")

### Zrealizowane przypadki użycia dostępnych dla pracowników foodtrucków

![Diagram](Diagrams/Diagram_przejsc_pracownicy_foodtruckow.jpg?raw=true "Zaplanowane przypadki użycia klientów foodtrucków")

Zrealizowane zostały wszystkie przypadki użycia z wyjątkiem przypadków dotycących zarządania nieobecnościami. Nie zrealizowano natomiast w pełnym zakresie zarządzania obecnościami, obecna wersja nie waliduje zakresów czasu.

![Widok dla pracowników foodtrucków](Screenshots/WidokGlownyPracownikowFoodtruckow.png?raw=true "Widok dla pracowników foodtrucków")

![Szczegóły zarządzanego foodtrucka](Screenshots/SzczegolyFoodtrucka.png?raw=true "Szczegóły zarządzanego foodtrucka")

## Uwagi techniczne

Usługa BitBucket została zmieniona na GitHub.

Do technologi użytych podczas wytwarzania aplikacji dołączył Jenkins - otwartoźródłowa usługa ulatwiająca automatyzacje zadań. W projekcie został wykorzystany do automatyzacji testów oraz automatyzacji publikowania oprogramowania na serwerze testowym. Akcje są inicjowane po opublikowaniu commitu do repozytorium GitHub.

### Leaflet

Podczas realizacji aplikacji wynikła konieczność integracji biblioteki wspomagającej wyświetlanie Leaflet z React, konieczność ta wynikała z lepszego bilansu korzyści w porówanianiu z kosztami niż dało by to gotowe rozwiązanie. Poniżej przykładowe elementy integracji

```jsx
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
                    attribution: '&copy; <a href="//osm.org/'
                    +'copyright">OpenStreetMap</a> contributors',
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
        L.easyButton('<span class="target map-location-icon">&target;</span>',
        this.props.goToLocationExtended).addTo(this.map);
        this.map.addLayer(this.markers);
        const center = this.map.getCenter();
        this.mapLongitude = center.lng;
        this.mapLatitidue = center.lat;
        this.mapZoom = this.map.getZoom();
        this.props.boundsChanged(this.map.getBounds());
        this.updateFoodtrucks(this.props);
        this.updatePresences(this.props);
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
        if (props.latitude !== this.mapLatitidue ||
            props.longitude !== this.mapLongitude) {
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
                            iconAnchor: [12, 12]
                            /* point of the icon which
                               will correspond to marker's location */
                        })
                    }
                );
                this.map.addLayer(this.userMarker);
            } else
                this.userMarker.setLatLng([props.position.latitude,
                props.position.longitude]);
        }

        this.updateFoodtrucks(props);
        this.updatePresences(props);
    };

    updateFoodtrucks = (props) => {
        const allNewSlugs = new Set(props.foodtrucks.map(f => f.slug));
        const newMarkers = new Map(props.foodtrucks
            .filter(f => !this.foodtruckMarkers.has(f.slug) && f.defaultLocation)
            .map(f => {
                const div = document.createElement('div');
                const pair = [f.slug, L.marker([f.defaultLocation.latitude,
                 f.defaultLocation.longitude])
                    .bindPopup(div)];
                ReactDOM.render(
                    <ErrorBoundary>
                        <ParentRouter context={this.context}>
                            <Link
                                to={`/foodtruck/${f.slug}`}
                            >
                                {f.displayName}
                            </Link>
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

    updatePresences = (props) => {
        const allNewIds = new Set(props.presences.map(p => p.id));
        const newMarkers = new Map(props.presences
            .filter(p => !this.presenceMarkers.has(p.id))
            .map(p => {
                const div = document.createElement('div');
                const pair = [p.id, L.marker([p.location.latitude,
                p.location.longitude])
                    .bindPopup(div)];
                ReactDOM.render(
                    <ErrorBoundary>
                        <ParentRouter context={this.context}>
                            <Link 
                                to={`/foodtruck/${p.foodtruckSlug}/${p.id}`}
                            >
                                {p.title}
                            </Link>
                        </ParentRouter>
                    </ErrorBoundary>, div);
                return pair;
            }));
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
                <div id='map' className='map' />
                <Route
                    path='/foodtruck/:foodtruckSlug/:presenceId?'
                    component={Foodtruck}
                />
            </div>
        );
    }
}
```