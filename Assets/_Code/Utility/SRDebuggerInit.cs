#if DEVELOPMENT_BUILD || UNITY_EDITOR
using UnityEngine;

namespace Utility {
   /// <summary>
   /// Performs an initialization for SRDebugger when build is set to development
   /// </summary>
   public static class SRDebuggerInit {
      [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
      private static void SRInit() {
         SRDebug.Init();
      }
   }
}
#endif