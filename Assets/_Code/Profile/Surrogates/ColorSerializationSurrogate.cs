using System.Runtime.Serialization;
using UnityEngine;

namespace Profile.Surrogates {
   public sealed class ColorSerializationSurrogate : ISerializationSurrogate {
      private const string R = "r";
      private const string G = "g";
      private const string B = "b";
      private const string A = "a";
      
      public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
         Color color = (Color) obj;
         info.AddValue(R, color.r);
         info.AddValue(G, color.g);
         info.AddValue(B, color.b);
         info.AddValue(A, color.a);
      }

      public object SetObjectData(object obj,
                                  SerializationInfo info,
                                  StreamingContext context,
                                  ISurrogateSelector selector) {
         Color color = (Color) obj;
         color.r = (float) info.GetValue(R, typeof(float));
         color.g = (float) info.GetValue(G, typeof(float));
         color.b = (float) info.GetValue(B, typeof(float));
         color.a = (float) info.GetValue(A, typeof(float));

         obj = color;
         return obj;
      }
   }
}