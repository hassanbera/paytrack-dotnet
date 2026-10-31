namespace App.Model;

public class Payment
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public Guid InstallmentId { get; set; } = Guid.Empty;
  public int AmountCents { get; set; } = 0;
  public string Note { get; set; } = "";
  public Guid IdempotencyKey { get; set; } = Guid.Empty;
}
