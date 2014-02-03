using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Subgurim.Controles;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using System.Configuration;
using System.Xml.XPath;
using System.Text;
using System.Net;
using System.IO;
using System.Data.SqlClient;

namespace Ontranet
{
    public partial class GMap3 : System.Web.UI.UserControl
    {
        private int region = 0;
        public int Region
        {
            get { return region; }
            set { region = value; }
        }

        private bool showMembersInList = false;
        public bool ShowMembersInList
        {
            get { return showMembersInList; }
            set { showMembersInList = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                  GMap1.Key = ConfigurationSettings.AppSettings["GMapKey"].ToString();
                GMap1.Add(new GControl(GControl.preBuilt.SmallMapControl));
                GMap1.Add(new GControl(GControl.preBuilt.MapTypeControl));

                string lat, lng;
                int izoom;
                switch (region)
                {
                    case 60:
                        lat = "55.8130390";
                        lng = "12.3707910";
                        izoom = 9;
                        break;
                    case 61:
                        lat = "55.4429520";
                        lng = "11.7913720";
                        izoom = 9;
                        break;
                    case 62:
                        lat = "55.2707841";
                        lng = "9.9019481";
                        izoom = 8;
                        break;
                    case 63:
                        lat = "56.0890730";
                        lng = "9.7666410";
                        izoom = 8;
                        break;
                    case 64:

                        lat = "57.0964510";
                        lng = "9.5221590";
                        izoom = 8;
                        break;
                    default:

                        lat = "55.437000";
                        lng = "10.411000";
                        izoom = 6;
                        break;
                }
                InitializeGoogleMap(lat, lng, false, izoom);
            }
        }

        enum elocationType
        {
            lat,
            lng
        }

        private string getValue(string Address, elocationType stype)
        {
            string output = string.Empty;

            try
            {
                String LatLng = ProcessAddress(Address, stype);

                if (stype == elocationType.lat)
                {
                    output = LatLng;
                }
                else
                {
                    output = LatLng;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + " " + ex.InnerException.ToString() + " " + ex.StackTrace);
            }
            return output;
        }

        private string ProcessAddress(string FullAddress, elocationType ltype)
        {
            string latVal = string.Empty;
            string lngVal = string.Empty;
            string returnVal = string.Empty;

            string url = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + FullAddress + "&sensor=false";
            WebResponse response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                response = request.GetResponse();
                if (response != null)
                {
                    XPathDocument document = new XPathDocument(response.GetResponseStream());
                    XPathNavigator navigator = document.CreateNavigator();

                    // get response status
                    XPathNodeIterator statusIterator = navigator.Select("/GeocodeResponse/status");
                    while (statusIterator.MoveNext())
                    {
                        if (statusIterator.Current.Value != "OK")
                        {
                            // Console.WriteLine("Error: response status = '" + statusIterator.Current.Value + "'");
                            //return;
                        }
                    }

                    // get results
                    XPathNodeIterator resultIterator = navigator.Select("/GeocodeResponse/result");
                    while (resultIterator.MoveNext())
                    {
                        XPathNodeIterator formattedAddressIterator = resultIterator.Current.Select("formatted_address");
                        while (formattedAddressIterator.MoveNext())
                        {
                            //Console.WriteLine(" formatted_address: " + formattedAddressIterator.Current.Value);
                        }

                        XPathNodeIterator geometryIterator = resultIterator.Current.Select("geometry");
                        while (geometryIterator.MoveNext())
                        {
                            // Console.WriteLine(" geometry: ");
                            XPathNodeIterator locationIterator = geometryIterator.Current.Select("location");
                            while (locationIterator.MoveNext())
                            {
                                //Console.WriteLine("     location: ");
                                XPathNodeIterator latIterator = locationIterator.Current.Select("lat");
                                while (latIterator.MoveNext())
                                {
                                    latVal = latIterator.Current.Value;
                                }

                                XPathNodeIterator lngIterator = locationIterator.Current.Select("lng");
                                while (lngIterator.MoveNext())
                                {
                                    lngVal = lngIterator.Current.Value;
                                }
                            }

                            XPathNodeIterator locationTypeIterator = geometryIterator.Current.Select("location_type");

                            while (locationTypeIterator.MoveNext())
                            {
                                // Console.WriteLine("         location_type: " + locationTypeIterator.Current.Value);
                            }
                        }
                    }
                }
            }
            catch 
            {
                // Console.WriteLine(ex.Message);
            }
            finally
            {
                // Console.WriteLine("Clean up");
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

            switch (ltype)
            {
                case elocationType.lat:
                    returnVal = latVal;
                    break;
                case elocationType.lng:
                    returnVal = lngVal;
                    break;
            }
            return returnVal;
        }

