using MatrixFishingUI.Framework;
using MatrixFishingUI.Framework.Fish;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewUI.Framework;

namespace MatrixFishingUI
{
    public class ModEntry : Mod
    {
        public static ModConfig Config = null!;
        public static IMonitor? _monitor;
        private readonly PerScreen<Lazy<GameLocation[]>> _locations = new(GetLocationsForCache);
        private IViewEngine? _viewEngine;
        internal static FishManager Fish = null!;
        
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            _monitor = Monitor;
            Monitor.Log($"Started with menu key {Config.OpenMenuKey}.");
            Fish = new(this);
            I18n.Init(helper.Translation);
            
            // hook events
            helper.Events.Display.Rendered += OnRendered;
            helper.Events.Display.MenuChanged += OnMenuChanged;

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

            helper.Events.Input.ButtonsChanged += OnButtonChanged;

            helper.Events.World.LocationListChanged += OnLocationListChanged;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        
        private void GenerateGMCM()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            // var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            // if (configMenu is null) return;
            //
            // // register mod
            // configMenu.Register(
            //     mod: ModManifest,
            //     reset: () => _config = new ModConfig(),
            //     save: () => Helper.WriteConfig(_config)
            // );
            //
            // // Twitch Auth
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Twitch_Title,
            //     tooltip: I18n.GMCM_Twitch_Tooltip
            // );
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Twitch_Instr
            // );
            // configMenu.AddTextOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Twitch_Paste_Here,
            //     getValue: () => _config.OAuthToken,
            //     setValue: value =>
            //     {
            //         _config.OAuthToken = value;
            //         Monitor.Log("Broadcaster OAuthToken saved", LogLevel.Info);
            //     }
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddPageLink(
            //     mod: ModManifest,
            //     pageId: "Redeem Configuration",
            //     text: () => "Configure Redeems"
            //     );
            //
            // configMenu.AddPage(
            //     mod: ModManifest,
            //     pageId: "Redeem Configuration"
            //     );
            //
            // // Redeem Configuration
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Redeems_Title,
            //     tooltip: I18n.GMCM_Redeems_Tooltip
            //     );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            //     );
            //
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Money_Title,
            //     tooltip: I18n.GMCM_Money_Tooltip
            //     );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Money_Maximum,
            //     getValue: () => _config.GiftMoneyMax,
            //     setValue: value =>
            //     { _config.GiftMoneyMax = value; }
            //     );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Money_Minimum,
            //     getValue: () => _config.GiftMoneyMin,
            //     setValue: value =>
            //     {
            //         _config.GiftMoneyMin = value;
            //     }
            //     );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Buff_Title,
            //     tooltip: I18n.GMCM_Buff_Tooltip
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Buff_Duration,
            //     getValue: () => _config.BuffDuration,
            //     setValue: value =>
            //     { _config.BuffDuration = value; },
            //     min: 5,
            //     max: 120
            // );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Debuff_Duration,
            //     getValue: () => _config.DebuffDuration,
            //     setValue: value =>
            //     { _config.DebuffDuration = value; },
            //     min: 5,
            //     max: 120
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Invert_Title,
            //     tooltip: I18n.GMCM_Invert_Tooltip
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Invert_Duration,
            //     getValue: () => _config.InvertDuration,
            //     setValue: value =>
            //     { _config.InvertDuration = value; },
            //     min: 5,
            //     max: 30
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Monster_Title,
            //     tooltip: I18n.GMCM_Monster_Tooltip
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Monster_Amount,
            //     getValue: () => _config.NumberOfMonsters,
            //     setValue: value =>
            //     { _config.NumberOfMonsters = value; },
            //     min: 1,
            //     max: 10
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddSectionTitle(
            //     mod: ModManifest,
            //     text: I18n.GMCM_Smite_Title,
            //     tooltip: I18n.GMCM_Smite_Tooltip
            // );
            //
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );
            //
            // configMenu.AddNumberOption(
            //     mod: ModManifest,
            //     name: I18n.GMCM_Smite_Dmg,
            //     getValue: () => _config.SmiteDamage,
            //     setValue: value =>
            //     { _config.SmiteDamage = value; },
            //     min: 1,
            //     max: 50
            // );

            Monitor.Log("GMCM Generated", LogLevel.Debug);
        }
        
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            ResetLocationCache();
        }

        /// <summary>Raised after the player returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            ResetLocationCache();
        }

        /// <summary>Raised after a game location is added or removed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLocationListChanged(object? sender, LocationListChangedEventArgs e)
        {
            ResetLocationCache();
        }

        /// <summary>Raised after the player presses or releases any keys on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
        {
            // open menu
            if (Config.OpenMenuKey.JustPressed())
            {
                if (!Context.IsPlayerFree || Game1.currentMinigame != null)
                {
                    // Players often ask for help due to the menu not opening when expected. To
                    // simplify troubleshooting, log when the key is ignored.
                    if (Game1.currentMinigame != null)
                        Monitor.Log($"Received menu open key, but a '{Game1.currentMinigame.GetType().Name}' minigame is active.");
                    else if (Game1.eventUp)
                        Monitor.Log("Received menu open key, but an event is active.");
                    else if (Game1.activeClickableMenu != null)
                        Monitor.Log($"Received menu open key, but a '{Game1.activeClickableMenu.GetType().Name}' menu is already open.");
                    else
                        Monitor.Log("Received menu open key, but the player isn't free.");
                }

                else
                {
                    Monitor.Log("Received menu open key.");
                    var context = FishMenuData.GetFish();
                    Game1.activeClickableMenu = _viewEngine?.CreateMenuFromAsset(
                        "Mods/TestMod/Views/ScrollingItemGrid",
                        context);
                }
            }
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        /// <inheritdoc cref="IGameLoopEvents.Saving"/>
        private void OnSaving(object? sender, SavingEventArgs e)
        {
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
        }

        /// <summary>Raised after game is launched.</summary>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            _viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            _viewEngine?.RegisterViews("Mods/TestMod/Views", "assets/views");
            _viewEngine?.EnableHotReloading();
            GenerateGMCM();
        }
        
        private void ResetLocationCache()
        {
            if (_locations.Value.IsValueCreated)
                _locations.Value = GetLocationsForCache();
        }

        /// <summary>Get a cached lookup of available locations.</summary>
        private static Lazy<GameLocation[]> GetLocationsForCache()
        {
            return new Lazy<GameLocation[]>(() => Context.IsWorldReady
                ? CommonHelper.GetAllLocations().ToArray()
                : []
            );
        }

        public static void Log(string input)
        {
            _monitor?.Log(input);
        }
        
        public static void LogError(string input)
        {
            _monitor?.Log(input, LogLevel.Error);
        }
        
        public static void LogDebug(string input)
        {
            _monitor?.Log(input, LogLevel.Debug);
        }
        
        public static void LogWarn(string input)
        {
            _monitor?.Log(input, LogLevel.Warn);
        }
    }
}