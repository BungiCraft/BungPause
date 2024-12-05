using IPA;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace BungPause
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private static IPALogger _logger;
        
        [Init]
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            _logger = logger;
            zenjector.UseLogger(_logger);
            zenjector.Install(Location.GameCore, container =>
            {
                container.BindInterfacesAndSelfTo<BungPauseController>().AsSingle().NonLazy();
            });
        }
    }
}
