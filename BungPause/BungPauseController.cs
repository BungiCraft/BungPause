using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.XR;
using Zenject;
using Debug = System.Diagnostics.Debug;

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
            _pauseController = pauseController;
            _pauseMenuManager = pauseMenu;
        }

        private InputDevice TryGetInputDevice()
        {
            var inputDevices = new List<InputDevice>();
            const InputDeviceCharacteristics desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, inputDevices);

            return inputDevices.Any() ? inputDevices[0] : new InputDevice();
        }
        
        #region Monobehaviour Messages
        public void Initialize()
        {
            _rightController = TryGetInputDevice();
            _pauseMenuManager?.CancelInvoke();
        }
        
        public void Tick()
        {
            if (!_rightController.isValid)
            {
                _rightController = TryGetInputDevice();
            }
            
            if (_rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var triggerValue) && triggerValue && triggerValue != _lastButtonState)
            {
                _lastButtonState = triggerValue;
            }
            
            switch (_paused)
            { 
                case false:
                    _paused = true;
                    _lastButtonState = triggerValue;
                    _pauseController?.Pause();
                    break;
                
                case true:
                    _paused = false;
                    _pauseMenuManager.Invoke("ContinueButtonPressed", 0.0f);
                    break;
            }
        }

        public void Dispose()
        {
            _log.Info("get rek'd lol");
        }
        #endregion
    }
}
