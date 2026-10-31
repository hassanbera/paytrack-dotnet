using App.Model;
using App.Repository;
using App.Common;
using static App.Common.Input;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace App.Service;

public class DomainService(IDomainRepository repo)
{

  public async Task<object> Plans() => await repo.Query<Plan>().OrderByDescending(x => x.CreatedAt).ToListAsync();
  public async Task<object> Plan(Guid id) { var p = await repo.Find<Plan>(id); var list = await repo.Query<Installment>().Where(x => x.PlanId == id).OrderBy(x => x.Number).ToListAsync(); var ids = list.Select(x => x.Id).ToList(); var payments = await repo.Query<Payment>().Where(x => ids.Contains(x.InstallmentId)).OrderBy(x => x.CreatedAt).ToListAsync(); return new { plan = p, installments = list.Select(i => new { installment = i, payments = payments.Where(x => x.InstallmentId == i.Id) }) }; }
  public async Task<object> CreatePlan(JsonElement j) { await using var tx = await repo.Begin(); var p = new Plan { Title = Text(j, "title", 120, true), ClientName = Text(j, "clientName", 100, true), ClientEmail = Email(j), TotalCents = Number(j, "totalCents", 1, 1000000000) }; int count = Number(j, "count", 1, 60); if (count > p.TotalCents) throw new ApiException(400, "Taksit tutarı en az bir kuruş olmalı."); var first = Date(j, "firstDueDate"); repo.Add(p); for (int n = 0; n < count; n++) { var month = new DateOnly(first.Year, first.Month, 1).AddMonths(n); int last = DateTime.DaysInMonth(month.Year, month.Month); int day = first.Day == DateTime.DaysInMonth(first.Year, first.Month) ? last : Math.Min(first.Day, last); repo.Add(new Installment { PlanId = p.Id, Number = n + 1, AmountCents = p.TotalCents / count + (n < p.TotalCents % count ? 1 : 0), DueDate = new DateOnly(month.Year, month.Month, day) }); } await repo.Save(); await tx.CommitAsync(); return p; }
  public async Task<object> Pay(Guid id, JsonElement j) { int amount = Number(j, "amountCents", 1, 1000000000); var key = Id(j, "idempotencyKey"); var note = Text(j, "note", 300); await using var tx = await repo.Begin(); var i = await repo.Lock<Installment>(id); var old = await repo.Query<Payment>().SingleOrDefaultAsync(x => x.IdempotencyKey == key); if (old != null) { if (old.InstallmentId != id || old.AmountCents != amount || old.Note != note) throw new ApiException(409, "Tekrar anahtarı farklı bir işlem için kullanılmış."); await tx.CommitAsync(); return old; } if (amount > i.AmountCents - i.PaidCents) throw new ApiException(409, "Tahsilat kalan bakiyeyi aşamaz."); var p = new Payment { InstallmentId = id, AmountCents = amount, IdempotencyKey = key, Note = note }; i.PaidCents += amount; repo.Add(p); await repo.Save(); await tx.CommitAsync(); return p; }

}
