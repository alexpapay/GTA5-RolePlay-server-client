API.onKeyDown.connect(function (sender, args) {

    if (args.KeyCode == Keys.F1) {
        API.triggerServerEvent("onKeyDown", 0);
    }
    else if (args.KeyCode == Keys.F2) {

        if (API.isCursorShown()) {
            API.showCursor(false)
        }
        else API.showCursor(true);
    }
    else if (args.KeyCode == Keys.N) {
        API.triggerServerEvent("onKeyDown", 2);
    }
    else if (args.KeyCode == Keys.Q) {
        API.triggerServerEvent("onKeyDown", 3);
    }
    else if (args.KeyCode == Keys.Y) {
        API.triggerServerEvent("onKeyDown", 4);
    }
    else if (args.KeyCode == Keys.I) {
        API.triggerServerEvent("onKeyDown", 5);
    }
    else if (args.KeyCode == Keys.K) {
        API.triggerServerEvent("onKeyDown", 6);
    }
    else if (args.KeyCode == Keys.L) {
        API.triggerServerEvent("onKeyDown", 7);
    }    
    else if (args.KeyCode == Keys.E) {
        if (resource.hud.drawAnimationHUD) {
            API.triggerServerEvent("onKeyDown", 8);
        }
    }
    else if (args.KeyCode == Keys.D1) {
        API.triggerServerEvent("onKeyDown", 9);
    }
    else if (args.KeyCode == Keys.D2) {
        API.triggerServerEvent("onKeyDown", 10);
    }
    else if (args.KeyCode == Keys.F) {
        API.triggerServerEvent("onKeyDown", 11);
    }
    else if (args.KeyCode == Keys.D3) {
        API.triggerServerEvent("onKeyDown", 12);
    }
    else if (args.KeyCode == Keys.D5) {
        API.triggerServerEvent("onKeyDown", 13);
    }
    else if (args.KeyCode == Keys.D6) {
        API.triggerServerEvent("onKeyDown", 14);
    }
    else if (args.KeyCode == Keys.D4) {
        API.createMarker(0, new Vector3(-1020.5, -2722.14, 13.8), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 150, 255, 0, 50);
        API.createMarker(0, new Vector3(-1035.88, -2735.7, 13.8), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 150, 255, 0, 50);
    }
    
});