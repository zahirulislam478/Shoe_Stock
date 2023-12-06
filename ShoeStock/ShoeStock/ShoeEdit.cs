using ShoeStock.Models;
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
    public partial class ShoeEdit : Form
    {
        string oldPath = "", filePath = "", fileName = "";
        Shoe shoe;
        public ShoeEdit()
        {
            InitializeComponent();
        }
        public int EditId { get; set; }
        public MasterForm MasterForm { get; set; }

        private void ShoeEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(shoe!= null)
                this.MasterForm.ShoeUpdated(shoe);
        }

        private void ShoeEdit_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.EditId.ToString();
            LoadCombo();
            using(SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Shoes WHERE ShoeId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.EditId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        
                        checkBox1.Checked = dr.GetBoolean(dr.GetOrdinal("Active"));
                        dateTimePicker1.Value = dr.GetDateTime(dr.GetOrdinal("FirstIntroducedOn"));
                        oldPath = dr.GetString(dr.GetOrdinal("Picture"));
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(dr.GetOrdinal("Picture"))));
                        comboBox1.SelectedValue= dr.GetInt32(dr.GetOrdinal("BrandId"));
                        comboBox2.SelectedValue = dr.GetInt32(dr.GetOrdinal("ModelId"));
                    }
                    con.Close();
                }
            }
        }
        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Brands", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox1.DataSource = dt;
                    DataTable dt1 = new DataTable();
                    da.SelectCommand.CommandText = "SELECT * FROM Models";
                    da.Fill(dt1);
                    comboBox2.DataSource = dt1;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                con.Open();
                using(SqlTransaction tran = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand(@"UPDATE Shoes SET 
                                    FirstIntroducedOn=@f, Active=@a, Picture=@p, BrandId=@br, ModelId=@md
                                    WHERE ShoeId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        //cmd.Parameters.AddWithValue("@m", textBox2.Text);
                        cmd.Parameters.AddWithValue("@f", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@br", comboBox1.SelectedValue);
                        cmd.Parameters.AddWithValue("@md", comboBox2.SelectedValue);
                        if (oldPath == "")
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@p", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@p", oldPath);
                        }
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                                shoe = new Shoe { 
                                ShoeId = EditId,
                                
                                FirstIntroducedOn= dateTimePicker1.Value,
                                Active = checkBox1.Checked,
                                Picture = oldPath == "" ? fileName: oldPath
                                };
                                
                               

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
