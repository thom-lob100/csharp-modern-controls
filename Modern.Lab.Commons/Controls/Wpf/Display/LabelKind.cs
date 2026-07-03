namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>Typography role of a label, mapped to the token type ramp.</summary>
    public enum LabelKind
    {
        /// <summary>Body text. Regular weight, primary text color.</summary>
        Body,

        /// <summary>Section title. Title size, SemiBold, primary text color.</summary>
        Title,

        /// <summary>Field label. Label size, SemiBold, secondary text color.</summary>
        Label,

        /// <summary>Helper/caption text. Helper size, Regular, secondary text color.</summary>
        Helper
    }
}
