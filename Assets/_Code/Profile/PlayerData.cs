using System;
using Data.RateUs;

namespace Profile {
   /// <summary>
   /// Data that will be saved for the current player progress
   /// </summary>
   [Serializable]
   public class PlayerData {
      /// <summary>
      /// Should be used for versioning checks and changes only
      /// </summary>
      public int Version;

      public int CurrentLevel;
      public long SoftCurrency;

      public bool VibroDisabled;
      public bool SoundDisabled;

      public RateUsData RateUsData;
      
      public int TotalSessionsPlayed;
   }
}