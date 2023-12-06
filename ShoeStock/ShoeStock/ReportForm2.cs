using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoeStock
{
    public partial class ReportForm2 : Form
    {
        public ReportForm2()
        {
            InitializeComponent();
        }

        private void ReportForm2_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Shoes", con))
                {
                    DataSet ds = new DataSet();
                   
                    da.Fill(ds, "Shoes");
                    da.SelectCommand.CommandText = "SELECT * FROM Brands";
                    da.Fill(ds, "Brands");
                    da.SelectCommand.CommandText = "SELECT * FROM Stocks";
                    da.Fill(ds, "Stocks");
                    CrystalReport2 rpt = new CrystalReport2();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();

                }
            }
        }
    }
}
