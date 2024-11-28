using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Owin.Security;

namespace OIDCWithADFS
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin(object sender, EventArgs e)
        {
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
            string authority = System.Configuration.ConfigurationManager.AppSettings["Authority"];
            string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

            string authorizationEndpoint = $"{authority}oauth2/authorize";
            string responseType = "code";
            string scope = "openid profile";

            string authorizationRequest = $"{authorizationEndpoint}?client_id={clientId}&response_type={responseType}&redirect_uri={redirectUri}&scope={scope}";

            Response.Redirect(authorizationRequest);
        }
    }

}