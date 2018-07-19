using System;
using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Hits.Core;
using SS.Hits.Model;
using SS.Hits.Pages;
using SS.Hits.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Hits
{
    public class Main : PluginBase
    {
        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = ConfigApi.GetConfig<ConfigInfo>(siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        public static Main Instance { get; private set; }

        public HitsDao HitsDao { get; private set; }

        public override void Startup(IService service)
        {
            Instance = this;

            HitsDao = new HitsDao(ConnectionString, DataApi);

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "内容点击量",
                    IconClass = "ion-connection-bars",
                    Href = $"{nameof(PageSettings)}.aspx"
                })
                ;

            service.BeforeStlParse += Service_BeforeStlParse;
            service.ApiGet += Service_ApiGet;
        }

        private void Service_BeforeStlParse(object sender, ParseEventArgs e)
        {
            if (e.TemplateType == TemplateType.ContentTemplate && e.ContentId > 0)
            {
                var apiUrl = $"{Instance.PluginApi.PluginApiUrl}/{nameof(ApiUtils.Hits)}/{e.SiteId}_{e.ChannelId}_{e.ContentId}";
                e.ContentBuilder.Append($@"
<script src=""{apiUrl}"" type=""text/javascript""></script>");
            }
        }

        private object Service_ApiGet(object sender, ApiEventArgs args)
        {
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(ApiUtils.Hits)))
            {
                return ApiUtils.Hits(args.Request, args.RouteId);
            }

            throw new Exception("请求的资源不在服务器上");
        }
    }
}