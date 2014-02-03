using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;

using umbraco;
using umbraco.presentation.nodeFactory;
using System.Web;
using umbraco.cms.businesslogic.web;

namespace Ontranet
{
    public partial class LoginNetworkGroupsForumAdmin : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             if (HttpContext.Current.User.IsInRole("ForumAdmin")) {

                 Member m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name);
                


                 //Response.Node currentNode = Node.GetCurrent();

             }

                  
        }

   

     

        

      
    }
}