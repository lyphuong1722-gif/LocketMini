using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LocketMini.Infrastructure.Persistence;

/// <summary>
/// Seed dữ liệu mẫu tương ứng với INSERT mẫu trong SQL gốc.
/// Chỉ chạy khi DB chưa có dữ liệu.
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

        await _db.Users.AddRangeAsync(admin, nam, linh, ct);

        // SaveChanges để có UserId từ IDENTITY trước khi dùng các relation
        await _db.SaveChangesAsync(ct);

        // ── Friends ───────────────────────────────────────────────────────
        // Reload với friends để domain check
        var adminWithFriends = await _db.Users
            .Include(u => u.Friends)
            .FirstAsync(u => u.Username == "admin", ct);

        var namUser = await _db.Users.FirstAsync(u => u.Username == "nam", ct);
        var linhUser = await _db.Users.FirstAsync(u => u.Username == "linh", ct);

        adminWithFriends.AddFriend(namUser);
        adminWithFriends.AddFriend(linhUser);
        await _db.SaveChangesAsync(ct);

        // ── Posts ─────────────────────────────────────────────────────────
        var adminFresh = await _db.Users.FirstAsync(u => u.Username == "admin", ct);
        var namFresh = await _db.Users.FirstAsync(u => u.Username == "nam", ct);

        var post1 = adminFresh.AddPost("Xin chao moi nguoi", "img1.jpg");
        var post2 = namFresh.AddPost("Bai viet dau tien", "img2.jpg");

        await _db.SaveChangesAsync(ct);

        // ── Likes & Comments ──────────────────────────────────────────────
        // Phải load lại post với details để domain validation không bị lỗi
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
