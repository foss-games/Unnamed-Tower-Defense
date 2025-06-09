using System;
using Godot;

public partial class Greg : Node2D
{
    public int Speed = 50;
    public bool HasMouse = false;
    public bool IsHeld = false;
    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("Click"))
        {
            if (HasMouse && !IsHeld)
            {
                GD.Print("Grabbed");
                IsHeld = true;
            }
            if (IsHeld)
            {
                Vector2 mousePosition = GetGlobalMousePosition();
                GD.Print("Setting Position");
                GD.Print(mousePosition);
                GD.Print(GlobalPosition);
                GlobalPosition = GetGlobalMousePosition();
            }
        }
        else if (IsHeld)
        {
            GD.Print("Dropped");
            IsHeld = false;
        }
    }

    public void OnMouseEnter()
    {
        GD.Print("Mouse Entered");
        GD.Print(GetGlobalMousePosition());
        HasMouse = true;
    }
    public void OnMouseExit()
    {
        GD.Print("Mouse Exited");
        HasMouse = false;
    }
}