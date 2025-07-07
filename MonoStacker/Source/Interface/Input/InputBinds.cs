using Microsoft.Xna.Framework.Input;

namespace MonoStacker.Source.Interface.Input;

public struct InputBinds
{
    public Keys k_MovePieceLeft { get; set; } = Keys.Left;
    public Keys k_MovePieceRight { get; set; } = Keys.Right;
    public Keys k_RotateCw { get; set; } = Keys.Up;
    public Keys k_RotateCcw { get; set; } = Keys.Z;
    public Keys k_Rotate180 { get; set; }
    public Keys k_HardDrop { get; set; } = Keys.Space;
    public Keys k_FirmDrop { get; set; }
    public Keys k_SoftDrop { get; set; } = Keys.Down;
    public Keys k_Hold { get; set; } = Keys.LeftShift;

    public Keys k_RotateCwAlt { get; set; }
    public Keys k_RotateCcwAlt { get; set; }


    public Buttons b_MovePieceLeft { get; set; } = Buttons.DPadLeft;
    public Buttons b_MovePieceRight { get; set; } = Buttons.DPadRight;
    public Buttons b_RotateCw { get; set; } = Buttons.A;
    public Buttons b_RotateCcw { get; set; } = Buttons.B;
    public Buttons b_Rotate180 { get; set; }
    public Buttons b_HardDrop { get; set; } = Buttons.RightTrigger;
    public Buttons b_FirmDrop { get; set; }
    public Buttons b_SoftDrop { get; set; } = Buttons.DPadDown;
    public Buttons b_Hold { get; set; } = Buttons.LeftTrigger;
        
    public Buttons b_RotateCwAlt { get; set; }
    public Buttons b_RotateCcwAlt { get; set; }
    
    public InputBinds(){}
}