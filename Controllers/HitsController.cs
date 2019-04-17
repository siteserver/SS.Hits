using SiteServer.Plugin;
using System;
using System.Web.Http;
using SS.Hits.Model;

namespace SS.Hits.Controllers
{
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

            if (configInfo.IsHitsCountByDay)
            {
                var now = DateTime.Now;

                if (contentInfo.LastHitsDate != null)
                {
                    var lastHitsDate = contentInfo.LastHitsDate.Value;

                    contentInfo.HitsByDay = now.Day != lastHitsDate.Day || now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : contentInfo.HitsByDay + 1;
                    contentInfo.HitsByWeek = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year || now.DayOfYear / 7 != lastHitsDate.DayOfYear / 7 ? 1 : contentInfo.HitsByWeek + 1;
                    contentInfo.HitsByMonth = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : contentInfo.HitsByMonth + 1;
                }
                else
                {
                    contentInfo.HitsByDay = contentInfo.HitsByWeek = contentInfo.HitsByMonth = 1;
                }

                contentInfo.Hits += 1;

                contentInfo.LastHitsDate = now;

                Context.ContentApi.Update(siteId, channelId, contentInfo);

            }
            else
            {
                contentInfo.Hits += 1;

                contentInfo.LastHitsDate = DateTime.Now;

                Context.ContentApi.Update(siteId, channelId, contentInfo);
            }
        }
    }
}
