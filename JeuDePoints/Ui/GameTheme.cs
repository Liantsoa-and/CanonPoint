namespace JeuDePoints.Ui
{
    internal static class GameTheme
    {
        public static readonly Color WindowBackground = Color.FromArgb(241, 245, 252);
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceAlt = Color.FromArgb(231, 238, 249);
        public static readonly Color Border = Color.FromArgb(206, 217, 235);

        public static readonly Color Primary = Color.FromArgb(35, 99, 200);
        public static readonly Color Accent = Color.FromArgb(46, 160, 120);
        public static readonly Color Danger = Color.FromArgb(188, 71, 73);
        public static readonly Color Neutral = Color.FromArgb(80, 93, 113);

        public static readonly Color Player1 = Color.FromArgb(44, 112, 224);
        public static readonly Color Player2 = Color.FromArgb(218, 68, 89);

        public static readonly Font TitleFont = new Font("Bahnschrift SemiBold", 20, FontStyle.Regular);
        public static readonly Font SectionFont = new Font("Bahnschrift SemiBold", 12, FontStyle.Regular);
        public static readonly Font UiFont = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font UiFontBold = new Font("Segoe UI Semibold", 10, FontStyle.Regular);

        public static void StylePrimaryButton(Button button)
        {
            StyleButton(button, Primary, Color.White);
        }

        public static void StyleDangerButton(Button button)
        {
            StyleButton(button, Danger, Color.White);
        }

        public static void StyleNeutralButton(Button button)
        {
            StyleButton(button, Neutral, Color.White);
        }

        public static void StyleAccentButton(Button button)
        {
            StyleButton(button, Accent, Color.White);
        }

        private static void StyleButton(Button button, Color background, Color foreground)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = background;
            button.ForeColor = foreground;
            button.Font = UiFontBold;
            button.Cursor = Cursors.Hand;
        }

        public static void StyleCard(Panel panel)
        {
            panel.BackColor = Surface;
            panel.Padding = new Padding(16);
            panel.Margin = new Padding(8);
        }
    }
}
