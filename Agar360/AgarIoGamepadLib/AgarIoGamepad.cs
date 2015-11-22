using System;
using System.Threading.Tasks;
using System.Threading;
namespace AgarIoGamepadLib
{
    public delegate void ButtonPressedEventHandler(object sender, CommandTypes button);
    public delegate void ButtonReleasedEventHandler(object sender, CommandTypes button);
    public delegate void NewHeadingEventHandler(object sender, Tuple<double,double> heading);
    public delegate void GamepadConnectivityChangedHandler(object sender, bool isConnectedNow);

    public abstract class AgarIoGamepad
    {
        
        private bool shouldPoll;

        private bool _isSplitPressed;
        private bool _isEjectPressed;
        private bool _isQuitPressed;
        private bool _isSuspendControlPressed;
        private bool _isConnected;

        private Tuple<double,double> _stickHeading;


        protected Tuple<double,double> stickHeading
        {
            get
            {
                return _stickHeading;
            }
            set
            {
                _stickHeading = value;
                sendNewHeading(value);
            }
        }
        protected bool isConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (value != _isConnected)
                {
                    _isConnected = value;
                    sendGamepadConnectivityChanged(value);
                }
            }

        }
        protected bool isSplitPressed
        {
            get
            {
                return _isSplitPressed;
            }
            set
            {
                if (value != _isSplitPressed)
                {
                    _isSplitPressed = value;
                    if (value)
                        sendButtonPressed(CommandTypes.SPLIT);
                    else
                        sendButtonReleased(CommandTypes.SPLIT);
                }
            }
                    
        }
        protected bool isEjectPressed
        {
            get
            {
                return _isEjectPressed;
            }
            set
            {
                if (value != _isEjectPressed)
                {
                    _isEjectPressed = value;
                    if (value)
                        sendButtonPressed(CommandTypes.EJECT);
                    else
                        sendButtonReleased(CommandTypes.EJECT);
                }
            }
        }
        protected bool isQuitPressed
        {
            get
            {
                return _isQuitPressed;
            }
            set
            {
                if (value != _isQuitPressed)
                {
                    _isQuitPressed = value;
                    if (value)
                        sendButtonPressed(CommandTypes.QUIT);
                    else
                        sendButtonReleased(CommandTypes.QUIT);
                }
            }
        }
        protected bool isSuspendControlPressed
        {
            get
            {
                return _isSuspendControlPressed;
            }
            set
            {
                if (value != _isSuspendControlPressed)
                {
                    _isSuspendControlPressed = value;
                    if (value)
                        sendButtonPressed(CommandTypes.SUSPEND);
                    else
                        sendButtonReleased(CommandTypes.SUSPEND);
                }
            }
        }


        public event ButtonPressedEventHandler ButtonPressed;
        public event ButtonReleasedEventHandler ButtonReleased;
        public event NewHeadingEventHandler NewHeading;
        public event GamepadConnectivityChangedHandler ConnectivityChanged;
        public bool invertYAxis { get; set;}

        public AgarIoGamepad()
        {
            shouldPoll = false;
            invertYAxis = false;
        }
        public virtual void sendGamepadConnectivityChanged(bool newStatus)
        {
            if (ConnectivityChanged != null)
                ConnectivityChanged(this, newStatus);
        }
        public virtual void sendNewHeading(Tuple<double,double> newHeading)
        {
            if (NewHeading != null)
                NewHeading(this, newHeading);
        }
        public virtual void sendButtonPressed(CommandTypes button)
        {
            if(ButtonPressed != null)
                ButtonPressed(this,button);
        }
        public virtual void sendButtonReleased(CommandTypes button)
        {
            if(ButtonReleased != null)
                ButtonReleased(this,button);
        }
        protected void setButtonState(CommandTypes button, bool value)
        {
            switch (button)
            {
                case CommandTypes.EJECT:
                    isEjectPressed = value;
                    break;
                case CommandTypes.QUIT:
                    isQuitPressed = value;
                    break;
                case CommandTypes.SPLIT:
                    isSplitPressed = value;
                    break;
                case CommandTypes.SUSPEND:
                    isSuspendControlPressed = value;
                    break;
            }
        }
        public async Task beginPolling(int pollDelay)
        {
            if (shouldPoll)
                return;

            shouldPoll = true;
            await Task.Run(() =>
                {
                    monitorLoop(pollDelay);
                });
        }
        public void stopPolling()
        {
            shouldPoll = false;
        }
        private async Task<string> monitorLoop(int pollDelay)
        {
            while (shouldPoll)
            {
                try {
                    Thread.Sleep(pollDelay);
                   // Console.WriteLine("amz polling");
                   poll();
                   // Console.WriteLine("more");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            return "done";
        }
        public abstract bool isControllerAvailable();
        public abstract Tuple<double,double> lastMovementStickDirection();
        public abstract bool poll();
        public abstract string getGamepadName();


    }
}

