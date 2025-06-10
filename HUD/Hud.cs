using Godot;

public partial class Hud : Node2D
{
    public Label TimerText;
    public Label WaveText;
    public Label CreditText;
    public Label HPText;

    public int MaxWaves
    {
        get
        {
            return _maxWaves;
        }
        set
        {
            _maxWaves = value;
            SetWaves();
        }
    }

    public int CurrentWave
    {
        get
        {
            return _currentWave;
        }
        set
        {
            _currentWave = value;
            SetWaves();
        }
    }

    private int _hp;
    public int HP
    {
        get { return _hp; }
        set
        {
            _hp = value;
            UpdateHPText();
        }
    }
    private int _maxHP;
    public int MaxHP
    {
        get { return _maxHP; }
        set
        {
            _maxHP = value;
            UpdateHPText();
        }
    }

    private int _currentWave;
    private int _maxWaves;

    private int _minutes = 0;
    private int _seconds = 0;

    private Timer timer;

    public override void _Ready()
    {
        TimerText = GetNode<Label>("TimerText");
        WaveText = GetNode<Label>("WaveText");
        CreditText = GetNode<Label>("CreditText");
        HPText = GetNode<Label>("HPText");
        timer = GetNode<Timer>("HudTimer");
    }

    public void SetWaves()
    {
        WaveText.Text = "Wave " + _currentWave + "/" + _maxWaves;
    }

    public void SetTimer(int seconds)
    {
        _seconds = seconds % 60;
        _minutes = seconds / 60; //_minutes is int, so the decimal portion is lost automatically
        timer.Start();
        UpdateTimerText();
    }

    public void UpdateTimerText()
    {
        TimerText.Text = _minutes.ToString("D2") + ":" + _seconds.ToString("D2");
    }

    public void UpdateHPText()
    {
        HPText.Text = "HP " + _hp + "/" + _maxHP;
    }

    public void SetCredits(double credits)
    {
        CreditText.Text = "Credits " + credits.ToString();
    }

    public void OnTimerTimeout()
    {
        if (_seconds == 0 && _minutes == 0)
        {
            timer.Stop();
            return;
        }

        if (_seconds == 0)
        {
            _seconds = 59;
            _minutes--;
            return;
        }

        _seconds--;
        UpdateTimerText();
    }
}