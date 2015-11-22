using AgarIoGamepadLib;
using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Agar360
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isSuspended;
        public bool isToggledSuspended;
        public AgarIoGamepad gamepad;
        public IGameInputBridge gameInputBridge;



        public MainWindow()
        {
            InitializeComponent();

            isSuspended = false;
            isToggledSuspended = true;

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;



            gamepad = new Xbox360AgarIoGamepad(UserIndex.One);
            gameInputBridge = new InputBridge_InputSimulator(screenWidth / screenHeight);


            gamepad.ButtonPressed += new ButtonPressedEventHandler(handleButtonPressed);
            gamepad.ButtonReleased += new ButtonReleasedEventHandler(handleButtonReleased);
            gamepad.NewHeading += new NewHeadingEventHandler(handleNewDirection);
            gamepad.ConnectivityChanged += new GamepadConnectivityChangedHandler(handleNewConnectivityStatus);

            
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            isToggledSuspended = false;
            
        }

        void handleNewConnectivityStatus(object sender, bool curStatus)
        {


        }
        void handleNewDirection(object sender, Tuple<double, double> newHeading)
        {
            if (!isSuspended && !isToggledSuspended)
                gameInputBridge.setMovement(newHeading);
        }
        void handleButtonPressed(object sender, CommandTypes buttonType)
        {
            

            switch (buttonType)
            {
                case CommandTypes.EJECT:
                    if (!isToggledSuspended)
                        gameInputBridge.eject();
                        break;
                case CommandTypes.QUIT:
                    isToggledSuspended = !isToggledSuspended;
                    break;
                case CommandTypes.SPLIT:
                    if (!isToggledSuspended)
                        gameInputBridge.split();
                    break;
                case CommandTypes.SUSPEND:
                    if (!isToggledSuspended)
                        isSuspended = true;
                    break;
            }
        }
        void handleButtonReleased(object sender, CommandTypes buttonType)
        {
            if (buttonType == CommandTypes.SUSPEND && !isToggledSuspended)
                isSuspended = false;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            isToggledSuspended = true;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            gamepad.isControllerAvailable();
        }

        private void loaded_Loaded(object sender, RoutedEventArgs e)
        {
            gamepad.beginPolling(50);
            textBlock.Text = "Right Stick = Move\nRight Bumper = Split\nLeft Bumper=Eject Mass\n" +
                             "Hold Y to release your mouse\n" +
                             "Press A to toggle simulation altogether" +
                             "the start stop buttons here set the same toggle as the A button.";
        }
    }
}
