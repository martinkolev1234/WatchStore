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
    public IActionResult Create([FromBody] CreateClientRequest request)
    {
        var validation = createValidator.Validate(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var clientEntity = mapper.Map<Client>(request);

        var createdClient = clientService.AddClient(clientEntity);

        var response = mapper.Map<ClientResponse>(createdClient);
        return CreatedAtAction(nameof(GetById), new { id = createdClient.Id }, response);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var clients = clientService.GetAllClients();
        return Ok(mapper.Map<IEnumerable<ClientResponse>>(clients));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var client = clientService.GetClientById(id);

        if (client is null) return NotFound();

        return Ok(mapper.Map<ClientResponse>(client));
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateClientRequest request)
    {
        var validation = updateValidator.Validate(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        clientService.UpdateClient(id, request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        clientService.DeleteClient(id);
        return NoContent();
    }

    [HttpPost("add-funds")]
    public IActionResult AddFunds([FromQuery] Guid clientId, [FromQuery] decimal amount)
    {
        clientService.AddFunds(clientId, amount);

        return Ok("Funds added successfully");
    }
}