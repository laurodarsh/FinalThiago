﻿using FinalThiago.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalThiago.Forms
{
	public partial class UserProfileDetailsForm : Form
	{
		string name = "";
		bool active = false;

        string connectionString = "workstation id=StockControl.mssql.somee.com;packet size=4096;user id=levelupacademy_SQLLogin_1;pwd=3wwate8gu1;data source=StockControl.mssql.somee.com;persist security info=False;initial catalog=StockControl";

        public UserProfileDetailsForm()
		{
			InitializeComponent();
		}

        public UserProfileDetailsForm(int idUserProfile)
        {

            InitializeComponent();

            lblId.Text = idUserProfile.ToString(); 

            SqlConnection sqlConnect = new SqlConnection(connectionString);

            if (!string.IsNullOrEmpty(lblId.Text))
            {
                try
                {
                    
                    sqlConnect.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM USER_PROFILE WHERE ID = @id", sqlConnect);

                    cmd.Parameters.Add(new SqlParameter("@id", idUserProfile));

                    UserProfile userProfile = new UserProfile(); 

                    using (SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        while (reader.Read())
                        {
                            userProfile.Id = Int32.Parse(reader["ID"].ToString());
                            userProfile.Name = reader["NAME"].ToString();
                            userProfile.Active = bool.Parse(reader["ACTIVE"].ToString());
                        }
                    }

                    tbxName.Text = userProfile.Name;
                    cbxActive.Checked = userProfile.Active;
                }
                catch (Exception EX)
                {
                    MessageBox.Show("Não foi carregado com sucesso");
                    throw;
                }
                finally
                {
                    //Fechar
                    sqlConnect.Close();
                }
            }
        }

		#region PbxClick

		private void pbxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbxSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblId.Text))
            {
                SqlConnection sqlConnect = new SqlConnection(connectionString);
                try
                {
                    GetData();
                    UserProfile userprofile = new UserProfile(name, active);

                    sqlConnect.Open();
                    string sql = "INSERT INTO USER_PROFILE(NAME, ACTIVE) VALUES (@name, @active)";

                    SqlCommand cmd = new SqlCommand(sql, sqlConnect);

                    cmd.Parameters.Add(new SqlParameter("@name", userprofile.Name));
                    cmd.Parameters.Add(new SqlParameter("@active", userprofile.Active));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Adicionado com sucesso!");
					Log.SaveLog(sqlConnect,"Perfil Criado", DateTime.Now, "Criação");

					CleanData();


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao adicionar categoria!" + ex.Message);
                    CleanData();
                }
                finally
                {
                    sqlConnect.Close();
                }
            }
            else
            {
                SqlConnection sqlConnect = new SqlConnection(connectionString);

                try
                {
                    GetData();
                    sqlConnect.Open();
                    string sql = "UPDATE USER_PROFILE SET NAME = @name, ACTIVE = @active WHERE ID = @id";

                    SqlCommand cmd = new SqlCommand(sql, sqlConnect);

                    cmd.Parameters.Add(new SqlParameter("@name", name));
                    cmd.Parameters.Add(new SqlParameter("@active", active));
                    cmd.Parameters.Add(new SqlParameter("@id", lblId.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Altereções salvas com sucesso!");
					Log.SaveLog(sqlConnect,"Perfil Editado", DateTime.Now, "Edição");
				}
                catch (Exception Ex)
                {
                    MessageBox.Show("Erro ao editar este perfil!" + "\n\n" + Ex.Message);
                    throw;
                }
                finally
                {
                    sqlConnect.Close();

                    this.Close();
                }
            }
        }

        private void pbxDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblId.Text)) //-----
            {
                SqlConnection sqlConnect = new SqlConnection(connectionString);

                try
                {
                    sqlConnect.Open();
                    string sql = "UPDATE USER_PROFILE SET ACTIVE = @active WHERE ID = @id";

                    SqlCommand cmd = new SqlCommand(sql, sqlConnect);

                    cmd.Parameters.Add(new SqlParameter("@id", lblId.Text));
                    cmd.Parameters.Add(new SqlParameter("@active", false));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Perfil inativo!");
					Log.SaveLog(sqlConnect,"Perfil Excluído", DateTime.Now, "Excluir");
				}
				catch (Exception Ex)
                {
                    MessageBox.Show("Erro ao desativar este perfil!" + "\n\n" + Ex.Message);
                    throw;
                }
                finally
                {
                    sqlConnect.Close();
                }
                }
        }

		#endregion

		#region Functions

		void GetData()
		{
			name = tbxName.Text;
			if (cbxActive.Checked)
			{
				active = true;
			}
			else
			{
				active = false;
			}
		}

		void CleanData()
		{
			tbxName.Text = "";
			cbxActive.Checked = false;
		}

		#endregion
	}
}
