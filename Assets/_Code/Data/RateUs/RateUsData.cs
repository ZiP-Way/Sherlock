namespace Data.RateUs {
   /// <summary>
   /// Data storage for persistence
   /// </summary>
   [System.Serializable]
   public struct RateUsData {
      public bool RateUsDeniedCompletely;
      public bool RateUsFirstCancel;
      public bool RateUsSecondCancel;

      public int RateUsLevels;
      public int RateUsSessionsPlayed;
      
      public long LastCancelTime;
   }
}