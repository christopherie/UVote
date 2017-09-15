﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Web.Configuration;
using System.IO;

namespace UVote.Models
{
    public class DAO
    {
        SqlConnection connection;
        public string message = string.Empty;

        // Initialize the connection object
        public void Connection()
        {
            connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["connectionStringLocal"].ConnectionString);
        }

        #region Student
        public int Insert(StudentModel model)
        {
            int count = 0;
            SqlCommand cmd;
            string password;
            Connection();
            cmd = new SqlCommand("uspInsertStudent", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@studentNumber", model.StudentNumber);
            cmd.Parameters.AddWithValue("@firstName", model.FirstName);
            cmd.Parameters.AddWithValue("@lastName", model.LastName);
            cmd.Parameters.AddWithValue("@telephone", model.Telephone);
            cmd.Parameters.AddWithValue("@email", model.Email);
            cmd.Parameters.AddWithValue("@addressLine1", model.AddressLine1);
            cmd.Parameters.AddWithValue("@addressLine2", model.AddressLine2);
            cmd.Parameters.AddWithValue("@addressLine3", model.AddressLine3);
            cmd.Parameters.AddWithValue("@addressLine4", model.AddressLine4);
            cmd.Parameters.AddWithValue("@employeeId", model.EmployeeId);
            password = Crypto.HashPassword(model.Password);
            cmd.Parameters.AddWithValue("@password", password);

            try
            {
                connection.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return count;
        }
        #endregion

        #region Admin
        // Create an admin
        public int InsertAdmin(AdminModel adminModel)
        {
            int count = 0;
            SqlCommand cmd;
            string password;
            Connection();
            cmd = new SqlCommand("uspInsertAdmin", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@employeeId", adminModel.EmployeeId);
            cmd.Parameters.AddWithValue("@name", adminModel.Name);
            cmd.Parameters.AddWithValue("@email", adminModel.Email);
            password = Crypto.HashPassword(adminModel.Password);
            cmd.Parameters.AddWithValue("@password", password);

            try
            {
                connection.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return count;
        }

        // Login an admin
        public string AdminLogin(AdminModel adminModel)
        {
            string employeeId = null;
            string name = null;
            string password;
            SqlCommand cmd;
            SqlDataReader reader;
            Connection();
            cmd = new SqlCommand("uspAdminLogin", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", adminModel.Email);

            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    password = reader["Password"].ToString();
                    if (Crypto.VerifyHashedPassword(password, adminModel.Password))
                    {
                        employeeId = reader["EmployeeId"].ToString();
                        name = reader["Name"].ToString();
                    }
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return employeeId;
        }

        #endregion

        #region Campaign
        // Create a campaign
        public int InsertCampaign(CampaignModel campaignModel)
        {
            int count = 0;
            SqlCommand cmd;
            Connection();
            cmd = new SqlCommand("uspInsertCampaign", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@roleTitle", campaignModel.RoleTitle);
            cmd.Parameters.AddWithValue("@roleDetails", campaignModel.RoleDetails);
            cmd.Parameters.AddWithValue("@officeTerm", campaignModel.OfficeTerm);
            cmd.Parameters.AddWithValue("@campaignStart", campaignModel.CampaignStart);
            cmd.Parameters.AddWithValue("@campaignEnd", campaignModel.CampaignEnd);
            cmd.Parameters.AddWithValue("@employeeId", campaignModel.EmployeeId);

            try
            {
                connection.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }

            return count;
        }

        // Get all campaigns
        public List<Campaign> GetCampaigns()
        {
            List<Campaign> campaigns = new List<Campaign>();
            SqlDataReader reader;
            SqlCommand cmd;
            Connection();
            cmd = new SqlCommand("uspGetCampaigns", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Campaign campaign = new Campaign();
                    campaign.CampaignId = reader[0].ToString();
                    campaign.RoleTitle = reader[1].ToString();
                    campaign.RoleDetails = reader[2].ToString();
                    campaign.OfficeTerm = reader[3].ToString();
                    campaign.CampaignStart = reader[4].ToString();
                    campaign.CampaignEnd = reader[5].ToString();
                    campaign.EmployeeId = reader[6].ToString();
                    campaigns.Add(campaign);
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return campaigns;
        }

        public List<Campaign> GetCampaignDropDown()
        {
            List<Campaign> campaigns = new List<Campaign>();
            SqlDataReader reader;
            SqlCommand cmd;
            Connection();
            cmd = new SqlCommand("uspGetCampaignDropDown", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Campaign campaign = new Campaign();
                    campaign.CampaignId = reader[0].ToString();
                    campaign.RoleTitle = reader[1].ToString();
                    campaigns.Add(campaign);
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return campaigns;
        }
        #endregion

        #region Candidate
        public int InsertCandidate(CandidateModel candidateModel)
        {
            int count = 0;
            SqlCommand cmd;
            Connection();
            cmd = new SqlCommand("uspInsertCandidate", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@candidateId", candidateModel.CandidateId);
            cmd.Parameters.AddWithValue("@firstName", candidateModel.FirstName);
            cmd.Parameters.AddWithValue("@lastName", candidateModel.LastName);
            cmd.Parameters.AddWithValue("@manifesto", candidateModel.Manifesto);
            cmd.Parameters.AddWithValue("@imageUrl", candidateModel.ImageUrl);
            cmd.Parameters.AddWithValue("@previousHistory", candidateModel.PreviousHistory);
            cmd.Parameters.AddWithValue("@campaignId", candidateModel.CampaignId);
            cmd.Parameters.AddWithValue("@employeeId", candidateModel.EmployeeId);

            try
            {
                connection.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return count;
        }
        #endregion

        #region Voter
        // Login an voter
        public string VoterLogin(VoterModel voterModel)
        {
            string studentNumber = null;
            string password;
            SqlCommand cmd;
            SqlDataReader reader;
            Connection();
            cmd = new SqlCommand("uspVoterLogin", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@studentNumber", voterModel.StudentNumber);

            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    password = reader["Password"].ToString();
                    if (Crypto.VerifyHashedPassword(password, voterModel.Password))
                    {
                        studentNumber = reader["StudentNumber"].ToString();
                    }
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return studentNumber;
        }

        // Get all campaigns
        public List<Election> GetElections()
        {
            List<Election> elections = new List<Election>();
            SqlDataReader reader;
            SqlCommand cmd;
            Connection();
            cmd = new SqlCommand("uspGetElections", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Election election = new Election();
                    election.CampaignId = reader[0].ToString();
                    election.RoleTitle = reader[1].ToString();
                    election.RoleDetails = reader[2].ToString();
                    election.OfficeTerm = reader[3].ToString();
                    election.CampaignStart = reader[4].ToString();
                    election.CampaignEnd = reader[5].ToString();
                    elections.Add(election);
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return elections;
        }
        #endregion
    }
}