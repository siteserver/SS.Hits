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
                var configInfo = Main.Instance.GetConfigInfo(siteId);

                var tableName = Main.Instance.ChannelApi.GetTableName(siteId, channelId);
                
                Main.Instance.HitsDao.AddHits(tableName, !configInfo.IsHitsDisabled, true, contentId);
            }

            return string.Empty;
        }
    }
}
