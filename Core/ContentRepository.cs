using System;
using Datory;
using SiteServer.Plugin;

namespace SS.Hits.Core
{
    public class ContentRepository
    {
        private readonly Repository _repository;

        public ContentRepository(string tableName)
        {
            _repository = new Repository(Context.Environment.DatabaseType, Context.Environment.ConnectionString, tableName);
        }

        public void AddHits(IContentInfo contentInfo, bool isHitsCountByDay)
        {
            var now = DateTime.Now;

            if (isHitsCountByDay)
            {
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

                _repository.Update(Q
                    .Set("HitsByDay", contentInfo.HitsByDay)
                    .Set("HitsByWeek", contentInfo.HitsByWeek)
                    .Set("HitsByMonth", contentInfo.HitsByMonth)
                    .Set("Hits", contentInfo.Hits)
                    .Set("LastHitsDate", contentInfo.LastHitsDate)
                    .Where("Id", contentInfo.Id)
                );
            }
            else
            {
                contentInfo.Hits += 1;
                contentInfo.LastHitsDate = DateTime.Now;
                _repository.Update(Q
                    .Set("Hits", contentInfo.Hits)
                    .Set("LastHitsDate", contentInfo.LastHitsDate)
                    .Where("Id", contentInfo.Id)
                );
            }
        }
    }
}
