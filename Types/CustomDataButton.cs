using Godot;

namespace FOSSGames
{
    public partial class CustomDataButton : Button
    {
        [Signal]
        public delegate void MyPressedEventHandler(Variant customData);
        public Variant CustomData;
        public override void _Ready()
        {
            base._Ready();
            Pressed += PressedHandler;
        }
        private void PressedHandler()
        {
            EmitSignal(SignalName.MyPressed, CustomData);
        }
    }
}