var flyEnabled = false;
var currentPos;
var forward;
var backward;
var left;
var right;
var down;
var up;
var shift;
var maxSpeed;

API.onUpdate.connect(function(){    
    if (flyEnabled)
    {
        var player = API.getLocalPlayer();
        var multiplier = 1;
        if (shift)
            multiplier = 3;
       
        if (maxSpeed)
            multiplier = 10;
       
        var camRotation = API.getGameplayCamRot();
        var camDirection = API.getGameplayCamDir();
       
        if (forward){
            currentPos.X = currentPos.X + camDirection.X*multiplier;
            currentPos.Y = currentPos.Y + camDirection.Y*multiplier;
        }
       
        if (backward){
            currentPos.X = currentPos.X - camDirection.X*multiplier;
            currentPos.Y = currentPos.Y - camDirection.Y*multiplier;
        }
       
        //BUG
        if (left){
            currentPos.X = currentPos.X + camDirection.X;
            currentPos.Y = currentPos.Y - camDirection.Y;
        }
       
        //BUG
        if (right){
            currentPos.X = currentPos.X - camDirection.X;
            currentPos.Y = currentPos.Y + camDirection.Y;
        }
       
        if (down){
            currentPos.Z = currentPos.Z - 1*multiplier;
        }
       
        if (up){
            currentPos.Z = currentPos.Z + 1*multiplier;
        }
       
        var newRotation = new Vector3(0, 0, camRotation.Z-180);
        API.setEntityRotation(player, newRotation);
        API.setEntityPosition(player, currentPos);
    }
});

API.onChatCommand.connect(function(msg) {
    if (msg == "/fly") {
        if (flyEnabled) {
            flyEnabled = false;
            API.sendChatMessage("[Fly] ~r~Disabled");
        } else {
            var player = API.getLocalPlayer();
            currentPos = API.getEntityPosition(player);
            flyEnabled = true;
            API.sendChatMessage("[Fly] ~g~Enabled");
        }
    }
});

API.onKeyUp.connect(function (sender, key) {
    if (flyEnabled) {
        if (key.KeyCode === Keys.W) {
            forward = false;
        }else if (key.KeyCode === Keys.D) {
            right = false;
        }else if (key.KeyCode === Keys.A) {
            left = false;
        }else if (key.KeyCode === Keys.S) {
            backward = false;
        }else if (key.KeyCode === Keys.ControlKey) {
            down = false;
        }else if (key.KeyCode === Keys.ShiftKey) {
            shift = false;
        }else if (key.KeyCode === Keys.Space) {
            up = false;
        }else if (key.KeyCode === Keys.E) {
            maxSpeed = false;
        }
    }
});

API.onKeyDown.connect(function (sender, key) {
    if (flyEnabled) {
        if (key.KeyCode === Keys.W) {
            forward = true;
        }else if (key.KeyCode === Keys.D) {
            right = true;
        }else if (key.KeyCode === Keys.A) {
            left = true;
        }else if (key.KeyCode === Keys.S) {
            backward = true;
        }else if (key.KeyCode === Keys.ControlKey) {
            down = true;
        }else if (key.KeyCode === Keys.ShiftKey) {
            shift = true;
        }else if (key.KeyCode === Keys.Space) {
            up = true;
        }else if (key.KeyCode === Keys.E) {
            maxSpeed = true;
        }
    }
});