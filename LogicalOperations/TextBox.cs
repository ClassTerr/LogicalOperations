using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MathParserTestNS
{
    [ToolboxBitmap(typeof(TextBox))]
    public class TextBox : System.Windows.Forms.TextBox
    {
        public const int ECM_FIRST = 0x1500;
        public const int EM_SETCUEBANNER = ECM_FIRST + 1;
        private string _cueBannerText = String.Empty;

        private bool _showCueFocused;

        /// <summary>Gets or sets the cue text that is displayed on the TextBox control.</summary>
        [Description("Text that is displayed as Cue banner.")]
        [Category("Appearance")]
        [DefaultValue("")]
        public string CueBannerText
        {
            get => _cueBannerText;
            set
            {
                _cueBannerText = value;
                SetCueText(ShowCueFocused);
            }
        }

        [Browsable(false)]
        public new bool Multiline
        {
            get => base.Multiline;
            set => base.Multiline = false;
        }

        /// <summary>Gets or sets whether the Cue text should be displyed even when the control has keybord focus.</summary>
        /// <remarks>If true, the Cue text will disappear as soon as the user starts typing.</remarks>
        [Description("If true, the Cue text will be displayed even when the control has keyboard focus.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowCueFocused
        {
            get => _showCueFocused;
            set
            {
                _showCueFocused = value;
                SetCueText(value);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        private void SetCueText(bool showFocus)
        {
            SendMessage(Handle, EM_SETCUEBANNER, new IntPtr(showFocus ? 1 : 0), _cueBannerText);
        }
    }
}