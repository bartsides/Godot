public class Timer {
    public bool Active { get; set; }
    public float Amount { get; set; }
    public float Max { get; set; }
    public float Min { get; set; }

    public Timer(float max, float min = 0, float amount = 0, bool active = true) {
        Max = max;
        Min = min;
        Amount = amount;
        Active = active;
    }

    public bool Process(float delta) {
        Amount += delta;
        return Amount >= Min && Amount > Max;
    }

    public void Reset() {
        Amount = 0;
    }
}