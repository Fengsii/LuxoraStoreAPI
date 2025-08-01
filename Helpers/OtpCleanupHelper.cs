using LuxoraStore.Interfaces;
using LuxoraStore.Model;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace LuxoraStore.Helpers
{
    public class OtpCleanupHelper : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public OtpCleanupHelper(IServiceScopeFactory serviceScopeFactory)
        {
            _scopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Jalankan setiap 5 menit
            _timer = new Timer(DeleteExpiredOtps, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DeleteExpiredOtps(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var expiredUsers = context.Users
                    .Where(u => u.OtpExpiredAt != null && u.OtpExpiredAt < DateTime.Now)
                    .ToList();

                if (expiredUsers.Any())
                {
                    foreach (var user in expiredUsers)
                    {
                        user.OtpCode = null;
                        user.OtpExpiredAt = null;
                    }

                    await context.SaveChangesAsync();
                    Console.WriteLine($"[OTP CLEANUP] {expiredUsers.Count} OTP expired berhasil dihapus.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }


    }
}
