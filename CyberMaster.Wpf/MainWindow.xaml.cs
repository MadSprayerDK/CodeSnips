using System;
using System.Windows;
using System.Windows.Input;

namespace CyberMasterWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CyberMaster.CyberMaster _cyberMaster;

        public MainWindow()
        {
            InitializeComponent();
            _cyberMaster = new CyberMaster.CyberMaster();
        }

        private void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            var res = _cyberMaster.Connect();

            StatusLabel.Content = res ? "Success" : "Error";
        }

        private void Motor2Backward_OnMouseDown(object sender, EventArgs e)
        {
            _cyberMaster.MotorOn(CyberMaster.CyberMaster.MotorDirection.Backward, 2);
        }

        private void Motor2Backward_OnMouseUp(object sender, EventArgs e)
        {
           _cyberMaster.MotorOff(2);
        }

        private void Motor0Backward_OnMouseDown(object sender, EventArgs e)
        {
            _cyberMaster.MotorOn(CyberMaster.CyberMaster.MotorDirection.Backward, 0);
        }

        private void Motor0Backward_OnMouseUp(object sender, EventArgs e)
        {
            _cyberMaster.MotorOff(0);
        }

        private void Motor1Forward_OnMouseDown(object sender, EventArgs e)
        {
            _cyberMaster.MotorOn(CyberMaster.CyberMaster.MotorDirection.Forward, 1);
        }

        private void Motor1Forward_OnMouseUp(object sender, EventArgs e)
        {
            _cyberMaster.MotorOff(1);
        }

        private void Motor1Backward_OnMouseDown(object sender, EventArgs e)
        {
            _cyberMaster.MotorOn(CyberMaster.CyberMaster.MotorDirection.Backward, 1);
        }

        private void Motor1Backward_OnMouseUp(object sender, EventArgs e)
        {
            _cyberMaster.MotorOff(1);
        }

        private void Motor0Forward_OnMouseDown(object sender, EventArgs e)
        {
            _cyberMaster.MotorOn(CyberMaster.CyberMaster.MotorDirection.Forward, 0);
        }

        private void Motor0Forward_OnMouseUp(object sender, EventArgs e)
        {
            _cyberMaster.MotorOff(0);
        }

        private void Motor2Forward_OnMouseDown(object sender, EventArgs e)
        {
            _cyberMaster.MotorOn(CyberMaster.CyberMaster.MotorDirection.Forward, 2);
        }

        private void Motor2Forward_OnMouseUp(object sender, EventArgs e)
        {
            _cyberMaster.MotorOff(2);
        }
    }
}
