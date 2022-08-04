namespace Data.Analytics {
   /// <summary>
   /// Data for passing level analytics events
   /// </summary>
   public struct LevelAnalyticsData {
      public const string WinCompletionReason = "win";
      public const string FailedCompletionReason = "fail";
      
      /// <summary>
      /// This should be actual level index + 1
      /// </summary>
      public string LevelNumber;
      public string LevelName;
      public string TotalSessionsPlayed;
      
      // level_finish only
      public string CompletionResult;
      public string PlayTime;
      public string Progress;
      //

      // TODO implement rest of analytic parameters if they're required
   }
}