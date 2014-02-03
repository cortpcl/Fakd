using System.ComponentModel;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using System.Net.Mail;
using umbraco.cms.businesslogic.media;
using umbraco.BusinessLogic;
using umbraco;


namespace Ontranet
{

    public class FolderRequest
    {
        public string id;
        public string folderName;
    }

    /// <summary>
    /// Summary description for fileManager
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService] // To allow this Web Service to be called from script, (using ASP.NET AJAX or JQuery), and return json formatted data.
    [ToolboxItem(false)]
    public class fileManager : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteFolder(FolderRequest req)
        {
            
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RenameFolder(FolderRequest req)
        {

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addFolder(FolderRequest req)
        {
            string returnValue;
            try
            {
                string strFolderName;
                int iId;

                iId = int.Parse(req.id);
                strFolderName = req.folderName;


                var fileType = MediaType.GetByAlias("Folder");
                Media m = Media.MakeNew(strFolderName, fileType, new User(0), iId);

                library.ClearLibraryCacheForMedia(iId);
                library.RefreshContent();

                HttpRuntime.UnloadAppDomain();

                returnValue = "success";
            }
            catch
            {

                returnValue = "failed";
            }

           
            return returnValue;

        }
    }
}
