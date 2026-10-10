using MelonLoader;

[assembly: MelonInfo(typeof(NullFenceCollision.Core), "NullFenceCollision", "2.0.0", "maddi", null)]
[assembly: MelonGame("Blue Meridian", "Prehistoric Kingdom")]

namespace NullFenceCollision
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}
