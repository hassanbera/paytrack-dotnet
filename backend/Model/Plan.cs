namespace App.Model;

public class Plan
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public string Title { get; set; } = "";
  public string ClientName { get; set; } = "";
  public string ClientEmail { get; set; } = "";
  public int TotalCents { get; set; } = 0;
}
