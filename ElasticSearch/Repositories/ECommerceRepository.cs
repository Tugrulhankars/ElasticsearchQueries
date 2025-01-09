using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Elasticsearch.Models;

namespace Elasticsearch.Repositories;

public class ECommerceRepository
{
    private readonly ElasticsearchClient _elasticsearchClient;
    private const string indexName = "kibana_sample_data_ecommerce";

    public ECommerceRepository()
    {
        ElasticsearchClientSettings settings = new(new Uri("http://localhost:9200"));
        settings.Authentication(new BasicAuthentication("elastic","changeme"));
        _elasticsearchClient = new ElasticsearchClient(settings);
    }



    public async Task<List<ECommerce>> TermQuery(string customerFullName)
    {
        var searchResult = _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Query(q=>q.Term(t=>t.Field("customer_fullname").Value(customerFullName))));

        return searchResult.Result.Documents.ToList();
    }

    public async Task<List<ECommerce>> PrefixQuery(string customerFullName)
    {
        var searchResult = _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Query(q=>q.Prefix(p=>p.Field(f=>f.CustomerFullName.Suffix("keyword")).Value(customerFullName))));

        return searchResult.Result.Documents.ToList();
    }

    public async Task<List<ECommerce>> RangeQuery(double fromPrice ,double toPrice)
    {
        var searchResult = _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Query(q=>q.Range(r=>r.NumberRange(n=>n.Field(f=>f.TaxfulTotalPrice).Gte(fromPrice).Lte(toPrice)))));
        return searchResult.Result.Documents.ToList();
    }

    public async Task<List<ECommerce>> MatchAllQuery()
    {
        var searchResult = _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Query(q=>q.Match(m=>m.Field(f=>f.CustomerFullName).Query("shaw"))));
        return searchResult.Result.Documents.ToList();
    }

    public async Task<List<ECommerce>> PaginationQuery(int page,int pageSize) {
        var pageFrom = (page - 1) * pageSize;
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Size(pageSize).From(pageFrom)
        .Query(q=>q.MatchAll(m=>m.GetType())));

        return result.Documents.ToList();
    }

    public async Task<List<ECommerce>> WildCardQuery(string customerFullName)
    {
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Query(q=>q.Wildcard(w=>w.Field(f=>f.CustomerFullName
        .Suffix("keyword"))
        .Wildcard(customerFullName))));

        return result.Documents.ToList();
    }

    public async Task<List<ECommerce>> FuzzQuery(string customerName)
    {
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Query(q=>q.Fuzzy(f=>f.Field(f=>f.CustomerFirstName.Suffix("keyword"))
        .Value(customerName)
        .Fuzziness(new Fuzziness(3))))
        .Sort(sort=>sort
        .Field(f=>f.TaxfulTotalPrice,new FieldSort() { Order=SortOrder.Desc})));

        return result.Documents.ToList();
    }

    public async Task<List<ECommerce>> MatchQueryFullText(string categoryName)
    {
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Size(100).Query(q=>q.Match(m=>m
        .Field(f=>f.Category)
        .Query(categoryName)
        .Operator(Operator.And))));

        return result.Documents.ToList();
    }

    public async Task<List<ECommerce>> MultiMatchQueryFullText(string name)
    {
        var result =await _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Size(1000).Query(q=>q
        .MultiMatch(mm=>mm
        .Fields(new Field("customer_first_name")
        .And(new Field("customer_last_name"))
        .And(new Field("customer_full_name")))
        .Query(name))));
        
        return result.Documents.ToList();

    }

    public async Task<List<ECommerce>> MatchBoolPrefixFullText(string customerFullName)
    {
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s=>s
        .Index(indexName).Size(1000).Query(q=>q
        .MatchBoolPrefix(m=>m
        .Field(f=>f.CustomerFullName)
        .Query(customerFullName))));

        return result.Documents.ToList();
    }


    public async Task<List<ECommerce>> MatchPhraseFullText(string customerFullName)
    {
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s => s.Index(indexName)
           .Size(300).Query(q => q
               .MatchPhrase(m => m
                   .Field(f => f.CustomerFullName)
                   .Query(customerFullName))));

        return result.Documents.ToList();
    }


    public async Task<List<ECommerce>> MatchPhrasePrefixFullText(string customerFullName)
    {

        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(1000).Query(q => q
                .MatchPhrasePrefix(m => m
                    .Field(f => f.CustomerFullName)
                    .Query(customerFullName))));

        return result.Documents.ToList();

    }

    public async Task<List<ECommerce>> CompoundQueryExampleOneAsync(string cityName, double taxfulTotalPrice, string categoryName, string menufacturer)
    {
        var result = await _elasticsearchClient.SearchAsync<ECommerce>(s => s.Index(indexName)
         .Size(1000).Query(q => q
             .Bool(b => b
                 .Must(m => m
                     .Term(t => t
                         .Field("geoip.city_name")
                         .Value(cityName)))
                 .MustNot(mn => mn
                     .Range(r => r
                         .NumberRange(nr => nr
                             .Field(f => f.TaxfulTotalPrice)
                             .Lte(taxfulTotalPrice))))
                 .Should(s => s.Term(t => t
                     .Field(f => f.Category.Suffix("keyword"))
                     .Value(categoryName)))
                 .Filter(f => f
                     .Term(t => t
                         .Field("manufacturer.keyword")
                         .Value(menufacturer))))


         ));

        return result.Documents.ToList();
    }
}
