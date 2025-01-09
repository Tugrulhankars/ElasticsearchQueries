using Elasticsearch.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ECommerceController : ControllerBase
{
    private readonly ECommerceRepository _eCommerRepository;
    public ECommerceController(ECommerceRepository eCommerRepository)
    {
        _eCommerRepository = eCommerRepository;
    }


    [HttpGet("term")]
    public async Task<IActionResult> TermQuery(string name)
    {
        var result = await _eCommerRepository.TermQuery(name);
        return Ok(result);

    }

    [HttpGet("prefix")]
    public async Task<IActionResult> PrefixQuery(string customerFullName)
    {
        var result = await _eCommerRepository.PrefixQuery(customerFullName);
        return Ok(result);
    }

    [HttpGet("range")]
    public async Task<IActionResult> RangeQuery(double from, double to)
    {
        var result = await _eCommerRepository.RangeQuery(from, to);
        return Ok(result);
    }

    [HttpGet("match")]
    public async Task<IActionResult> MatchAllQuery()
    {
        var result = await _eCommerRepository.MatchAllQuery();
        return Ok(result);
    }


    [HttpGet("pagination")]
    public async Task<IActionResult> PaginationQuery(int page, int size)
    {
        var result = await _eCommerRepository.PaginationQuery(page, size);
        return Ok(result);
    }

    [HttpGet("wildCard")]
    public async Task<IActionResult> WildCardQuery(string name)
    {
        var result = await _eCommerRepository.WildCardQuery(name);
        return Ok(result);
    }

    [HttpGet("fuzzQuery")]
    public async Task<IActionResult> FuzzyQuery(string customerName)
    {

        return Ok(await _eCommerRepository.FuzzQuery(customerName));

    }

    [HttpGet("matchQueryFullText")]
    public async Task<IActionResult> MatchQueryFullText(string category)
    {

        return Ok(await _eCommerRepository.MatchQueryFullText(category));

    }


    [HttpGet("matchBoolPrefixFullText")]
    public async Task<IActionResult> MatchBoolPrefixQueryFullText(string customerFullName)
    {

        return Ok(await _eCommerRepository.MatchBoolPrefixFullText(customerFullName));

    }

    [HttpGet("matchPhraseFullText")]
    public async Task<IActionResult> MatchPhraseQueryFullText(string customerFullName)
    {

        return Ok(await _eCommerRepository.MatchPhraseFullText(customerFullName));

    }

    [HttpGet("multiMatchQueryFullText")]
    public async Task<IActionResult> MatchPhrasePrefixQueryFullText(string customerFullName)
    {

        return Ok(await _eCommerRepository.MultiMatchQueryFullText(customerFullName));

    }
    [HttpGet]
    public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxfulTotalPrice, string categoryName, string menufacturer)
    {

        return Ok(await _eCommerRepository.CompoundQueryExampleOneAsync(cityName, taxfulTotalPrice, categoryName, menufacturer));

    }


  
 
}
