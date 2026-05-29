namespace WatchStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(
    IClientService clientService,
    IMapper mapper,
    IValidator<CreateClientRequest> createValidator,
    IValidator<UpdateClientRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var validation = await createValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var clientEntity = mapper.Map<Client>(request);

        var createdClient = await clientService.AddClientAsync(clientEntity);

        var response = mapper.Map<ClientResponse>(createdClient);
        return CreatedAtAction(nameof(GetById), new { id = createdClient.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await clientService.GetAllClientsAsync();
        return Ok(mapper.Map<IEnumerable<ClientResponse>>(clients));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var client = await clientService.GetClientByIdAsync(id);

        if (client is null) return NotFound();

        return Ok(mapper.Map<ClientResponse>(client));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequest request)
    {
        var validation = await updateValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        await clientService.UpdateClientAsync(id, request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await clientService.DeleteClientAsync(id);
        return NoContent();
    }

    [HttpPost("add-funds")]
    public async Task<IActionResult> AddFunds([FromQuery] Guid clientId, [FromQuery] decimal amount)
    {
        await clientService.AddFundsAsync(clientId, amount);

        return Ok("Funds added successfully");
    }
}