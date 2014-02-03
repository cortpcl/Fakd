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
    public partial class GMap : System.Web.UI.UserControl
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

        enum locationType
        {
            lat,
            lng
        }

        private string getValue(string Address, locationType stype, Member m = null)
        {
            string output = string.Empty;

            try
            {
                String LatLng = ProcessAddress(Address, stype);

                if (stype == locationType.lat)
                {
                    output = LatLng;
                    if (m.getProperty("latitude").Value.ToString() == "" || m.getProperty("latitude").Value.ToString() == "0")
                        m.getProperty("latitude").Value = LatLng;
                }
                else
                {
                    output = LatLng;
                    if (m.getProperty("longitude").Value.ToString() == "" || m.getProperty("longitude").Value.ToString() == "0")
                        m.getProperty("longitude").Value = LatLng;
                }

                if ((m.getProperty("latitude").Value.ToString() == "" || m.getProperty("latitude").Value.ToString() == "0") || (m.getProperty("longitude").Value.ToString() == "" || m.getProperty("longitude").Value.ToString() == "0"))
                    m.Save();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + " " + ex.InnerException.ToString() + " " + ex.StackTrace);
            }
            return output;
        }

        private string ProcessAddress(string FullAddress, locationType ltype)
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
                case locationType.lat:
                    returnVal = latVal;
                    break;
                case locationType.lng:
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

                double dLatitude = Convert.ToDouble(getValue(regionArea + ", Danmark", locationType.lat), new System.Globalization.CultureInfo("en-US", false));
                double dLongitude = Convert.ToDouble(getValue(regionArea + ", Danmark", locationType.lng), new System.Globalization.CultureInfo("en-US", false));
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

            string stitelFraBasisuddannelseMember = "";
            string sfornavnMember = "";
            string sefternavnMember = "";
            string scvrNumberMember = "";
            string sfrdiguddanetMember = "";
            string sautorisationsIDMember = "";

            foreach (Member m in Member.GetAll)
            {
                MemberType demoMemberType = MemberType.GetByAlias("Distister");

                if (m.ContentType.Equals(demoMemberType))
                {
                    Property showOnMap = m.getProperty("showOnMap");
                    Property memberAthorized = m.getProperty("memberAthorized");

                    //hvis property vis på kort er sat til true
                    if (showOnMap.Value.ToString() == "1" && memberAthorized.Value.ToString() == "1")
                    {
                        try
                        {
                            Property memberRegion = m.getProperty("employmentRegionMember");
                            int RegionId = 0;
                            if ((memberRegion != null))
                            {
                                if (!string.IsNullOrEmpty(memberRegion.Value.ToString()))
                                {
                                    RegionId = int.Parse(memberRegion.Value.ToString());
                                }
                            }

                            Property ansaettelsesSted = m.getProperty("ansttelsesstedHovedbeskftigelseMember");
                            Property titelFraBasisuddannelseMember = m.getProperty("employmentWorkTypeNameMember");
                            Property fornavnMember = m.getProperty("fornavnMember");
                            Property efternavnMember = m.getProperty("efternavnMember");
                            Property adresseMember = m.getProperty("adressePArbejdsstedMember");
                            Property postnrMember = m.getProperty("postnrEmploymentMember");
                            Property byNavnMember = m.getProperty("byNavnEmploymentMember");
                            Property employmentPhoneMember = m.getProperty("employmentPhoneMember");

                            Property emailMember = m.getProperty("arbejdsEmailMember");
                            Property hjemmeside = m.getProperty("employmentWebsiteMember");
                            Property cvrNumberMember = m.getProperty("employmentCVRMember");
                            Property frdiguddanetMember = m.getProperty("frdiguddanetMember");
                            Property autorisationsIDMember = m.getProperty("autorisationsIDMember");
                            Property specialer = m.getProperty("specialtiesMember");
                            Property latitudeProp = m.getProperty("latitude");
                            Property longitudeProp = m.getProperty("longitude");

                            StringBuilder sb = new StringBuilder();

                            sb.Append("<div style=\"text-align:left; width:180px\">");

                            if (RegionId > 0)
                            {
                                XPathNodeIterator iterator = umbraco.library.GetPreValues(1751);
                                iterator.MoveNext();
                                XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
                                while ((preValues.MoveNext()))
                                {
                                    string valueId = preValues.Current.GetAttribute("id", "");
                                    if (int.Parse(valueId) == int.Parse(RegionId.ToString()))
                                    {
                                        string preValue = preValues.Current.Value;
                                        sb.Append("<b>" + preValue + "</b><br/>");
                                    }
                                }
                            }

                            if ((ansaettelsesSted != null))
                            {
                                if (!string.IsNullOrEmpty(ansaettelsesSted.Value.ToString()))
                                {
                                    sb.Append(ansaettelsesSted.Value + "<br/>");
                                }
                            }

                            if ((titelFraBasisuddannelseMember != null))
                            {
                                if (!string.IsNullOrEmpty(titelFraBasisuddannelseMember.Value.ToString()))
                                {
                                    stitelFraBasisuddannelseMember = titelFraBasisuddannelseMember.Value.ToString();
                                    sb.Append(titelFraBasisuddannelseMember.Value.ToString() + " ");
                                }

                            }

                            if ((fornavnMember != null))
                            {
                                if (!string.IsNullOrEmpty(fornavnMember.Value.ToString()))
                                {
                                    sfornavnMember = fornavnMember.Value.ToString();
                                    sb.Append(fornavnMember.Value.ToString() + " ");
                                }
                            }


                            if ((efternavnMember != null))
                            {
                                if (!string.IsNullOrEmpty(efternavnMember.Value.ToString()))
                                {
                                    sefternavnMember = efternavnMember.Value.ToString();
                                    sb.Append(efternavnMember.Value.ToString() + "<br/>");
                                }
                                else
                                {
                                    sb.Append("<br/>");
                                }
                            }

                            if ((adresseMember != null))
                            {
                                if (!string.IsNullOrEmpty(adresseMember.Value.ToString()))
                                {
                                    sb.Append(adresseMember.Value.ToString() + "<br/>");
                                }
                            }

                            if ((postnrMember != null))
                            {
                                if (!string.IsNullOrEmpty(postnrMember.Value.ToString()))
                                {
                                    sb.Append(postnrMember.Value.ToString() + " ");
                                }
                            }

                            if ((byNavnMember != null))
                            {
                                if (!string.IsNullOrEmpty(byNavnMember.Value.ToString()))
                                {
                                    sb.Append(byNavnMember.Value.ToString() + "<br/>");
                                }
                                else
                                {
                                    sb.Append("<br/>");
                                }
                            }

                            if ((employmentPhoneMember != null))
                            {
                                if (!string.IsNullOrEmpty(employmentPhoneMember.Value.ToString()))
                                {
                                    sb.Append("<b>Telefon:</b> " + employmentPhoneMember.Value.ToString() + "<br/>");
                                }
                            }

                            if ((emailMember != null))
                            {
                                if (!string.IsNullOrEmpty(emailMember.Value.ToString()))
                                {
                                    sb.Append("<b>E-mail:</b> <a href=\"mailto:" + emailMember.Value.ToString() + "\">" + emailMember.Value.ToString() + "</a><br/>");
                                }
                            }

                            if ((hjemmeside != null))
                            {
                                if (!string.IsNullOrEmpty(hjemmeside.Value.ToString()))
                                {
                                    string website = hjemmeside.Value.ToString();
                                    if (!website.StartsWith("http://"))
                                    {
                                        website = "http://" + website;
                                    }
                                    if (!string.IsNullOrEmpty(hjemmeside.Value.ToString()))
                                        sb.Append("<b>Hjemmeside:</b> <a href=\"" + website + "\" target=\"_blank\">" + hjemmeside.Value + "</a><br/><br/>");
                                }
                            }



                            // StringBuilder sbTab2 = new StringBuilder();

                            if ((cvrNumberMember != null))
                            {
                                if (!string.IsNullOrEmpty(cvrNumberMember.Value.ToString()))
                                {
                                    scvrNumberMember = cvrNumberMember.Value.ToString();
                                    sb.Append("<b>CVR:</b> " + cvrNumberMember.Value.ToString() + "<br/>");
                                }
                            }

                            if ((frdiguddanetMember != null))
                            {
                                if (!string.IsNullOrEmpty(frdiguddanetMember.Value.ToString()))
                                {
                                    sfrdiguddanetMember = frdiguddanetMember.Value.ToString();
                                    sb.Append("<b>Færdiguddannet år:</b> " + frdiguddanetMember.Value.ToString() + "<br/>");
                                }
                            }

                            if ((autorisationsIDMember != null))
                            {
                                if (!string.IsNullOrEmpty(autorisationsIDMember.Value.ToString()))
                                {
                                    sautorisationsIDMember = autorisationsIDMember.Value.ToString();
                                    sb.Append("<b>AutorisationsID:</b> " + autorisationsIDMember.Value.ToString() + "<br/>");
                                }
                            }

                            if ((specialer != null))
                            {
                                if (!string.IsNullOrEmpty(specialer.Value.ToString()))
                                {
                                    sb.Append("<b>Specialer:</b> " + specialer.Value.ToString() + "<br/>");
                                }
                            }

                            sb.Append("</div>");

                            if (showMembersInList)
                            {
                                memberslist.Visible = true;

                                member.Text = sb.ToString();
                            }

                            string addressen = string.Empty;

                            if ((string.IsNullOrEmpty(latitudeProp.Value.ToString()) & string.IsNullOrEmpty(longitudeProp.Value.ToString())) || (latitudeProp.Value.ToString() == "0" || longitudeProp.Value.ToString() == "0"))
                            {
                                if ((adresseMember != null))
                                {
                                    if (!string.IsNullOrEmpty(adresseMember.Value.ToString()))
                                    {
                                        addressen = adresseMember.Value.ToString();
                                    }
                                }

                                if ((postnrMember != null))
                                {
                                    if (!string.IsNullOrEmpty(postnrMember.Value.ToString()))
                                    {
                                        addressen = addressen + "," + postnrMember.Value.ToString();
                                    }
                                }

                                if ((byNavnMember != null))
                                {
                                    if (!string.IsNullOrEmpty(byNavnMember.Value.ToString()))
                                    {
                                        addressen = addressen + " " + byNavnMember.Value.ToString();
                                    }
                                }

                                addressen = addressen + ",Danmark";
                            }



                            double dLatitude = 0;
                            double dLongitude = 0;

                            if (latitudeProp.Value.ToString() != "" && latitudeProp.Value.ToString() != "0")
                            {
                                dLatitude = Convert.ToDouble(latitudeProp.Value.ToString(), new System.Globalization.CultureInfo("en-US", false));
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(addressen))
                                    dLatitude = Convert.ToDouble(getValue(addressen, locationType.lat, m), new System.Globalization.CultureInfo("en-US", false));
                            }

                            if (longitudeProp.Value.ToString() != "" && longitudeProp.Value.ToString() != "0")
                            {
                                dLongitude = Convert.ToDouble(longitudeProp.Value.ToString(), new System.Globalization.CultureInfo("en-US", false));
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(addressen))
                                    dLongitude = Convert.ToDouble(getValue(addressen, locationType.lng, m), new System.Globalization.CultureInfo("en-US", false));
                            }



                            if (!(dLatitude == 0) & !(dLongitude == 0))
                            {


                                GMarker marker = new GMarker(new GLatLng(dLatitude, dLongitude));


                                GInfoWindow window = new GInfoWindow(marker, sb.ToString(), false);

                               // GMap1.addInfoWindow(window);
                                GMap1.Add(window);


                                //GMarker marker = new GMarker(latlng);


                                //    GInfoWindowTabs iwTabs = new GInfoWindowTabs();
                                //    iwTabs.gMarker = marker;

                                //   System.Collections.Generic.List<GInfoWindowTab> tabs = new System.Collections.Generic.List<GInfoWindowTab>();

                                //    tabs.Add(new GInfoWindowTab("Info", sb.ToString()));
                                //////    if (sbTab2.Length > 0)
                                //////    {
                                //////        tabs.Add(new GInfoWindowTab("Klinik", "<div style=\"width:200px; float:left; padding-left:5px\">" + sbTab2.ToString() + "</div>"));

                                //////    }

                                //    iwTabs.tabs = tabs;
                                //    GMap1.addInfoWindowTabs(iwTabs);

                            }

                            addAdditionalDepartments(m, stitelFraBasisuddannelseMember, sfornavnMember, sefternavnMember, scvrNumberMember, sfrdiguddanetMember, sautorisationsIDMember);

                        }
                        catch (Exception ex)
                        {
                            Response.Write(ex.Message);
                        }
                    }
                }
            }

        }

        private void addAdditionalDepartments(Member m, string titelFraBasisuddannelseMember, string fornavnMember, string efternavnMember, string cvrNumberMember, string frdiguddanetMember, string autorisationsIDMember)
        {

            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd = new SqlCommand("SELECT [afdelingsnavn],[adresse],[postnr],[bynavn],[telefon],[email],[hjemmeside],[showOnMap],[region],[memberId],[specialer] FROM dbo.ontranetMemberDepartment WHERE [memberId]=" + m.Id, con);
            SqlDataReader reader;
            con.Open();

            using (reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        bool showOnMap = reader.GetSqlBoolean(7).Value;

                        if (showOnMap)
                        {
                            int RegionId = 0;

                            if (reader.GetSqlInt32(8) > 0) { RegionId = reader.GetSqlInt32(8).Value; }
                            string adresseMember = "";
                            string postnrMember = "";
                            string byNavnMember = "";

                            StringBuilder sb = new StringBuilder();

                            sb.Append("<div style=\"text-align:left; width:180px\">");

                            if (RegionId > 0)
                            {
                                XPathNodeIterator iterator = umbraco.library.GetPreValues(1751);
                                iterator.MoveNext();
                                XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
                                while ((preValues.MoveNext()))
                                {
                                    string valueId = preValues.Current.GetAttribute("id", "");
                                    if (int.Parse(valueId) == int.Parse(RegionId.ToString()))
                                    {
                                        string preValue = preValues.Current.Value;
                                        sb.Append("<b>" + preValue + "</b><br/>");
                                    }
                                }
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(0).Value))
                            {
                                sb.Append(reader.GetSqlString(0).Value + "<br/>");
                            }

                            if (!string.IsNullOrEmpty(titelFraBasisuddannelseMember))
                            {
                                sb.Append(titelFraBasisuddannelseMember + " ");
                            }

                            if (!string.IsNullOrEmpty(fornavnMember))
                            {
                                sb.Append(fornavnMember + " ");
                            }

                            if (!string.IsNullOrEmpty(efternavnMember))
                            {
                                sb.Append(efternavnMember + "<br/>");
                            }
                            else
                            {
                                sb.Append("<br/>");
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(1).Value))
                            {
                                adresseMember = reader.GetSqlString(1).Value;
                                sb.Append(adresseMember + "<br/>");
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(2).Value))
                            {
                                postnrMember = reader.GetSqlString(2).Value;
                                sb.Append(postnrMember + " ");
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(3).Value))
                            {
                                byNavnMember = reader.GetSqlString(3).Value;
                                sb.Append(byNavnMember + "<br/>");
                            }
                            else
                            {
                                sb.Append("<br/>");
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(4).Value))
                            {
                                sb.Append("<b>Telefon:</b> " + reader.GetSqlString(4).Value + "<br/>");
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(5).Value))
                            {
                                sb.Append("<b>E-mail:</b> <a href=\"mailto:" + reader.GetSqlString(5).Value + "\">" + reader.GetSqlString(5).Value + "</a><br/>");
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(6).Value))
                            {
                                string website = reader.GetSqlString(6).Value;
                                if (!website.StartsWith("http://"))
                                {
                                    website = "http://" + website;
                                }
                                if (!string.IsNullOrEmpty(reader.GetSqlString(6).Value))
                                    sb.Append("<b>Hjemmeside:</b> <a href=\"" + website + "\" target=\"_blank\">" + reader.GetSqlString(6).Value + "</a><br/><br/>");
                            }


                            if (!string.IsNullOrEmpty(cvrNumberMember))
                            {
                                sb.Append("<b>CVR:</b> " + cvrNumberMember + "<br/>");
                            }

                            if (!string.IsNullOrEmpty(frdiguddanetMember))
                            {
                                sb.Append("<b>Færdiguddannet år:</b> " + frdiguddanetMember + "<br/>");
                            }


                            if ((autorisationsIDMember != null))
                            {
                                if (!string.IsNullOrEmpty(autorisationsIDMember))
                                {
                                    sb.Append("<b>AutorisationsID:</b> " + autorisationsIDMember + "<br/>");
                                }
                            }


                            if (!string.IsNullOrEmpty(reader.GetSqlString(10).Value))
                            {
                                sb.Append("<b>Specialer:</b> " + reader.GetSqlString(10).Value + "<br/>");
                            }


                            sb.Append("</div>");

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

                        
                            double dLatitude = 0;
                            double dLongitude = 0;

                            if (!string.IsNullOrEmpty(addressen))
                                dLatitude = Convert.ToDouble(getValue(addressen, locationType.lat, m), new System.Globalization.CultureInfo("en-US", false));

                            if (!string.IsNullOrEmpty(addressen))
                                dLongitude = Convert.ToDouble(getValue(addressen, locationType.lng, m), new System.Globalization.CultureInfo("en-US", false));

                            if (!(dLatitude == 0) & !(dLongitude == 0))
                            {
                                GMarker marker = new GMarker(new GLatLng(dLatitude, dLongitude));
                                GInfoWindow window = new GInfoWindow(marker, sb.ToString(), false);
                               // GMap1.addInfoWindow(window);
                                GMap1.Add(window);
                            }
                        }
                    }
                }
            }

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
                    //GMap1.addGGeoXML(new GGeoXml("http://fakd.dk.web34.wannafindserver.dk/images/kommunegraense1.kml"));
                    GMap1.Add(new GGeoXml("http://fakd.dk.web34.wannafindserver.dk/images/kommunegraense1.kml"));
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
    }
}