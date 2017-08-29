using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Views.InputMethods;
using Android.Widget;

namespace AutoWordQuiz {
  [Activity(Label = "AutoWordQuiz", MainLauncher = true, Icon = "@drawable/car", Theme = "@android:style/Theme.NoTitleBar")]
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
            while (!reader.EndOfStream) {
              string tmp = reader.ReadLine();
              if (tmp?.Length > 2) result.Add(tmp);
            }
          }
      }
#if DEBUG
      System.Diagnostics.Debug.WriteLine($"Total words loaded: {result.Count}");
      // NOTE: use for debugging, not in released app code!
      foreach (var res in assembly.GetManifestResourceNames()) {
        System.Diagnostics.Debug.WriteLine("found resource: " + res);
      }
#endif



      return result;
    }

    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);
      this.Words = LoadDict();
      this.listAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);


      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.Main);

      this.text = FindViewById<EditText>(Resource.Id.editText1);

      this.text.AfterTextChanged += TextChanged;

      this.lv = FindViewById<ListView>(Resource.Id.listView1);
      this.lv.Adapter = this.listAdapter;

    }


    private void TextChanged(object sender, AfterTextChangedEventArgs e) {
      if (this.text.Text.Length != 3) return;
      GetWords(sender);
    }

    private void GetWords(object sender) {

      string txt = this.text.Text;
      string s1, s2, s3;
      s1 = txt[0].ToString();
      s2 = txt[1].ToString();
      s3 = txt[2].ToString();

      if (txt.Length != 3) return;

      this.listAdapter.Clear();
      this.listAdapter.AddAll(this.Words.Where(w => Regex.IsMatch(w, $"^{s1}.*{s2}.*{s3}.*")).ToList());

      if (this.listAdapter.Count > 4) {
        InputMethodManager manager = (InputMethodManager)GetSystemService(InputMethodService);
        manager.HideSoftInputFromWindow(this.text.WindowToken, HideSoftInputFlags.None);
      }
    }

    public override void OnBackPressed() {
      Process.KillProcess(Process.MyPid());
    }
  }
}