using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EfremovaParser {
  internal class Program {
    private static void Main(string[] args) {
      //string filePath = @"C:\Users\iabramov\Documents\Visual Studio 2017\Projects\AutoWordQuiz\Dict\efremova_light.txt";
      string filePath = @"C:\Users\iabramov\Documents\Visual Studio 2017\Projects\AutoWordQuiz\Dict\efremovau.txt";
      string outFile =
        @"C:\Users\iabramov\Documents\Visual Studio 2017\Projects\AutoWordQuiz\Dict\efremovau_out_nodesc.txt";
      FileStream stream = new FileStream(filePath, FileMode.Open);

      TextReader reader = new StreamReader(stream);
      var words = new List<Word>();

      string str = "";
      string strPrev = "";

      Console.WriteLine("Парсим файлик...");

      do {
        var lines = new List<string>();
        do {
          strPrev = str;
          str = reader.ReadLine();
          lines.Add(str?.Trim());
        } while (!string.IsNullOrEmpty(str));

        if (Word.New(lines, out Word newWord)) words.Add(newWord);
      } while (!string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(strPrev));

      Console.WriteLine("Всего слов: " + words.Count);
      Console.WriteLine("Пишем в файл...");
      File.WriteAllLines(outFile, words.OutputWords());
      Console.WriteLine("Нажмите любую клавишу для завершения работы...");
      Console.ReadKey();
    }
  }

  public class Word {
    public Word(List<string> alllines) {
      if (!(alllines.Count > 1)) return;

      var lines = alllines.Where(l => l != null).ToList();

      this.word = lines[0];
      var meanings = new List<string>();
      string[] mnngs = {"1.", "2.", "3.", "4.", "5.", "6.", "7.", "8.", "9."};
      string[] rods = {"м.", "ж.", "ср."};
      for (int i = 1; i < lines.Count; i++) this.Description += lines[i];

      var exactMeaning = lines?.Where(l => rods.Any(r => l.StartsWith(r))).ToList();
      var multiMeanings = lines.Where(line => mnngs.Any(mnng => line.StartsWith(mnng)));

      foreach (string multiMeaning in multiMeanings)
      foreach (string m in mnngs)
        if (multiMeaning.StartsWith(m)) {
          exactMeaning.Add(multiMeaning.Replace(m, "").Trim());
          break;
        }

      foreach (string mean in exactMeaning) {
        if (!rods.Any(r => mean.StartsWith(r))) continue;

        if (mean.StartsWith("м.")) {
          this.rod = Rod.Мужской;
          break;
        }

        if (mean.StartsWith("ж.")) {
          this.rod = Rod.Женский;
          break;
        }

        if (mean.StartsWith("ср.")) {
          this.rod = Rod.Средний;
          break;
        }
      }
    }

    public Rod rod { get; set; }
    public string word { get; set; }
    public string Description { get; set; }

    public static bool New(List<string> lines, out Word word) {
      word = null;
      if (string.IsNullOrEmpty(lines[0])) return false;

      string w = lines[0];
      if (w.StartsWith("-") || w.EndsWith("-")) return false;

      word = new Word(lines);
      if (word.rod == 0) return false;

      return true;
    }

    public static Word tst(Word word) { return word; }
  }

  public enum Rod {
    Мужской = 1,
    Женский,
    Средний
  }

  public static class ListExtensions {
    public static string[] OutputWords(this List<Word> list) {
      var result = new string[list.Count];
      for (int i = 0; i < list.Count; i++) {
        Word w = list[i];
        //result[i] = w.word + "||" + w.Description;
        result[i] = w.word;
      }

      return result;
    }
  }
}