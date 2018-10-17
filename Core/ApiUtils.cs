using SiteServer.Plugin;

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

                var tableName = Context.ContentApi.GetTableName(siteId, channelId);
                
                Main.HitsDao.AddHits(tableName, !configInfo.IsHitsDisabled, true, contentId);
            }

            return string.Empty;
        }
    }
}
