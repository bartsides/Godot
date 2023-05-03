public partial class Timer {
    public bool Active { get; set; }
    public double Amount { get; set; }
    public double Max { get; set; }
    public double Min { get; set; }

    public Timer(double max, double min = 0, double amount = 0, bool active = true) {
        Max = max;
        Min = min;
        Amount = amount;
        Active = active;
    }

    public bool Process(double delta) {
        Amount += delta;
        return Amount >= Min && Amount > Max;
    }

    public void Reset() => Amount = 0;
}