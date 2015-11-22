using System;

namespace AgarIoGamepadLib
{
    public interface IGameInputBridge
    {
        void setMovement(Tuple<double,double> directionAndMag);
        void split();
        void eject();
    }
}

