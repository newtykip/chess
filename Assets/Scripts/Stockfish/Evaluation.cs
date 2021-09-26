public class StockfishEvaluation
{
    public string Type { get; set; }

    public int Value { get; set; }

    public StockfishEvaluation()
    {
    }

    public StockfishEvaluation(string type, int value)
    {
        this.Type = type;
        this.Value = value;
    }
}