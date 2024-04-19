using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace BungPause
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class BungPauseController : MonoBehaviour
    {
        public static BungPauseController Instance { get; private set; }
        //private static PauseController pauseController = new PauseController();
        private static InputDevice rightController = new InputDevice();

        private static InputDevice GetInputDevice()
        {
            List<InputDevice> inputDevices = new List<InputDevice>();
            InputDeviceCharacteristics desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, inputDevices);
            if (inputDevices.Count > 0)
            {
                return inputDevices[0];
            }
            else
            {
                return new InputDevice();
            }
        }

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private bool lastButtonState = false;
        private bool paused = false;
        
        private void Update()
        {
            if (rightController.isValid == true)
            {
                if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool triggerValue) && triggerValue && triggerValue != lastButtonState)
                {
                    PauseController pauseController = FindObjectOfType<PauseController>();
                    PauseMenuManager pauseMenuManager = FindObjectOfType<PauseMenuManager>();
                    pauseMenuManager?.CancelInvoke();
                    if (paused == false)
                    {
                        paused = true;
                        lastButtonState = triggerValue;
                        pauseController?.Pause();
                    } else if (paused == true)
                    {
                        paused = false;
                        pauseMenuManager?.Invoke("ContinueButtonPressed", 0.0f);
                        
                    }
                } else
                {
                    lastButtonState = triggerValue;
                }
            } else
            {
                rightController = GetInputDevice();
                Console.WriteLine(rightController.name);
            }
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
    }
}
