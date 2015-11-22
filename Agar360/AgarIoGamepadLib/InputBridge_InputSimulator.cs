using System;
using WindowsInput;
namespace AgarIoGamepadLib
{
    public class InputBridge_InputSimulator : IGameInputBridge
    {
        private InputSimulator inSim;
        const double SCREEN_CENTRE = 32767.5;
        public double scale{ get; set;}
        public double screenAspectRatio { get; set;}
        public InputBridge_InputSimulator(double screenAspectRatio, double scale = 100.0)
        {
            inSim = new InputSimulator();
            this.scale = scale;
            this.screenAspectRatio = screenAspectRatio;
        }
        public void split()
        {
            inSim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
        }
        public void eject()
        {
            inSim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_W);
        }
        public void setMovement(Tuple<double,double> directionAndMag)
        {
            
            double calculatedX = SCREEN_CENTRE + (directionAndMag.Item1 * scale * 100);
            double calculatedY = SCREEN_CENTRE - (directionAndMag.Item2 * scale * 100 * screenAspectRatio);
            inSim.Mouse.MoveMouseTo(calculatedX, calculatedY);
        }
    }
}

