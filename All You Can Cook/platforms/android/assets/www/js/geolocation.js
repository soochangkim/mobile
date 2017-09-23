/**
 * Created by skim7663 on 4/9/2017.
 */


var Map;
var Infowindow;
var Latitude = undefined;
var Longitude = undefined;
var PrevPath;

function getPlacesLocation() {
    navigator.geolocation.getCurrentPosition(onPlacesSuccess, onPlacesError, {enableHighAccuracy: true});
}

function onPlacesSuccess(position) {
    Latitude = position.coords.latitude;
    Longitude = position.coords.longitude;

    getPlaces(Latitude, Longitude);
}

function getPlaces(latitude, longitude) {
    var latLong = {lat: latitude, lng: longitude};

    var mapOptions = {
        center: latLong,
        zoom: 12
    };

    Map = new google.maps.Map(document.getElementById("map"), mapOptions);

    Infowindow = new google.maps.InfoWindow();

    var service = new google.maps.places.PlacesService(Map);
    service.nearbySearch({
        location: latLong,
        radius: 3000,
        name: "Grocery"
        //type: ['grocery_or_supermarket']
    }, foundCallback);
}

function foundCallback(results, status) {
    if (status === google.maps.places.PlacesServiceStatus.OK) {
        for (var i = 0; i < results.length; i++) {
            createMarker(results[i]);
            createList(results[i]);
        }
    }
}

function createList(place) {
    var photoUrl = typeof place.photos !== 'undefined' ?
        place.photos[0].getUrl({maxWidth:100, maxHeight:100}):
        "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcT7w0RRnoFwkisLUdRSFpAf123qtGlc05LLKVAf0RBMXM4mhgl7Qp_gyBwFOQ";

    var html =
        "<li><a><img src='" + photoUrl +"'/>" +
        "<h4>" + place.name + "</h4>" +
        "<p>" + place.vicinity  + "</p></a></li>";

    $('#lstGrocery').append(html).listview("refresh").children(":last-child").on("click", function(){
        getDirection(place);
    });
}

function createMarker(place) {
    var location = place.geometry.location;

    var marker = new google.maps.Marker({
        map: Map,
        position: place.geometry.location
    });

    google.maps.event.addListener(marker, "click", function () {
        getDirection(place);
    });

    google.maps.event.addListener(marker, "mouseover", function () {
        Infowindow.setContent(place.name);
        Infowindow.open(Map, this);
    });

    google.maps.event.addListener(marker, "mouseout", function () {
        Infowindow.close();
    });
}


function onPlacesError(error) {
    console.log('code: ' + error.code + '\n' +
        'message: ' + error.message + '\n');
}

function getDirection(place) {
    var service = new google.maps.DirectionsService;
    var display = new google.maps.DirectionsRenderer;
    var latLong = {lat: Latitude, lng: Longitude};

    Map = null;
    display.setMap(null);

    var mapOptions = {
        center: latLong,
        zoom: 15
    };

    Map = new google.maps.Map(document.getElementById("map"), mapOptions);

    display.setMap(Map);
    displayRoute(service, display, place);
}

function displayRoute(service, display, place) {

    service.route({
        origin: new google.maps.LatLng(Latitude, Longitude),
        destination: place.geometry.location,
        travelMode: 'DRIVING'
    }, function (response, status) {
        if (status === 'OK') {
            PrevPath = response;
            display.setDirections(response);
        } else {
            window.alert("Directions request failed due to " + status);
        }
    });
}