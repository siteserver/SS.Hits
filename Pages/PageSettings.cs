using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Hits.Core;
using SS.Hits.Model;

namespace SS.Hits.Pages
{
    public class PageSettings : Page
    {
        public Literal LtlMessage;
        public DropDownList DdlIsHitsDisabled;
        public DropDownList DdlIsDownloadsDisabled;

        private int _siteId;
        private ConfigInfo _configInfo;

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);

            if (!Main.Instance.AdminApi.HasSitePermissions(_siteId, Main.Instance.Id))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Main.Instance.GetConfigInfo(_siteId);

            if (IsPostBack) return;

            Utils.SelectSingleItem(DdlIsHitsDisabled, _configInfo.IsHitsDisabled.ToString());
            Utils.SelectSingleItem(DdlIsDownloadsDisabled, _configInfo.IsDownloadsDisabled.ToString());
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            _configInfo.IsHitsDisabled = Utils.ToBool(DdlIsHitsDisabled.SelectedValue);
            _configInfo.IsDownloadsDisabled = Utils.ToBool(DdlIsDownloadsDisabled.SelectedValue);

            Main.Instance.ConfigApi.SetConfig(_siteId, _configInfo);
            LtlMessage.Text = Utils.GetMessageHtml("设置修改成功！", true);
        }
    }
}