        protected void DropDownListRegionsLoad()
        {
            try
            {
                int categoryDataTypeNodeID = 1751;
                XPathNodeIterator preValueRootElementIterator = umbraco.library.GetPreValues(categoryDataTypeNodeID);
                preValueRootElementIterator.MoveNext();
                XPathNodeIterator preValueIterator = preValueRootElementIterator.Current.SelectChildren("preValue", "");
                ListItemCollection categoryListItems = new ListItemCollection();
                while (preValueIterator.MoveNext())
                {
                    categoryListItems.Add(new ListItem(" " + preValueIterator.Current.Value, preValueIterator.Current.GetAttribute("id", "")));
                }

                DropDownList2.DataSource = categoryListItems;
                DropDownList2.DataTextField = "Text";
                DropDownList2.DataValueField = "Value";
                DropDownList2.DataBind();
                DropDownList2.Items.Insert(0, new ListItem("Alle regioner...", "-1"));
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + " " + ex.InnerException.ToString() + " " + ex.StackTrace + " " + ex.Source);
            }
        }

        protected void filterByRegion()
        {
            try
            {
                string region = DropDownList2.SelectedItem.Value.ToString();
                string regionText = DropDownList2.SelectedItem.Text.ToString();
                string regionArea = regionText.Replace("Region ", "");

                double dLatitude = Convert.ToDouble(getValue(regionArea + ", Danmark", elocationType.lat), new System.Globalization.CultureInfo("en-US", false));
                double dLongitude = Convert.ToDouble(getValue(regionArea + ", Danmark", elocationType.lng), new System.Globalization.CultureInfo("en-US", false));
                GLatLng latlng = new GLatLng(dLatitude, dLongitude);

                GMap1.setCenter(latlng);

                switch (region)
                {
                    case "60":
                        GMap1.GZoom = 9;
                        break;
                    case "61":
                        GMap1.GZoom = 8;
                        break;
                    case "62":
                        GMap1.GZoom = 8;
                        break;
                    default:
                        GMap1.GZoom = 8;
                        break;
                }
                GMap1.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + " " + ex.InnerException.ToString() + " " + ex.StackTrace);
            }
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(DropDownList2.SelectedIndex == 0))
            {
                filterByRegion();
            }
            else
            {
                Page.Response.Redirect("/forside/selvstaendig/find-din-lokale-diaetist/");
            }
        }

        private void addMembers()
        {

            StringBuilder sblist = new StringBuilder();

            string sqlSelect = "";
            if (region == 0)
            {
                sqlSelect = "SELECT [id],[afdelingsnavn],[adresse],[postnr],[bynavn],[telefon],[email],[hjemmeside],[showOnMap],[region],[memberId],[specialer],[CVR],[hjemmeside2],[latitude],[longitude] FROM dbo.ontranetMemberDepartment2 where showOnMap=1 ORDER BY afdelingsnavn";

            }
            else
            {

                sqlSelect = "SELECT [id],[afdelingsnavn],[adresse],[postnr],[bynavn],[telefon],[email],[hjemmeside],[showOnMap],[region],[memberId],[specialer],[CVR],[hjemmeside2],[latitude],[longitude] FROM dbo.ontranetMemberDepartment2 WHERE [region]=" + region + " AND showOnMap=1 ORDER BY afdelingsnavn";

            }

            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader reader;
            con.Open();

            using (reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
        //               // bool showOnMap = reader.GetSqlBoolean(8).Value;
                        int memberId = reader.GetSqlInt32(10).Value;

                        string memberAthorized = getMemberPropertyValue(memberId, "memberAthorized");

                        if (memberAthorized == "1")
                        {

                           // int RegionId = 0;
                            string lat = "";
                            string lng = "";


                            if (!string.IsNullOrEmpty(reader["latitude"].ToString())) { lat = reader["latitude"].ToString(); }
                            if (!string.IsNullOrEmpty(reader["longitude"].ToString())) { lng = reader["longitude"].ToString(); }

                            string adresseMember = "";
                            string postnrMember = "";
                            string byNavnMember = "";

                            StringBuilder sb = new StringBuilder();

                            sb.Append("<div style=\"text-align:left; \">");
                            sblist.Append("<div class=\"member\">");

        //                   // string regioname = getRegion(region);


                            if (reader["afdelingsnavn"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["afdelingsnavn"].ToString()))
                                {
                                    sb.Append("<b>" + reader["afdelingsnavn"].ToString() + "</b><br/>");
                                    sblist.Append("<h2>" + reader["afdelingsnavn"].ToString() + "</h2>");
                                }
                            }

                            if (reader["adresse"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["adresse"].ToString()))
                                {
                                    adresseMember = reader["adresse"].ToString();
                                    sb.Append(adresseMember + "<br/>");
                                    sblist.Append(adresseMember + "<br/>");
                                }
                            }

                            if (reader["postnr"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["postnr"].ToString()))
                                {
                                    postnrMember = reader["postnr"].ToString();
                                    sb.Append(postnrMember + " ");
                                    sblist.Append(postnrMember + " ");
                                }
                            }

                            if (reader["bynavn"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["bynavn"].ToString()))
                                {
                                    byNavnMember = reader["bynavn"].ToString();
                                    sb.Append(byNavnMember + "<br/>");
                                    sblist.Append(byNavnMember + "<br/>");
                                }
                                else
                                {
                                    sb.Append("<br/>");
                                    sblist.Append("<br/>");
                                }
                            }

                            if (reader["hjemmeside"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["hjemmeside"].ToString()))
                                {
                                    string website = reader["hjemmeside"].ToString();
                                    if (!website.StartsWith("http://"))
                                    {
                                        website = "http://" + website;
                                    }
                                    if (!string.IsNullOrEmpty(reader["hjemmeside"].ToString()))
                                        sb.Append("<a href=\"" + website + "\" target=\"_blank\">" + reader["hjemmeside"].ToString() + "</a><br/><br/>");
                                    sblist.Append("<b>Hjemmeside:</b> <a href=\"" + website + "\" target=\"_blank\">" + reader["hjemmeside"].ToString() + "</a><br/>");
                                }
                            }

                            sb.Append("</div>");

                            if (reader["hjemmeside2"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["hjemmeside2"].ToString()))
                                {
                                    string website = reader["hjemmeside2"].ToString();
                                    if (!website.StartsWith("http://"))
                                    {
                                        website = "http://" + website;
                                    }
                                    if (!string.IsNullOrEmpty(reader["hjemmeside2"].ToString()))
                                        sblist.Append("<b>Hjemmeside 2:</b> <a href=\"" + website + "\" target=\"_blank\">" + reader["hjemmeside2"].ToString() + "</a><br/>");
                                }
                            }

                            if (reader["CVR"] != null)
                            {
                                if (!string.IsNullOrEmpty(reader["CVR"].ToString()))
                                {
                                    sblist.Append("<b>CVR:</b> " + reader["CVR"].ToString() + "<br/>");
                                }
                            }


                            StringBuilder sbMember = new StringBuilder();
                            SqlConnection con2 = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
                            SqlCommand cmd2 = new SqlCommand("SELECT MemberId FROM dbo.ontranetRelMembersDepartments WHERE DepartmentId=" + reader["id"].ToString(), con2);
                            SqlDataReader reader2;
                            con2.Open();

                            using (reader2 = cmd2.ExecuteReader())
                            {
                                if (reader2.HasRows)
                                {
                                    while (reader2.Read())
                                    {

                                        int _MemberId = reader2.GetSqlInt32(0).Value;
                                        string employmentWorkTypeNameMember = getMemberPropertyValue(_MemberId, "employmentWorkTypeNameMember");
                                        string fornavnMember = getMemberPropertyValue(_MemberId, "fornavnMember");
                                        string efternavnMember = getMemberPropertyValue(_MemberId, "efternavnMember");
                                        string frdiguddanetMember = getMemberPropertyValue(_MemberId, "frdiguddanetMember");
                                        string autorisationsIDMember = getMemberPropertyValue(_MemberId, "autorisationsIDMember");
                                        string arbejdsEmailMember = getMemberPropertyValue(_MemberId, "arbejdsEmailMember");
                                        string employmentPhoneMember = getMemberPropertyValue(_MemberId, "employmentPhoneMember");
                                        string specialtiesMember = getMemberPropertyValue(_MemberId, "specialtiesMember");

                                        sbMember.Append("<div class=\"singlemember\">");
                                        sbMember.Append("<b>");
                                        if (!string.IsNullOrEmpty(employmentWorkTypeNameMember))
                                        {
                                            sbMember.Append(employmentWorkTypeNameMember + " ");
                                        }

                                        if (!string.IsNullOrEmpty(fornavnMember))
                                        {
                                            sbMember.Append(fornavnMember + " ");
                                        }

                                        if (!string.IsNullOrEmpty(efternavnMember))
                                        {
                                            sbMember.Append(efternavnMember + "<br/>");
                                        }
                                        else
                                        {
                                            sbMember.Append("<br/>");
                                        }
                                        sbMember.Append("</b>");

                                        if (!string.IsNullOrEmpty(employmentPhoneMember))
                                        {
                                            sbMember.Append("<b>Telefon:</b> " + employmentPhoneMember + "<br/>");
                                        }

                                        if (!string.IsNullOrEmpty(arbejdsEmailMember))
                                        {
                                            sbMember.Append("<b>Email:</b> <a href=\"mailto:" + arbejdsEmailMember + "\">" + arbejdsEmailMember + "</a><br/>");
                                        }

                                        if (!string.IsNullOrEmpty(specialtiesMember))
                                        {
                                            sbMember.Append("<b>Specialer:</b> " + specialtiesMember + "<br/>");
                                        }

                                        if (!string.IsNullOrEmpty(frdiguddanetMember))
                                        {
                                            sbMember.Append("<b>Færdiguddannet år:</b> " + frdiguddanetMember + "<br/>");
                                        }

                                        if (!string.IsNullOrEmpty(autorisationsIDMember))
                                        {
                                            sbMember.Append("<b>AutorisationsID:</b> " + autorisationsIDMember + "<br/>");
                                        }
                                        sbMember.Append("</div>");
                                    }
                                }
                            }

                           
                            reader2.Dispose();
                            cmd2.Dispose();
                            con2.Close();

                            double dLatitude = 0;
                            double dLongitude = 0;

                            if (!(lat == "0") && !(lng == "0"))
                            {
                                dLatitude = Convert.ToDouble(lat, new System.Globalization.CultureInfo("en-US", false));
                                dLongitude = Convert.ToDouble(lng, new System.Globalization.CultureInfo("en-US", false));
                            }
                            else
                            {
                                string addressen = string.Empty;

                                if (!string.IsNullOrEmpty(adresseMember))
                                {
                                    addressen = adresseMember;
                                }

                                if (!string.IsNullOrEmpty(postnrMember))
                                {
                                    addressen = addressen + "," + postnrMember;
                                }

                                if (!string.IsNullOrEmpty(byNavnMember))
                                {
                                    addressen = addressen + " " + byNavnMember;
                                }

                                addressen = addressen + ",Danmark";

                                if (!string.IsNullOrEmpty(addressen)) {
                                    try
                                    {
                                        dLatitude = Convert.ToDouble(getValue(addressen, elocationType.lat), new System.Globalization.CultureInfo("en-US", false));
                                    }
                                    catch 
                                    {
                                    }

                                    try
                                    {
                                        dLongitude = Convert.ToDouble(getValue(addressen, elocationType.lng), new System.Globalization.CultureInfo("en-US", false));
                                    }
                                    catch
                                    {
                                    }
                                }                             
                            }

                            if (region > 0)
                            {
                                sb.Append("<i>" + umbraco.library.GetDictionaryItem("Map.ExplanationText") + "</i>");
                            }


        //                    ///TODO: save latitude and longitude
        //                    ///
                            if (!(dLatitude == 0) && !(dLongitude == 0))
                            {
                                sblist.Append(sbMember.ToString());
                                GMarker marker = new GMarker(new GLatLng(dLatitude, dLongitude));
                                GInfoWindow window = new GInfoWindow(marker, sb.ToString(), false);
                                //GMap1.addInfoWindow(window);
                                GMap1.Add(window);

                                //GMarker marker = new GMarker(latlng);
                                //  GInfoWindowTabs iwTabs = new GInfoWindowTabs();
                                //    iwTabs.gMarker = marker;

                                //   System.Collections.Generic.List<GInfoWindowTab> tabs = new System.Collections.Generic.List<GInfoWindowTab>();

                                //    tabs.Add(new GInfoWindowTab("Info", sb.ToString()));
                                //   // tabs.Add(new GInfoWindowTab("Info2", ""));
                                ////////    if (sbTab2.Length > 0)
                                ////////    {
                                ////////        tabs.Add(new GInfoWindowTab("Klinik", "<div style=\"width:200px; float:left; padding-left:5px\">" + sbTab2.ToString() + "</div>"));

                                ////////    }

                                //    iwTabs.tabs = tabs;
                                //    GMap1.addInfoWindowTabs(iwTabs);
                            }
                            
                            sblist.Append("</div>");
                        }
                    }
                }
            }

            reader.Dispose();
            cmd.Dispose();
            con.Close();

            if (showMembersInList)
            {
                memberslist.Visible = true;
                member.Text = sblist.ToString();
            }
        }

        private string getRegion(int regionId)
        {
            XPathNodeIterator iterator = umbraco.library.GetPreValues(1751);
            iterator.MoveNext();
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
            while ((preValues.MoveNext()))
            {
                string valueId = preValues.Current.GetAttribute("id", "");
                if (int.Parse(valueId) == regionId)
                {
                    string preValue = preValues.Current.Value;
                    return preValue;
                }
            }
            return "";
        }

        private void InitializeGoogleMap(string lat, string lng, bool addMarker, int ZoomLevel)
        {
            try
            {
                double dLatitude = Convert.ToDouble(lat, new System.Globalization.CultureInfo("en-US", false));
                double dLongitude = Convert.ToDouble(lng, new System.Globalization.CultureInfo("en-US", false));
                GLatLng latlng = new GLatLng(dLatitude, dLongitude);

                if (addMarker)
                {
                    GMarker marker = new GMarker(latlng);
                    GMap1.Add(marker);
                }

                if (region == 0)
                {

                    GMap1.Add(new GGeoXml("http://fakd.dk.web34.wannafindserver.dk/images/kommunegraense1.kml"));
                    //GMap1.addGGeoXML(new GGeoXml("http://fakd.dk.web34.wannafindserver.dk/images/kommunegraense1.kml"));
                }
                GMap1.GZoom = ZoomLevel;
                GMap1.setCenter(latlng);
                GMap1.mapType = GMapType.GTypes.Normal;
                
                GMap1.DataBind();

               addMembers();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + " " + ex.InnerException.ToString() + " " + ex.StackTrace);
            }
        }

        private string getMemberPropertyValue(int memberID, string propertyName)
        {
           

            string sqlSelect = "EXEC [dbo].[getmemberPropertyValue] " + memberID + ",'" + propertyName  + "'";



            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader reader;
            con.Open();

            using (reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        if (reader.GetString(5).ToString() != string.Empty)
                        {
                            return reader.GetString(5).ToString();
                        }
                    }
                }
            }

            //foreach (Member m in Member.GetAll)
            //{
            //    if (m.Id.Equals(memberID))
            //    {
            //        if (m.getProperty(propertyName).Value != string.Empty) {
            //        return m.getProperty(propertyName).Value.ToString();
            //        }
            //    }
            //}
            return "";
        }
    }
}