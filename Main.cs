using System;
using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Hits.Core;
using SS.Hits.Model;
using SS.Hits.Pages;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Hits
{
    public class Main : PluginBase
    {
        public static string PluginId { get; private set; }

        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = Context.ConfigApi.GetConfig<ConfigInfo>(PluginId, siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        public override void Startup(IService service)
        {
            PluginId = Id;

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "内容点击量",
                    IconClass = "ion-connection-bars",
                    Href = $"{nameof(PageSettings)}.aspx"
                })
                ;

            service.BeforeStlParse += Service_BeforeStlParse;
            service.RestApiGet += Service_ApiGet;
        }

        private void Service_BeforeStlParse(object sender, ParseEventArgs e)
        {
            if (e.TemplateType == TemplateType.ContentTemplate && e.ContentId > 0)
            {
                var apiUrl = $"{Context.PluginApi.GetPluginApiUrl(PluginId)}/{nameof(ApiUtils.Hits)}/{e.SiteId}_{e.ChannelId}_{e.ContentId}";
                e.ContentBuilder.Append($@"
<script src=""{apiUrl}"" type=""text/javascript""></script>");
            }
        }

        private object Service_ApiGet(object sender, RestApiEventArgs args)
        {
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(ApiUtils.Hits)))
            {
                return ApiUtils.Hits(args.Request, args.RouteId);
            }

            throw new Exception("请求的资源不在服务器上");
        }
    }
}