using System.Windows.Forms;

namespace MathParserTestNS
{
    public partial class VariableValue : UserControl
    {
        public VariableValue()
        {
            InitializeComponent();
        }

        public string Variable 
        { 
            get { return txtVariable.Text; }
            set { txtVariable.Text = value; }
        }

        public string Value 
        { 
            get { return txtValue.Text; }
            set { txtValue.Text = value; }
        }
    }
}
