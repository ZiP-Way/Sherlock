using System.Collections.Generic;
using Ordering;
using UnityEngine;

namespace Data.Levels {
   /// <summary>
   /// Data storage that holds references to levels
   /// </summary>
   [CreateAssetMenu(menuName = "Levels/Level List", fileName = "levelLst_", order = SOrder.Levels)]
   public class LevelList : ScriptableObject {
      [SerializeField]
      private List<LevelData> _availableLevels = new List<LevelData>();

      #region [Properties]

      public List<LevelData> AllLevels => _availableLevels;

      #endregion
   }
}