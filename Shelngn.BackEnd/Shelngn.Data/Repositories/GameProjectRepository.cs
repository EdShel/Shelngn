using Shelngn.Entities;
using Shelngn.Repositories;

namespace Shelngn.Data.Repositories
{
    public class GameProjectRepository : DapperBaseRepository, IGameProjectRepository
    {
        public GameProjectRepository(IUnitOfWork injectedUnitOfWork) : base(injectedUnitOfWork)
        {
        }

        public async Task CreateAsync(GameProject gameProject, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "INSERT INTO game_project(id, project_name, files_location) " +
                "VALUES(@Id, @ProjectName, @FilesLocation)";
            await ExecuteAsync(sql, gameProject);
        }

        public async Task UpdateNameAsync(Guid gameProjectId, string newProjectName, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "UPDATE game_project SET project_name = @newProjectName WHERE id = @gameProjectId";
            await ExecuteAsync(sql, new { gameProjectId, newProjectName });
        }

        public async Task DeleteAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "DELETE FROM game_project WHERE id = @gameProjectId;";
            await ExecuteAsync(sql, new { gameProjectId });
        }

        public async Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT g.id, g.project_name, g.files_location, g.insert_date " +
                "FROM game_project g " +
                "JOIN game_project_member gp ON g.id = gp.game_project_id " +
                "WHERE gp.app_user_id = @appUserId " +
                "ORDER BY gp.insert_date DESC;";
            return await QueryAsync<GameProject>(sql, new { appUserId });
        }

        public async Task<GameProject?> GetByIdAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT g.id, g.project_name, g.files_location, g.insert_date " +
                "FROM game_project g " +
                "WHERE g.id = @gameProjectId;";
            return await QueryFirstOrDefaultAsync<GameProject>(sql, new { gameProjectId });
        }

        public async Task AddMemberAsync(GameProjectMember gameProjectMember, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "INSERT INTO game_project_member (game_project_id, app_user_id, member_role) " +
                "VALUES (@GameProjectId, @AppUserId, @MemberRole);";
            await ExecuteAsync(sql, gameProjectMember);
        }

        public async Task RemoveMemberAsync(Guid gameProjectId, Guid appUserId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "DELETE FROM game_project_member WHERE game_project_id = @gameProjectId AND app_user_id = @appUserId;";
            await ExecuteAsync(sql, new { gameProjectId, appUserId });
        }

        public Task<GameProjectMember?> GetMemberAsync(Guid gameProjectId, Guid appUserId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT game_project_id, app_user_id, member_role FROM game_project_member " +
                "WHERE game_project_id = @gameProjectId AND app_user_id = @appUserId";
            return QueryFirstOrDefaultAsync<GameProjectMember?>(sql, new { gameProjectId, appUserId });
        }

        public Task<IEnumerable<GameProjectMemberUser>> GetAllMembersAsync(Guid gameProjectId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                "SELECT au.id, au.email, au.user_name, au.avatar_url, m.member_role FROM app_user au " +
                "JOIN game_project_member m ON au.id = m.app_user_id " +
                "WHERE m.game_project_id = @gameProjectId " +
                "ORDER BY m.insert_date ASC";
            return QueryAsync<GameProjectMemberUser>(sql, new { gameProjectId });
        }

        public async Task<IEnumerable<PublishedGameProject>> GetMostRecentPublishedGameProjectsAsync(DateTimeOffset until, int take, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            const string sql =
                @"SELECT TOP(@take)
                    p.id, 
                    p.project_name,
                    p.insert_date,
                    '' AS Screenshot, 
                    s.id,
                    s.image_url,
                    '' AS AppUser,
                    u.id,
                    u.user_name
                FROM game_project p
                LEFT JOIN game_project_screenshot s ON s.game_project_id = p.id
                JOIN game_project_member m ON m.game_project_id = p.id
                JOIN app_user u ON m.app_user_id = u.id
                LEFT JOIN game_project_publication pb ON pb.game_project_id = p.id
                WHERE p.insert_date <= @until AND pb.game_project_id IS NOT NULL
                ORDER BY p.insert_date DESC;";
            var projLookup = new Dictionary<Guid, PublishedGameProject>();
            var picLookup = new Dictionary<Guid, PublishedGameProjectScreenshot>();
            await QueryAsync<PublishedGameProject, PublishedGameProjectScreenshot, PublishedGameProjectUserMember, PublishedGameProject>(
                sql, 
                map: (proj, pic, user) =>
                {
                    PublishedGameProject? result;
                    if (!projLookup.TryGetValue(proj.Id, out result))
                    {
                        result = proj;
                        result.Screenshots = new List<PublishedGameProjectScreenshot>();
                        result.Members = new List<PublishedGameProjectUserMember>();
                        projLookup.Add(result.Id, result);
                    }
                    if (pic.ImageUrl != null && !picLookup.ContainsKey(pic.Id))
                    {
                        result.Screenshots.Add(pic);
                        picLookup.Add(pic.Id, pic);
                    }
                    if (user.UserName != null && !result.Members.Any(u => u.Id == user.Id))
                    {
                        result.Members.Add(user);
                    }
                    return result;
                }, 
                splitOn: "Screenshot,AppUser", 
                param: new { until, take });
            return projLookup.Values.OrderByDescending(p => p.InsertDate).ToList();
        }
    }
}
