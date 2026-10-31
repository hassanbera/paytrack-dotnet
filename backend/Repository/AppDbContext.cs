using Microsoft.EntityFrameworkCore;
using App.Model;
namespace App.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Plan> Plans => Set<Plan>();
  public DbSet<Installment> Installments => Set<Installment>();
  public DbSet<Payment> Payments => Set<Payment>();
  protected override void OnModelCreating(ModelBuilder b)
  {
    b.Entity<Plan>(e => { e.ToTable("plans"); e.HasKey(x => x.Id); e.Property(x => x.Id).HasColumnName("id"); e.Property(x => x.CreatedAt).HasColumnName("created_at"); e.Property(x => x.Title).HasColumnName("title").HasMaxLength(120).IsRequired(); e.Property(x => x.ClientName).HasColumnName("client_name").HasMaxLength(100).IsRequired(); e.Property(x => x.ClientEmail).HasColumnName("client_email").HasMaxLength(200).IsRequired(); e.Property(x => x.TotalCents).HasColumnName("total_cents"); });
    b.Entity<Installment>(e => { e.ToTable("installments"); e.HasKey(x => x.Id); e.Property(x => x.Id).HasColumnName("id"); e.Property(x => x.CreatedAt).HasColumnName("created_at"); e.Property(x => x.PlanId).HasColumnName("plan_id"); e.Property(x => x.Number).HasColumnName("number"); e.Property(x => x.AmountCents).HasColumnName("amount_cents"); e.Property(x => x.PaidCents).HasColumnName("paid_cents"); e.Property(x => x.DueDate).HasColumnName("due_date"); });
    b.Entity<Payment>(e => { e.ToTable("payments"); e.HasKey(x => x.Id); e.Property(x => x.Id).HasColumnName("id"); e.Property(x => x.CreatedAt).HasColumnName("created_at"); e.Property(x => x.InstallmentId).HasColumnName("installment_id"); e.Property(x => x.AmountCents).HasColumnName("amount_cents"); e.Property(x => x.Note).HasColumnName("note").HasMaxLength(300).IsRequired(); e.Property(x => x.IdempotencyKey).HasColumnName("idempotency_key"); });
    b.Entity<Installment>().HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
    b.Entity<Payment>().HasOne<Installment>().WithMany().HasForeignKey(x => x.InstallmentId).OnDelete(DeleteBehavior.Restrict);
  }
}
