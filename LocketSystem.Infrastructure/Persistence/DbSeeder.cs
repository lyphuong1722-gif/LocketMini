using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LocketMini.Infrastructure.Persistence;

/// <summary>
/// Seed dữ liệu mẫu. Chỉ chạy khi DB chưa có dữ liệu.
/// </summary>
public sealed class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext db, IPasswordHasher hasher, ILogger<DbSeeder> logger)
    {
        _db = db;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await _db.Users.AnyAsync(ct))
        {
            _logger.LogInformation("Database đã có dữ liệu, bỏ qua seeding.");
            return;
        }

        _logger.LogInformation("Bắt đầu seed dữ liệu mẫu...");

        // ── Users ─────────────────────────────────────────────────────────
        var admin = User.Create("admin", _hasher.Hash("123456"));
        var nam = User.Create("nam", _hasher.Hash("111111"));
        var linh = User.Create("linh", _hasher.Hash("222222"));

        admin.SetProfile("Nguyen Van Admin", "Quan tri vien");
        nam.SetProfile("Tran Van Nam", "Yeu thich cong nghe");
        linh.SetProfile("Le Thi Linh", "Sinh vien");

        await _db.Users.AddRangeAsync(new[] { admin, nam, linh }, ct);
        await _db.SaveChangesAsync(ct);

        // ── Reload để có UserId từ IDENTITY ──────────────────────────────
        // Dùng .Value để so sánh string thay vì Value Object
        var adminUser = await _db.Users
            .Include(u => u.Friends)
            .FirstAsync(u => u.Username.Value == "admin", ct);

        var namUser = await _db.Users
            .FirstAsync(u => u.Username.Value == "nam", ct);

        var linhUser = await _db.Users
            .FirstAsync(u => u.Username.Value == "linh", ct);

        // ── Friends ───────────────────────────────────────────────────────
        adminUser.AddFriend(namUser);
        adminUser.AddFriend(linhUser);
        await _db.SaveChangesAsync(ct);

        // ── Posts ─────────────────────────────────────────────────────────
        var adminFresh = await _db.Users
            .FirstAsync(u => u.Username.Value == "admin", ct);

        var namFresh = await _db.Users
            .FirstAsync(u => u.Username.Value == "nam", ct);

        var post1 = adminFresh.AddPost("Xin chao moi nguoi", "img1.jpg");
        var post2 = namFresh.AddPost("Bai viet dau tien", "img2.jpg");

        await _db.SaveChangesAsync(ct);

        // ── Likes & Comments ──────────────────────────────────────────────
        var post1WithDetails = await _db.Posts
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .FirstAsync(p => p.PostId == post1.PostId, ct);

        post1WithDetails.AddLike(namUser.UserId);
        post1WithDetails.AddLike(linhUser.UserId);
        post1WithDetails.AddComment(namUser.UserId, "Bai viet rat hay");
        post1WithDetails.AddComment(linhUser.UserId, "Chuc mung ban");

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Seed dữ liệu hoàn tất.");
    }
}
