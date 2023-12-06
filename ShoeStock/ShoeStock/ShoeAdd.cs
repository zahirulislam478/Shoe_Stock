﻿using ShoeStock.Models;
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
    public partial class ShoeAdd : Form
    {
        string fileName = "", filePath="";
        List<Shoe> shoes = new List<Shoe>();
        public ShoeAdd()
        {
            InitializeComponent();
        }
        public MasterForm MasterForm { get; set; }
        private void ShoeAdd_Load(object sender, EventArgs e)
        {
            SetNewId(textBox1);
            LoadCombo();
        }

        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Brands", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox1.DataSource= dt;
                    DataTable dt1 = new DataTable();
                    da.SelectCommand.CommandText = "SELECT * FROM Models";
                    da.Fill(dt1);
                    comboBox2.DataSource = dt1;
                }
            }
        }

        private void SetNewId(TextBox t)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(ShoeId), 0) FROM Shoes", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    t.Text = $"{id + 1}";
                }
            }
        }

        private void ShoeAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.MasterForm.ShoesAdded(shoes);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Shoes 
                                            (ShoeId,  FirstIntroducedOn, Active, Picture, BrandId, ModelId) VALUES
                                            (@i, @f, @a,@p, @br, @md)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        
                        cmd.Parameters.AddWithValue("@f", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@br", comboBox1.SelectedValue);
                        cmd.Parameters.AddWithValue("@md", comboBox2.SelectedValue);
                        string ext = Path.GetExtension(this.filePath);
                        fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@p", fileName);


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                                shoes.Add(new Shoe
                                {
                                    ShoeId = int.Parse(textBox1.Text),
                                    
                                    FirstIntroducedOn = dateTimePicker1.Value,
                                    Active = checkBox1.Checked,
                                    Picture = fileName
                                  
                                });
                                SetNewId(textBox1);
                                
                                checkBox1.Checked = false;
                                dateTimePicker1.Value = DateTime.Now;
                                pictureBox1.Image = null;
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