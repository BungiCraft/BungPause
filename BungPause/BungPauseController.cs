using System.Collections.Generic;
using System.Linq;
using SiraUtil.Logging;
using UnityEngine.XR;
using Zenject;

namespace BungPause
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BungPauseController : IInitializable, ITickable
    {
        private readonly SiraLog _log;
        private readonly PauseController _pauseController;
        private static InputDevice _rightController;
        private bool _isPaused;
        private bool _lastButtonState;

        public BungPauseController(SiraLog log, PauseController pauseController)
        {
            _log = log;
            _pauseController = pauseController;
        }
        
        public void Initialize()
        {
            _pauseController.didPauseEvent += () => _isPaused = true;
            _pauseController.didResumeEvent += () => _isPaused = false;
            _rightController = TryGetInputDevice();
        }

        private static InputDevice TryGetInputDevice()
        {
            var inputDevices = new List<InputDevice>();
            const InputDeviceCharacteristics desiredCharacteristics = InputDeviceCharacteristics.HeldInHand 
                                                                      | InputDeviceCharacteristics.Right 
                                                                      | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, inputDevices);
            return inputDevices.Any() ? inputDevices[0] : new InputDevice();
        }
        
        public void Tick()
        {
            if (!_rightController.isValid)
            {
                _rightController = TryGetInputDevice();
            }
            
            var tempState = _rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, // Check if the primary 2D axis is clicked
                                out var primary2DAxisClickState) // Get the state of the primary 2D axis
                                && primary2DAxisClickState; // Set the tempState to the state of the primary 2D axis

            if (tempState == _lastButtonState) return;
            // If the state of the button is true and the last state is false, then the button was pressed
            // This makes it not trigger the method when u let go of the button
            if (tempState)
            {
                PauseBtnPressed();
            }
            _lastButtonState = tempState;
        }

        private void PauseBtnPressed()
        {
            switch (_isPaused)
            {
                case false:
                    _pauseController.Pause();
                    break;
                
                case true:
                    _pauseController.Invoke("HandlePauseMenuManagerDidPressContinueButton", 0.0f);
                    break;
            }
        }
    }
}
