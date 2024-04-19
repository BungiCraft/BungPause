using System;
using IPA;
using SiraUtil.Logging;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace BungPause
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static IPALogger Logger;
        private SiraLog _log;
        
        
        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            Logger = logger;
            
            zenjector.UseLogger(Logger);
            
            zenjector.Install(Location.GameCore, container =>
            {
                container.BindInterfacesAndSelfTo<BungPauseController>().AsSingle().NonLazy();
            });
        }

        [OnEnable, OnDisable]
        public void OnStateChanged()
        { }
    }
}
