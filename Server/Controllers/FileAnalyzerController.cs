using FileVersioning.Server.Application;
using FileVersioning.Server.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileVersioning.Server.Controllers;

[Route("api/file-analyzer")]
[ApiController]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class FileAnalyzerController : ControllerBase
{
    private readonly IMediator _mediator;

    public FileAnalyzerController(IMediator mediator) {
        _mediator = mediator;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<FolderAnalysisResult>> AnalyzeDirectory([FromBody] FolderAnalysisRequest analysisRequest)
    {

        var analysisResult = await _mediator.Send(new FolderAnalyzer {FolderPath = analysisRequest.FolderPath});
        return Ok(analysisResult);
    }
}