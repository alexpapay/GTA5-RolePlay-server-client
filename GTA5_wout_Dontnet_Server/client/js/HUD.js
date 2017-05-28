var drawVehicleHUD = false;
var drawAnimationHUD = false;
var currentMoney = null;
var bankMoney = null;
var currentFuel = null;
var res = API.getScreenResolutionMantainRatio();

var res_X = API.getScreenResolutionMantainRatio().Width;
var res_Y = API.getScreenResolutionMantainRatio().Height;

API.onUpdate.connect(function (sender, args) {

    if (drawAnimationHUD) {
        API.drawText("Press E to stop", res.Width - 30, res.Height - 100, 0.5, 255, 186, 131, 255, 4, 2, false, true, 0);
    }

    if (currentMoney != null) {
        API.drawText("$" + currentMoney, res.Width - 15, 50, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
    }

    if (bankMoney != null) {
        API.drawText("$" + bankMoney, res.Width - 15, 20, 0.5, 115, 115, 0, 255, 4, 2, false, true, 0);
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "update_money_display") {
        currentMoney = Number(args[0]).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    }
    if (eventName === "update_bank_money_display") {
        bankMoney = Number(args[0]).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    }
    if (eventName == "animation_text") {
        drawAnimationHUD = false;
    }
    if (eventName == "update_fuel_display") {
        currentFuel = parseFloat(args[0]).toFixed(2);
    }
});

API.onUpdate.connect(function () {
    var player = API.getLocalPlayer();
    var inVeh = API.isPlayerInAnyVehicle(player);

    if (inVeh) {        
        var veh = API.getPlayerVehicle(player);
        var vel = API.getEntityVelocity(veh);
        var health = API.getVehicleHealth(veh);
        var maxhealth = API.returnNative("GET_ENTITY_MAX_HEALTH", 0, veh);
        var healthpercent = Math.floor((health / maxhealth) * 100);
        var speed = Math.sqrt(
            vel.X * vel.X +
            vel.Y * vel.Y +
            vel.Z * vel.Z
        );
        var displaySpeedkmph = Math.round(speed * 3.6); 

        var engineStatus = API.getVehicleEngineStatus(veh);
        if (engineStatus == true) {
            var vehRpm = parseFloat(API.getVehicleRPM(veh)).toFixed(2);
            API.triggerServerEvent("fuel_consumption", vehRpm);
        }        

        API.triggerServerEvent("ask_fuel_in_car");
        API.drawText(`LITERS:`, res_X - 20, res_Y - 75, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
        API.drawText(`${currentFuel}`, res_X - 100, res_Y - 100, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
        if (currentFuel < 15) API.drawText(`${currentFuel}`, res_X - 100, res_Y - 100, 1, 219, 122, 46, 255, 4, 2, false, true, 0);
        if (currentFuel < 5)  API.drawText(`${currentFuel}`, res_X - 100, res_Y - 100, 1, 219, 46, 46, 255, 4, 2, false, true, 0);
                
        API.drawText(`KMPH`, res_X - 20, res_Y - 175, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
        API.drawText(`${displaySpeedkmph}`, res_X - 100, res_Y - 200, 1, 255, 255, 255, 255, 4, 2, false, true, 0);

        API.drawText(`HEALTH:`, res_X - 70, res_Y - 225, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
        API.drawText(`${healthpercent}%`, res_X - 20, res_Y - 225, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
        if (healthpercent < 60) {
            API.drawText(`${healthpercent}%`, res_X - 20, res_Y - 225, 0.5, 219, 122, 46, 255, 4, 2, false, true, 0);
        }
        if (healthpercent < 30) {
            API.drawText(`${healthpercent}%`, res_X - 20, res_Y - 225, 0.5, 219, 46, 46, 255, 4, 2, false, true, 0);
        }
    }
});