using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Hits.Core;
using SS.Hits.Model;

namespace SS.Hits.Provider
{
    public class HitsDao
    {
        private readonly string _connectionString;
        private readonly IDatabaseApi _helper;

        public HitsDao(string connectionString, IDatabaseApi helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public void AddHits(string tableName, bool isCountHits, bool isCountHitsByDay, int contentId)
        {
            if (contentId <= 0 || !isCountHits) return;

            if (isCountHitsByDay)
            {
                var referenceId = 0;
                var hitsByDay = 0;
                var hitsByWeek = 0;
                var hitsByMonth = 0;
                var lastHitsDate = DateTime.Now;

                var sqlString =
                    $"SELECT ReferenceId, HitsByDay, HitsByWeek, HitsByMonth, LastHitsDate FROM {tableName} WHERE (Id = {contentId})";

                using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
                {
                    if (rdr.Read())
                    {
                        var i = 0;
                        referenceId = _helper.GetInt(rdr, i++);
                        hitsByDay = _helper.GetInt(rdr, i++);
                        hitsByWeek = _helper.GetInt(rdr, i++);
                        hitsByMonth = _helper.GetInt(rdr, i++);
                        lastHitsDate = _helper.GetDateTime(rdr, i);
                    }
                    rdr.Close();
                }

                if (referenceId > 0)
                {
                    contentId = referenceId;
                }

                var now = DateTime.Now;

                hitsByDay = now.Day != lastHitsDate.Day || now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : hitsByDay + 1;
                hitsByWeek = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year || now.DayOfYear / 7 != lastHitsDate.DayOfYear / 7 ? 1 : hitsByWeek + 1;
                hitsByMonth = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : hitsByMonth + 1;

                sqlString =
                    $"UPDATE {tableName} SET {_helper.ToPlusSqlString("Hits", 1)}, HitsByDay = {hitsByDay}, HitsByWeek = {hitsByWeek}, HitsByMonth = {hitsByMonth}, LastHitsDate = {_helper.ToDateTimeSqlString(DateTime.Now)} WHERE Id = {contentId}  AND ReferenceId = 0";
                _helper.ExecuteNonQuery(_connectionString, sqlString);
            }
            else
            {
                var sqlString =
                    $"UPDATE {tableName} SET {_helper.ToPlusSqlString("Hits", 1)}, LastHitsDate = {_helper.ToDateTimeSqlString(DateTime.Now)} WHERE Id = {contentId} AND ReferenceId = 0";
                var count = _helper.ExecuteNonQuery(_connectionString, sqlString);
                if (count < 1)
                {
                    var referenceId = 0;

                    sqlString = $"SELECT ReferenceId FROM {tableName} WHERE (Id = {contentId})";

                    using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
                    {
                        if (rdr.Read())
                        {
                            referenceId = _helper.GetInt(rdr, 0);
                        }
                        rdr.Close();
                    }

                    if (referenceId > 0)
                    {
                        sqlString =
                            $"UPDATE {tableName} SET {_helper.ToPlusSqlString("Hits", 1)}, LastHitsDate = {_helper.ToDateTimeSqlString(DateTime.Now)} WHERE Id = {referenceId} AND ReferenceId = 0";
                        _helper.ExecuteNonQuery(_connectionString, sqlString);
                    }
                }
            }
        }
    }
}
