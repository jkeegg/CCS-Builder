using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCS_Builder
{
    public partial class FromDetail : Form
    {
        public FromDetail(object o)
        {
            InitializeComponent();
            TextBox srcBox = o as TextBox;
            textBoxDetail.DataBindings.Add("Text", srcBox, "Text", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void FormDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void textBoxDetail_TextChanged(object sender, EventArgs e)
        {
            this.textBoxDetail.SelectionStart = this.textBoxDetail.TextLength;
            this.textBoxDetail.ScrollToCaret();
        }

        private void textBoxDetail_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Hide();
        }
    }
}
