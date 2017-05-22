var marker1 = null;
var marker2 = null;
var marker3 = null;
var marker4 = null;

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "bus_marker") {
        var posX = args[0];
        var posY = args[1];
        var posZ = args[2];
        marker1 = API.createMarker(1, new Vector3(posX, posY, posZ), new Vector3(), new Vector3(), new Vector3(3, 3, 3), 255, 0, 0, 50);
    }

    if (name == "loader_one") {
        if (marker2 != null) {
            API.deleteEntity(marker2);
            marker2 = null;
        }        
        var posX = args[0];
        var posY = args[1];
        var posZ = args[2];
        marker1 = API.createMarker(0, new Vector3(posX, posY, posZ), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 0, 0, 50);
    }

    if (name == "loader_two") {
        if (marker1 != null) {
            API.deleteEntity(marker1);
            marker1 = null;
        }  
        var posX = args[0];
        var posY = args[1];
        var posZ = args[2];
        marker2 = API.createMarker(0, new Vector3(posX, posY, posZ), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 0, 0, 50);
    }

    if (name == "bus_end") {
        if (marker1 != null) {
            API.deleteEntity(marker1);
            marker1 = null;
        }
        if (marker2 != null) {
            API.deleteEntity(marker2);
            marker2 = null;
        }
        if (marker3 != null) {
            API.deleteEntity(marker3);
            marker3 = null;
        }
        if (marker4 != null) {
            API.deleteEntity(marker4);
            marker4 = null;
        }
    }
    if (name == "loader_end")
    {
        if (marker1 != null) {
            API.deleteEntity(marker1);
            marker1 = null;
        } 
        if (marker2 != null) {
            API.deleteEntity(marker2);
            marker2 = null;
        }
    }
});