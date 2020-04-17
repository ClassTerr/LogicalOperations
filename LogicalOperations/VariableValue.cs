using System.Windows.Forms;

namespace LogicalOperations
{
    public partial class VariableValue : UserControl
    {
        public VariableValue()
        {
            InitializeComponent();
        }

        public string Variable
        {
            get => txtVariable.Text;
            set => txtVariable.Text = value;
        }

        public string Value
        {
            get => txtValue.Text;
            set => txtValue.Text = value;
        }
    }
}