using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;

namespace NoEnergyLossMod
{
    public class ModConfig
    {
        public bool EnergyLossDisabled { get; set; } = false;
    }

    public class NoEnergyLossMod : Mod
    {
        private ModConfig Config = new ModConfig();

        public override void Entry(IModHelper helper)
        {
            // Carregar a configuração
            Config = helper.ReadConfig<ModConfig>();

            // Registrar eventos
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Player.Warped += OnPlayerWarped;

            // Registrar para Generic Mod Config Menu quando o jogo lançar
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                RegisterGMCM();
        }

        private void RegisterGMCM()
{
    var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
    if (api == null)
        return;

    var i18n = Helper.Translation;

    // Registrar o mod no GMCM sem o parâmetro 'name'
    api.Register(
        mod: ModManifest,
        reset: () => Config = new ModConfig(),
        save: () => Helper.WriteConfig(Config)
    );

    // Adicionar a opção de configuração de perda de energia
    api.AddBoolOption(
        mod: ModManifest,
        name: () => i18n.Get("config.energyLossDisabled"),
        tooltip: () => i18n.Get("config.energyLossDisabled.tooltip"),
        getValue: () => Config.EnergyLossDisabled,
        setValue: value => Config.EnergyLossDisabled = value
    );
}
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Config.EnergyLossDisabled)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
            else
            {
                Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }

        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (Config.EnergyLossDisabled)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
            else
            {
                Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Impedir a perda de energia quando desativado
            if (Config.EnergyLossDisabled)
            {
                Game1.player.Stamina = Game1.player.MaxStamina;
            }
        }
    }
}