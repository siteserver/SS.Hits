using SiteServer.Plugin;
using System;
using System.Web.Http;
using SS.Hits.Core;
using SS.Hits.Model;

namespace SS.Hits.Controllers
{
    [RoutePrefix("hits")]
    public class HitsController : ApiController
    {
        [HttpGet, Route("{siteId:int}/{channelId:int}/{contentId:int}")]
        public IHttpActionResult Hits(int siteId, int channelId, int contentId)
        {
            try
            {
                var configInfo = Main.GetConfigInfo(siteId);

                //var tableName = Context.ContentApi.GetTableName(siteId, channelId);

                AddContentHits(siteId, channelId, contentId, configInfo);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static void AddContentHits(int siteId, int channelId, int contentId, ConfigInfo configInfo)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0 || configInfo.IsHitsDisabled) return;
            var contentInfo = Context.ContentApi.GetContentInfo(siteId, channelId, contentId);
            if (contentInfo == null) return;

            var repository = new ContentRepository(Context.ContentApi.GetTableName(siteId, channelId));

            repository.AddHits(contentInfo, configInfo.IsHitsCountByDay);
        }
    }
}
