using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;

namespace Ontranet
{
    public partial class copyMemberEmploymentData : System.Web.UI.Page
    {

        public int departmentId = -1;


        public int DepartmentId
        {
            get
            {
                return departmentId;
            }
            set
            {
                departmentId = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String[] memberProperties = new string[] { "ansttelsesstedHovedbeskftigelseMember", "adressePArbejdsstedMember", "postnrEmploymentMember", "byNavnEmploymentMember", "employmentRegionMember", "employmentWebsiteMember", "showOnMap", "employmentCVRMember" }; ;

            string dbTable = "ontranetMemberDepartment2";

            foreach (Member m in Member.GetAll)
            {
                MemberType demoMemberType = MemberType.GetByAlias("Distister");

                if (m.ContentType.Equals(demoMemberType))
                {
                    SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
                    SqlCommand cmd = new SqlCommand("dbo.getmember " + m.Id, con);
                    SqlDataReader reader;
                    con.Open();
                    int i = 0;

                    using (reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string sqlQ = "";
                             

                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                if (memberProperties.Contains(reader["MemberFieldAlias"].ToString()))
                                {
                                    if (i == 0)
                                    {
                                        if (reader["MemberFieldAlias"].ToString() == "employmentRegionMember" || reader["MemberFieldAlias"].ToString() == "showOnMap")
                                        {
                                            int showonmap = 0;
                                            if (reader["MemberFieldAlias"].ToString() == "showOnMap") {
                                                if (reader["MemberData"].ToString() == "1") {
                                                    showonmap = 1;
                                                }
                                                sqlQ = "INSERT INTO dbo." + dbTable + " (" + GetCorrespondingField(reader["MemberFieldAlias"].ToString()) + ",memberId) VALUES(" + showonmap + "," + m.Id + "); SELECT @@IDENTITY AS LastID";
                                            }
                                            else {
                                                sqlQ = "INSERT INTO dbo." + dbTable + " (" + GetCorrespondingField(reader["MemberFieldAlias"].ToString()) + ",memberId) VALUES(" + reader["MemberData"].ToString() + "," + m.Id + "); SELECT @@IDENTITY AS LastID";
                                            }

                                        }
                                        else
                                        {
                                            sqlQ = "INSERT INTO dbo." + dbTable + " (" + GetCorrespondingField(reader["MemberFieldAlias"].ToString()) + ",memberId) VALUES('" + reader["MemberData"].ToString() + "'," + m.Id + ");SELECT @@IDENTITY AS LastID";
                                        }
                                       
                                    }
                                    else
                                    {
                                        if (reader["MemberFieldAlias"].ToString() == "employmentRegionMember" || reader["MemberFieldAlias"].ToString() == "showOnMap")
                                        {
                                            int showonmap = 0;
                                            if (reader["MemberFieldAlias"].ToString() == "showOnMap") {
                                                if (reader["MemberData"].ToString() == "1") {
                                                    showonmap = 1;
                                                }
                                                 sqlQ = "UPDATE  dbo." + dbTable + " SET " + GetCorrespondingField(reader["MemberFieldAlias"].ToString()) + "=" + showonmap + " WHERE memberId=" + m.Id;
                                            }
                                            else {
                                                sqlQ = "UPDATE  dbo." + dbTable + " SET " + GetCorrespondingField(reader["MemberFieldAlias"].ToString()) + "=" + reader["MemberData"].ToString() + " WHERE memberId=" + m.Id;
                                            }
                                        }
                                        else
                                        {
                                            sqlQ = "UPDATE  dbo." + dbTable + " SET " + GetCorrespondingField(reader["MemberFieldAlias"].ToString()) + "='" + reader["MemberData"].ToString() + "' WHERE memberId=" + m.Id;
                                        }
                                        
                                    }

                                    try
                                    {

                                        SqlConnection con3 = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
                                        using (SqlCommand cmd3 = new SqlCommand(sqlQ, con3))
                                        {
                                            con3.Open();
                                            cmd3.ExecuteNonQuery();
                                            cmd3.Dispose();
                                            con3.Close();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Response.Write(ex.Message + " " + sqlQ + "<br>");
                                    }

                                    

                                    i++;
                                }
                            }
                        }
                    }
                    reader.Dispose();
                    cmd.Dispose();
                    con.Close();

                    //try
                    //{
                    //    SqlConnection con4 = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
                    //    con4.Open();
                    //    SqlCommand cmd4 = new SqlCommand(" update [fakd_dk].[dbo].[ontranetMemberDepartment2] set showOnMap = 0 where showonmap is null", con4);
                    //    cmd4.ExecuteNonQuery();
                    //    cmd4.Dispose();
                    //    con4.Dispose();
                    //}
                    //catch (Exception ex)
                    //{
                    //}


                }
            }


            SqlConnection con6 = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd6 = new SqlCommand("SELECT id,memberId FROM dbo." + dbTable, con6);
            SqlDataReader reader6;
            con6.Open();

            using (reader6 = cmd6.ExecuteReader())
            {
                if (reader6.HasRows)
                {
                    while (reader6.Read())
                    {

                        try
                        {

                            SqlConnection con5 = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
                            using (SqlCommand cmd5 = new SqlCommand("INSERT INTO dbo.ontranetRelMembersDepartments (MemberId,DepartmentId) VALUES(" + reader6.GetSqlInt32(1) + " ," + reader6.GetSqlInt32(0) + ")", con5))
                            {
                                con5.Open();
                                cmd5.ExecuteNonQuery();
                                cmd5.Dispose();
                                con5.Close();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            reader6.Dispose();
            cmd6.Dispose();
            con6.Close();


            
        }

        private string GetCorrespondingField(string val)
        {
            string returnValue = "";

            switch (val)
            {
                case "ansttelsesstedHovedbeskftigelseMember":
                    returnValue = "afdelingsnavn";
                    break;
                case "adressePArbejdsstedMember":
                    returnValue = "adresse";
                    break;
                case "postnrEmploymentMember":
                    returnValue = "postnr";
                    break;
                case "byNavnEmploymentMember":
                    returnValue = "bynavn";
                    break;
                case "employmentPhoneMember":
                    returnValue = "telefon";
                    break;
                case "arbejdsEmailMember":
                    returnValue = "email";
                    break;
                case "employmentWebsiteMember":
                    returnValue = "hjemmeside";
                    break;
                case "showOnMap":
                    returnValue = "showOnMap";
                    break;
                case "employmentRegionMember":
                    returnValue = "region";
                    break;
                case "specialtiesMember":
                    returnValue = "specialer";
                    break;
                case "employmentCVRMember":
                    returnValue = "CVR";
                    break;
                    
            }
            return returnValue;
        }
    }
}