using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.OS;
using Android.Widget;

namespace AutoWordQuiz {
  [Activity(Label = "AutoWordQuiz", MainLauncher = true, Icon = "@drawable/car")]
  public class MainActivity : Activity {
    private ArrayAdapter<string> listAdapter;
    private ListView lv;
    private EditText text;
    private List<string> Words;

    private List<string> LoadDict() {
      var result = new List<string>();
      Assembly assembly = typeof(MainActivity).GetTypeInfo().Assembly;
      using (Stream stream = assembly.GetManifestResourceStream("AutoWordQuiz.Resources.dict.txt")) {
        if (stream != null)
          using (StreamReader reader = new StreamReader(stream)) {
            while (!reader.EndOfStream) result.Add(reader.ReadLine());
          }
      }
#if DEBUG
      System.Diagnostics.Debug.WriteLine($"Total words loaded: {result.Count}");
#endif


      //// NOTE: use for debugging, not in released app code!
      //foreach (var res in assembly.GetManifestResourceNames())
      //  System.Diagnostics.Debug.WriteLine("found resource: " + res);

      return result;
    }

    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);
      this.Words = LoadDict();
      this.listAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);

      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.Main);

      Button button = FindViewById<Button>(Resource.Id.MyButton);
      this.text = FindViewById<EditText>(Resource.Id.editText1);

      button.Click += Button_Click;
      this.text.AfterTextChanged += Button_Click;

      this.lv = FindViewById<ListView>(Resource.Id.listView1);
      this.lv.Adapter = this.listAdapter;
    }

    private void Button_Click(object sender, EventArgs e) {
      string txt = this.text.Text;
      if (txt.Length != 3) return;

      this.listAdapter.Clear();
      this.listAdapter.AddAll(
                              this.Words.Where(
                                               w =>
                                                 w.StartsWith(txt[0].ToString()) && txt.All(w.Contains)
                                                 && w.IndexOf(txt[1], 1) < w.LastIndexOf(txt[2])).ToList());
    }
  }
}