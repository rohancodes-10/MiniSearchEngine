
using MiniSearchEnginee;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



var crawler = new Crawler("https://en.wikipedia.org/wiki/Web_crawler");
var pages = await crawler.CrawlAsync(10);

var index = new InvertedIndex();
int docid = 0;
foreach (var page in pages)
{
    index.AddDocument(docid, page.Title, page.Url, page.Text);
    docid++;
}

var scorer = new TfIdfScore(index);
var searchEngine = new SearchEngineService(index, scorer);

builder.Services.AddSingleton(searchEngine);
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Search}/{action=Results}/{id?}")
    .WithStaticAssets();


app.Run();
