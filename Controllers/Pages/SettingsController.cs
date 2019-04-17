using System;
using System.Web.Http;
using SiteServer.Plugin;

namespace SS.Hits.Controllers.Pages
{
    [RoutePrefix("pages/settings")]
    public class SettingsController : ApiController
    {
        private const string Route = "{siteId:int}";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig(int siteId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
                {
                    return Unauthorized();
                }

                var configInfo = Main.GetConfigInfo(siteId);

                return Ok(new
                {
                    Value = configInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit(int siteId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
                {
                    return Unauthorized();
                }

                var configInfo = Main.GetConfigInfo(siteId);

                configInfo.IsHitsDisabled = request.GetPostBool(nameof(configInfo.IsHitsDisabled));
                configInfo.IsHitsCountByDay = request.GetPostBool(nameof(configInfo.IsHitsCountByDay));

                Context.ConfigApi.SetConfig(Main.PluginId, 0, configInfo);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
