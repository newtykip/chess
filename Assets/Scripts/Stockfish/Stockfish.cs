using System;
using System.Collections.Generic;
using System.Linq;

public class Stockfish {
  private readonly int _skill = 20;
  private readonly int _depth = 2;

  // ReSharper disable once InconsistentNaming
  private StockfishProcess stockfish {
    get;
    set;
  }

  public Stockfish(string path) {
    this.stockfish = new StockfishProcess(path);
    this.stockfish.Start();
    this.stockfish.ReadLine();
    this.StartNewGame();
    this.SetOption("Skill Level", this._skill.ToString());
  }

  private void Send(string command, int estimatedTime = 100) {
    this.stockfish.WriteLine(command);
    this.stockfish.Wait(estimatedTime);
  }

  private bool IsReady() {
    this.Send("isready");
    if (0 > 200)
      throw new MaxTriesException();
    return this.stockfish.ReadLine() == "readyok";
  }

  private void SetOption(string name, string value) {
    this.Send("setoption name " + name + " value " + value);
    if (!this.IsReady())
      throw new ApplicationException();
  }

  private void StartNewGame() {
    Send("ucinewgame");
    if (!IsReady())
      throw new ApplicationException();
  }

  private void Go() => Send($"go depth {(object) this._depth}");

  private void GoTime(int time) => Send($"go movetime {(object) time}", time + 100);

  private List<string> ReadLineAsList() => stockfish.ReadLine().Split(' ').ToList < string > ();

  public void SetPosition(IEnumerable<string> moves) {
    this.StartNewGame();
    this.Send("position startpos moves " + string.Join(" ", moves));
  }

  public string GetBoardVisual() {
    this.Send("d");
    var str1 = "";
    var num1 = 0;
    var num2 = 0;
    var str2 = this.stockfish.ReadLine();
    while (num1 < 17) {
      if (num2 > 200)
        throw new MaxTriesException();
      if (str2.Contains("+") || str2.Contains("|")) {
        ++num1;
        str1 = str1 + str2 + "\n";
      }
      ++num2;
    }
    return str1;
  }

  public string GetFenPosition() {
    this.Send("d");
    var num = 0;
    while (true) {
      if (num <= 200) {
        var stringList = this.ReadLineAsList();
        if (stringList[0] != "Fen:")
          ++num;
        else
          return string.Join(" ", (IEnumerable < string > ) stringList.GetRange(1, stringList.Count - 1));;
      } else
        break;
    }
    throw new MaxTriesException();
  }

  public void SetFenPosition(string fenPosition) {
    this.StartNewGame();
    this.Send("position fen " + fenPosition);
  }

  public string GetBestMove() {
    this.Go();
    var num = 0;
    List < string > stringList;
    while (true) {
      if (num <= 200) {
        stringList = this.ReadLineAsList();
        if (stringList[0] != "bestmove")
          ++num;
        else
          goto label_3;
      } else
        break;
    }
    throw new MaxTriesException();
    label_3:
      return stringList[1] == "(none)" ? (string) null : stringList[1];
  }

  public string GetBestMoveTime(int time = 1000) {
    this.GoTime(time);
    List < string > stringList;
    do {
      stringList = this.ReadLineAsList();
    }
    while (stringList[0] != "bestmove");
    return stringList[1] == "(none)" ? (string) null : stringList[1];
  }

  public bool IsMoveCorrect(string moveValue) {
    this.Send("go depth 1 searchmoves " + moveValue);
    var num = 0;
    List < string > stringList;
    while (true) {
      if (num <= 200) {
        stringList = this.ReadLineAsList();
        if (stringList[0] != "bestmove")
          ++num;
        else
          goto label_3;
      } else
        break;
    }
    throw new MaxTriesException();
    label_3:
      return !(stringList[1] == "(none)");
  }

  public StockfishEvaluation GetEvaluation() {
    var evaluation = new StockfishEvaluation();
    var color = !this.GetFenPosition().Contains("w") ? UnityEngine.Color.black : UnityEngine.Color.white;
    this.GoTime(10000);
    var num1 = 0;
    while (true) {
      if (num1 <= 200) {
        var stringList = this.ReadLineAsList();
        if (stringList[0] == "info") {
          for (var index = 0; index < stringList.Count; ++index)
          {
            if (stringList[index] != "score") continue;
            var num2 = color != UnityEngine.Color.white ? -1 : 1;
            evaluation = new StockfishEvaluation(stringList[index + 1], Convert.ToInt32(stringList[index + 2]) * num2);
          }
        }
        if (stringList[0] != "bestmove")
          ++num1;
        else
          goto label_9;
      } else
        break;
    }
    throw new MaxTriesException();
    label_9:
      return evaluation;
  }
}