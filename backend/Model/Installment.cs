namespace App.Model;

public class Installment
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public Guid PlanId { get; set; } = Guid.Empty;
  public int Number { get; set; } = 0;
  public int AmountCents { get; set; } = 0;
  public int PaidCents { get; set; } = 0;
  public DateOnly DueDate { get; set; } = default;
}
