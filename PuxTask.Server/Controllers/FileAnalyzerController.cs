using MediatR;
using Microsoft.AspNetCore.Mvc;
using PuxTask.Server.Application;
using PuxTask.Server.Domain;

namespace PuxTask.Server.Controllers;

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