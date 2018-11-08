using SiteServer.Plugin;
using SS.Hits.Provider;

namespace SS.Hits.Core
{
    public static class ApiUtils
    {
        public static object Hits(IRequest request, string id)
        {
            var idList = id.Split('_');
            if (idList.Length == 3)
            {
                var siteId = Utils.ToInt(idList[0]);
                var channelId = Utils.ToInt(idList[1]);
                var contentId = Utils.ToInt(idList[2]);
                var configInfo = Main.GetConfigInfo(siteId);

                //var tableName = Context.ContentApi.GetTableName(siteId, channelId);
                
                HitsDao.AddHits(siteId, channelId, contentId, !configInfo.IsHitsDisabled, true);
            }

            return string.Empty;
        }
    }
}
