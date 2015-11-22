using System;
using System.Collections.Generic;
using SlimDX;
using SlimDX.XInput;
namespace AgarIoGamepadLib
{
    public class Xbox360AgarIoGamepad : AgarIoGamepad
    {
        private Controller controller;
        private State lastState;
        private short lastRTX, lastRTY;
        private const string CONN_ERR_STRING = "ERROR_DEVICE_NOT_CONNECTED";

        private string gamepadName;

        private Dictionary<GamepadButtonFlags,CommandTypes> buttonMapping;

        public Xbox360AgarIoGamepad(UserIndex userIndex, Dictionary<GamepadButtonFlags,CommandTypes> buttonMappings = null)
        {
            controller = new Controller(userIndex);
            lastRTX = lastRTY = 0;
            if (buttonMapping == null)
                this.buttonMapping = Xbox360AgarIoGamepad.getDefaultButtonMap();
            else
                this.buttonMapping = buttonMappings;

            gamepadName = "Xbox 360 Controller " + ((int)userIndex + 1); 
            
        }
        public static Dictionary<GamepadButtonFlags,CommandTypes> getDefaultButtonMap()
        {
            Dictionary<GamepadButtonFlags,CommandTypes> ret = new Dictionary<GamepadButtonFlags, CommandTypes>();
            ret.Add(GamepadButtonFlags.A, CommandTypes.QUIT);
            ret.Add(GamepadButtonFlags.Y, CommandTypes.SUSPEND);
            ret.Add(GamepadButtonFlags.RightShoulder, CommandTypes.SPLIT);
            ret.Add(GamepadButtonFlags.LeftShoulder, CommandTypes.EJECT);
            return ret;
        }
        public override Tuple<double,double> lastMovementStickDirection()
        {
            Vector2 result = normalize(lastState.Gamepad.RightThumbX, lastState.Gamepad.RightThumbY, Gamepad.GamepadRightThumbDeadZone);
            if (invertYAxis)
                result.Y *= -1;
            return new Tuple<double,double>(result.X, result.Y);
        }
        public override bool poll()
        {
            try
            {
                lastState = controller.GetState();
                foreach (KeyValuePair<GamepadButtonFlags,CommandTypes> pair in buttonMapping)
                    setButtonState(pair.Value, lastState.Gamepad.Buttons.HasFlag(pair.Key));
                if(lastState.Gamepad.RightThumbX != lastRTX || lastState.Gamepad.RightThumbY != lastRTY)
                    stickHeading = lastMovementStickDirection();
                isConnected = true;
            }
            catch(XInputException e)
            {
                isConnected = false;
            }
            return isConnected;

        }
        private Vector2 normalize(short rawX, short rawY, short threshold)
        {
            var value = new Vector2(rawX, rawY);
            var magnitude = value.Length();
            var direction = value / (magnitude == 0 ? 1 : magnitude);

            var normalizedMagnitude = 0.0f;
            if (magnitude - threshold > 0)
                normalizedMagnitude = Math.Min((magnitude - threshold) / (short.MaxValue - threshold), 1);

            return direction * normalizedMagnitude;

        }
        public override bool isControllerAvailable()
        {
            bool res = poll();
            isConnected = res;
            return res;
        }
        public override string getGamepadName()
        {
            return gamepadName;
        }

    }
}

