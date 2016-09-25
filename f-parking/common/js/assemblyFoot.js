$(function initMap() {
	var latlng1 = new google.maps.LatLng(33.5978735, 130.40891750000003);
	var latlng2 = new google.maps.LatLng(33.5978735, 130.40891750000003);

	var opts1 = {
	zoom: 15,
	center: latlng1,
	mapTypeId: google.maps.MapTypeId.ROADMAP
	};
	var opts2 = {
	zoom: 15,
	center: latlng2,
	mapTypeId: google.maps.MapTypeId.ROADMAP
	};

  var markerImg = {
	url: '/common/img/map_icon.png',
  };
  var markerImgS = {
	url: '/common/img/map_icon.png',
	scaledSize : new google.maps.Size(34, 48) //ratina
  };

  var map1 = new google.maps.Map(document.getElementById("mapCanvas"), opts1);
  var marker1 = new google.maps.Marker({
	  position: new google.maps.LatLng(33.5978735, 130.40891750000003),
    map: map1,
 	icon: markerImg
  });
  var map2 = new google.maps.Map(document.getElementById("mapCanvasS"), opts2);
  var marker2 = new google.maps.Marker({
	  position: new google.maps.LatLng(33.5978735, 130.40891750000003),
    map: map2,
 	icon: markerImgS
  });
});

// --------------------

