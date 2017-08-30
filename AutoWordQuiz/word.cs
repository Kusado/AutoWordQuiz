using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AutoWordQuiz {
  public static class ObjectTypeHelper {
    public static T Cast<T>(this Java.Lang.Object obj) where T : class {
      var propertyInfo = obj.GetType().GetProperty("Instance");
      return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
    }
  }
  internal class word /*: Java.Lang.Object */{
    public string Word { get; set; }
    public string Description { get; set; }
    public word(string line, out bool sucs) {
      try {
        var s = line.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
        this.Word = s[0];
        this.Description = s[1];
        sucs = this.Word.Length > 2;
      }
      catch (Exception) {
        Console.WriteLine();
        sucs = false;
      }
    }

    public override string ToString() { return this.Word; }
   
  }

}