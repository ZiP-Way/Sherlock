using Data;
using Data.RateUs;
using SignalsFramework;
using TapticFeedback;
using UnityEngine;

namespace Profile {
   /// <summary>
   /// Domain defined logic should be here.
   /// 
   /// Define properties, modify data loaded with postprocessing and apply versioning changes when needed.
   /// Or change existing ones to fit your needs.
   /// </summary>
   public static partial class PlayerProfile {
      //
      // Define logic related properties here
      //
      #region [Properties]

      /// <summary>
      /// Current game level
      /// </summary>
      public static int CurrentLevel {
         get => _data.CurrentLevel;
         set {
            // Clamp to 0..value
            value = value < 0 ? 0 : value;

            _data.CurrentLevel = value;
         }
      }

      /// <summary>
      /// Current available Soft Currency 
      /// </summary>
      public static long SoftCurrency {
         get => _data.SoftCurrency;
         set
         {
            // Using uints would require checked keyword (since overflow),
            // which will raise exception, but is it really required?
            // long should cover most (if not all apps)
            
            // Just clamp, leave the rest to the domain logic checks
            value = value < 0 ? 0 : value;
            
            _data.SoftCurrency = value;
            Hub.SoftCurrencyChanged.Fire(value);
         }
      }

      /// <summary>
      /// Current vibro state
      /// </summary>
      public static bool VibroDisabled {
         get => _data.VibroDisabled;
         set
         {
            _data.VibroDisabled = value;
            UpdateVibroState();

            Hub.VibroDisabled.Fire(value);
         }
      }

      /// <summary>
      /// Current audio state
      /// </summary>
      public static bool SoundDisabled {
         get => _data.SoundDisabled;
         set
         {
            _data.SoundDisabled = value;
            UpdateAudioState();

            Hub.SoundDisabled.Fire(value);
         }
      }

      public static RateUsData RateUsData {
         get => _data.RateUsData;
         set => _data.RateUsData = value;
      }

      /// <summary>
      /// Total sessions played for this player
      /// </summary>
      public static int TotalSessionsPlayed {
         get => _data.TotalSessionsPlayed;
         set => _data.TotalSessionsPlayed = value;
      }
      
      /// <summary>
      /// Current version of the application.
      /// Increment it when performing changes to the data layout to call HandleVersioning method.
      /// </summary>
      private const int AppVersion = 1;

      #endregion

      /// <summary>
      /// Performs a post processing on loaded data. Use this if you need to reset something in data, or apply
      /// some checks etc.
      /// </summary>
      /// <remarks>This method is called before HandleVersioning</remarks>
      private static void PostProcessLoadedData() {
         UpdateAudioState();
         UpdateVibroState();
      }

      /// <summary>
      /// Modify save file based on the version it was used on previously
      /// </summary>
      /// <remarks>Called after PostProcessLoadedData</remarks>
      private static void HandleVersioning() {
         int dataVersion = _data.Version;
         if (dataVersion == AppVersion) return;

         // Version changes can be implemented here //

         _data.Version = AppVersion;
      }

      private static void UpdateAudioState() { AudioListener.volume = SoundDisabled ? 0 : 1; }
      private static void UpdateVibroState() { Taptic.IsDisabled = VibroDisabled; }
   }
}