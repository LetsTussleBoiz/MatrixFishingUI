using MatrixFishingUI.Framework;
using MatrixFishingUI.Framework.Fish;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewUI.Framework;
using StardewValley.Tools;

namespace MatrixFishingUI
{
    public class ModEntry : Mod
    {
        private static ModConfig _config = null!;
        private static IMonitor? _monitor;
        private readonly PerScreen<Lazy<GameLocation[]>> _locations = new(GetLocationsForCache);
        public static IViewEngine? ViewEngine;
        internal static FishManager Fish = null!;
        private bool HoldingRod = false;
        private IViewDrawable? hudWidget;
        private int _timer = 0;
        
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _monitor = Monitor;
            Monitor.Log($"Started with menu key {_config.OpenMenuKey}.");
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
            helper.Events.Display.RenderedHud += Display_RenderedHud;

            helper.Events.Input.ButtonsChanged += OnButtonChanged;

            helper.Events.World.LocationListChanged += OnLocationListChanged;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        
        private void Display_RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            hudWidget?.Draw(e.SpriteBatch, new(0, 100));
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
            Fish.RefreshFish();
            ResetLocationCache();
        }
        
        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            ResetLocationCache();
        }
        
        private void OnLocationListChanged(object? sender, LocationListChangedEventArgs e)
        {
            ResetLocationCache();
        }
        
        private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
        {
            // open menu
            if (_config.OpenMenuKey.JustPressed())
            {
                if (!Context.IsPlayerFree || Game1.currentMinigame != null)
                {
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
                    Game1.activeClickableMenu = ViewEngine?.CreateMenuFromAsset(
                        "Mods/Borealis.MatrixFishingUI/Views/ScrollingItemGrid",
                        context);
                }
            }
        }
        
        private void ToggleHud()
        {
            if (hudWidget is not null)
            {
                hudWidget.Dispose();
                hudWidget = null;
            }
            else
            {
                hudWidget = ViewEngine?.CreateDrawableFromAsset("Mods/Borealis.MatrixFishingUI/Views/Hud");
                if (hudWidget != null) hudWidget.Context = new HudMenuData(Fish.GetAllFish());
                HudMenuData? data;
                if (hudWidget?.Context != null)
                {
                    data = (HudMenuData)hudWidget.Context;
                    data.UpdateLocalFish();
                }
                LogTrace("Attempting HUD toggle On...");
            }
        }
        
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
            switch (Game1.player.CurrentTool)
            {
                case FishingRod when !HoldingRod:
                    HoldingRod = true;
                    ToggleHud();
                    return;
                case FishingRod when HoldingRod:
                    break;
            }

            if (Game1.player.CurrentTool is FishingRod || !HoldingRod) return;
            HoldingRod = false;
            ToggleHud();
            LogTrace("Attempting HUD toggle off...");
        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (_timer < 10)
            {
                _timer++;
            }
            else
            {
                _timer = 0;
                if (hudWidget?.Context == null) return;
                var context = (HudMenuData)hudWidget.Context;
                context.UpdateLocalFish();
            }
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
            ViewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            ViewEngine?.RegisterViews("Mods/Borealis.MatrixFishingUI/Views", "assets/views");
            ViewEngine?.RegisterSprites("Mods/Borealis.MatrixFishingUI/Sprites", "assets/sprites");
            ViewEngine?.EnableHotReloading();
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
            _monitor?.Log(input, LogLevel.Info);
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
        
        public static void LogTrace(string input)
        {
            _monitor?.Log(input);
        }
    }
}