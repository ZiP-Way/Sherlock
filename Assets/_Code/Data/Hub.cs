using Data.Analytics;
using Data.Levels;
using Data.RateUs;
using UniRx;

namespace Data
{
    /// <summary>
    /// Main events hub that are available to the application for subscription.
    /// Add / Remove them here when necessary
    /// </summary>
    public static class Hub
    {
        #region [Game Flow]

        /// <summary>
        /// Fired when next level should be loaded
        /// </summary>
        /// <remarks>Loads PlayerProfile.CurrentLevel, make sure to increment</remarks>
        public static readonly Subject<Unit> LoadLevel = new Subject<Unit>();

        /// <summary>
        /// Fired when level has been generated.
        /// </summary>
        public static readonly Subject<LevelMetaData> LevelGenerationCompleted = new Subject<LevelMetaData>();

        /// <summary>
        /// Fired when user has started new game. E.g. when pressed on play button.
        /// Subscribe to this event to initialize / spawn level, start game logic etc.
        /// </summary>
        public static readonly Subject<Unit> GameStarted = new Subject<Unit>();

        /// <summary>
        /// Fired when level has been completed
        /// </summary>
        public static readonly Subject<Unit> LevelComplete = new Subject<Unit>();

        /// <summary>
        /// Fired when level failed condition met
        /// </summary>
        public static readonly Subject<Unit> LevelFailed = new Subject<Unit>();

        /// <summary>
        /// Fired when cleanup on systems should be performed.
        /// Can be on level start, or on lobby transition, see what fits best.
        /// </summary>
        public static readonly Subject<Unit> Cleanup = new Subject<Unit>();

        #endregion

        #region [UI Flow]

        /// <summary>
        /// Fired when Lobby Screen should be displayed
        /// </summary>
        public static readonly Subject<Unit> RequestLobbyTransition = new Subject<Unit>();

        /// <summary>
        /// Fired when coin fx completes
        /// </summary>
        public static readonly Subject<Unit> CoinFlightComplete = new Subject<Unit>();

        #endregion

        #region [Profile]

        /// <summary>
        /// Fired when soft currency value changes
        /// </summary>
        public static readonly Subject<long> SoftCurrencyChanged = new Subject<long>();

        /// <summary>
        /// Fired when application vibration state changes
        /// </summary>
        public static readonly Subject<bool> VibroDisabled = new Subject<bool>();

        /// <summary>
        /// Fired when application sound state changes
        /// </summary>
        public static readonly Subject<bool> SoundDisabled = new Subject<bool>();

        #endregion

        #region [Rate us]

        /// <summary>
        /// Fired when Rate Us overlay should be shown
        /// </summary>
        public static readonly Subject<RateUsRequest> ShowRateUs = new Subject<RateUsRequest>();

        #endregion

        #region [Purchasing]

        /// <summary>
        /// Fired when purchase restore is requested
        /// </summary>
        public static readonly Subject<Unit> RestorePurchases = new Subject<Unit>();

        #endregion

        #region [Analytics]

        /// <summary>
        /// Fired when Rate Us event should be tracked
        /// </summary>
        public static readonly Subject<RateUsTrackData> TrackRateUs = new Subject<RateUsTrackData>();

        #endregion

        #region [Cheats]

        /// <summary>
        /// Fired when soft currency value should be force updated ignoring any logic
        /// </summary>
        public static readonly Subject<Unit> ForceRefreshSoftCurrency = new Subject<Unit>();

        #endregion
    }
}