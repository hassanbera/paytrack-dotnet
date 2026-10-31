using Microsoft.AspNetCore.Mvc;
using App.Service;
using App.Repository;
using System.Text.Json;
namespace App.Controller;

[ApiController]
[Route("api")]
public class ApiController(DomainService service, IDomainRepository repo) : ControllerBase
{
  [HttpGet("health")] public async Task<object> Health() { if (!await repo.Healthy()) throw new Exception("Database unavailable"); return new { data = new { status = "ok" } }; }
  [HttpGet("plans")] public async Task<object> Action0() { return new { data = await service.Plans() }; }
  [HttpPost("plans")] public async Task<object> Action1([FromBody] JsonElement j) { return new { data = await service.CreatePlan(j) }; }
  [HttpGet("plans/{id}")] public async Task<object> Action2(Guid id) { return new { data = await service.Plan(id) }; }
  [HttpPost("installments/{id}/payments")] public async Task<object> Action3(Guid id, [FromBody] JsonElement j) { return new { data = await service.Pay(id, j) }; }
}
