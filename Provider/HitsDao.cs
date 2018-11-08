using System;
using SiteServer.Plugin;

namespace SS.Hits.Provider
{
    public static class HitsDao
    {
        public static void AddHits(int siteId, int channelId, int contentId, bool isCountHits, bool isCountHitsByDay)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0 || !isCountHits) return;
            var contentInfo = Context.ContentApi.GetContentInfo(siteId, channelId, contentId);
            if (contentInfo == null) return;

            if (isCountHitsByDay)
            {
                var now = DateTime.Now;

                contentInfo.HitsByDay = now.Day != contentInfo.LastHitsDate.Day || now.Month != contentInfo.LastHitsDate.Month || now.Year != contentInfo.LastHitsDate.Year ? 1 : contentInfo.HitsByDay + 1;

                contentInfo.HitsByWeek = now.Month != contentInfo.LastHitsDate.Month || now.Year != contentInfo.LastHitsDate.Year || now.DayOfYear / 7 != contentInfo.LastHitsDate.DayOfYear / 7 ? 1 : contentInfo.HitsByWeek + 1;

                contentInfo.HitsByMonth = now.Month != contentInfo.LastHitsDate.Month || now.Year != contentInfo.LastHitsDate.Year ? 1 : contentInfo.HitsByMonth + 1;

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

        //public static void AddHits(string tableName, bool isCountHits, bool isCountHitsByDay, int contentId)
        //{
        //    if (contentId <= 0 || !isCountHits) return;

        //    if (isCountHitsByDay)
        //    {
        //        var referenceId = 0;
        //        var hitsByDay = 0;
        //        var hitsByWeek = 0;
        //        var hitsByMonth = 0;
        //        var lastHitsDate = DateTime.Now;

        //        var sqlString =
        //            $"SELECT ReferenceId, HitsByDay, HitsByWeek, HitsByMonth, LastHitsDate FROM {tableName} WHERE (Id = {contentId})";

        //        using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
        //        {
        //            if (rdr.Read())
        //            {
        //                var i = 0;
        //                referenceId = Context.DatabaseApi.GetInt(rdr, i++);
        //                hitsByDay = Context.DatabaseApi.GetInt(rdr, i++);
        //                hitsByWeek = Context.DatabaseApi.GetInt(rdr, i++);
        //                hitsByMonth = Context.DatabaseApi.GetInt(rdr, i++);
        //                lastHitsDate = Context.DatabaseApi.GetDateTime(rdr, i);
        //            }
        //            rdr.Close();
        //        }

        //        if (referenceId > 0)
        //        {
        //            contentId = referenceId;
        //        }

        //        var now = DateTime.Now;

        //        hitsByDay = now.Day != lastHitsDate.Day || now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : hitsByDay + 1;
        //        hitsByWeek = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year || now.DayOfYear / 7 != lastHitsDate.DayOfYear / 7 ? 1 : hitsByWeek + 1;
        //        hitsByMonth = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : hitsByMonth + 1;

        //        sqlString =
        //            $"UPDATE {tableName} SET {Context.DatabaseApi.ToPlusSqlString("Hits", 1)}, HitsByDay = {hitsByDay}, HitsByWeek = {hitsByWeek}, HitsByMonth = {hitsByMonth}, LastHitsDate = {Context.DatabaseApi.ToDateTimeSqlString(DateTime.Now)} WHERE Id = {contentId}  AND ReferenceId = 0";
        //        Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        //    }
        //    else
        //    {
        //        var sqlString =
        //            $"UPDATE {tableName} SET {Context.DatabaseApi.ToPlusSqlString("Hits", 1)}, LastHitsDate = {Context.DatabaseApi.ToDateTimeSqlString(DateTime.Now)} WHERE Id = {contentId} AND ReferenceId = 0";
        //        var count = Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        //        if (count < 1)
        //        {
        //            var referenceId = 0;

        //            sqlString = $"SELECT ReferenceId FROM {tableName} WHERE (Id = {contentId})";

        //            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
        //            {
        //                if (rdr.Read())
        //                {
        //                    referenceId = Context.DatabaseApi.GetInt(rdr, 0);
        //                }
        //                rdr.Close();
        //            }

        //            if (referenceId > 0)
        //            {
        //                sqlString =
        //                    $"UPDATE {tableName} SET {Context.DatabaseApi.ToPlusSqlString("Hits", 1)}, LastHitsDate = {Context.DatabaseApi.ToDateTimeSqlString(DateTime.Now)} WHERE Id = {referenceId} AND ReferenceId = 0";
        //                Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        //            }
        //        }
        //    }
        //}
    }
}
