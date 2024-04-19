using System;
using System.Collections.Generic;
using System.Reflection;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

namespace BungPause
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class BungPauseController : IInitializable, IDisposable, ITickable
    {
        private bool _lastButtonState;
        private bool _paused;
        [Inject] private PauseController _pauseController;
        [Inject] private PauseMenuManager _pauseMenuManager;
        [Inject] private SiraLog _log;
        [Inject] private static InputDevice _rightController;

        public BungPauseController(PauseController pauseController, PauseMenuManager pauseMenu, SiraLog log)
        {
            _log = log;
            _log.Debug($"Initializing {Assembly.GetExecutingAssembly().GetName().Name}");
            _pauseController = pauseController;
            _pauseMenuManager = pauseMenu;
        }

        private InputDevice GetInputDevice()
        {
            var inputDevices = new List<InputDevice>();
            var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, inputDevices);
            _log.Debug($"Found controller characteristic: {desiredCharacteristics} with count: {inputDevices.Count}");
            return inputDevices.Count > 0 ? inputDevices[0] : new InputDevice();
        }

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        public void Initialize()
        {
            _pauseMenuManager?.CancelInvoke();
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        
        public void Tick()
        {
            _log.Debug("why doe smodding suck");
            if (_rightController.isValid)
            { 
                _log.Debug($"{_rightController} was valid.");
                if (_rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var triggerValue) && triggerValue && triggerValue != _lastButtonState)
                {
                    switch (_paused)
                    { 
                        case false:
                            _log.Debug($"case was {_paused}");
                            _paused = true;
                            _lastButtonState = triggerValue;
                            _pauseController?.Pause();
                            break;
                        case true:
                            _log.Debug($"case was {_paused}");
                            _paused = false;
                            _pauseMenuManager?.Invoke("ContinueButtonPressed", 0f);
                            break;
                    }
                } else
                {
                    _lastButtonState = triggerValue;
                }
            } else
            {
                _rightController = GetInputDevice();
                _log.Debug(_rightController.name);
            }
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        public void Dispose()
        {
            _log.Info("get rek'd lol");
        }
        #endregion
    }
}
