using API.Data;
using API.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddOData(opt =>
        opt.Select().Filter().Expand().OrderBy().SetMaxTop(null).Count()
            .AddRouteComponents("odata", GetEdmModel()));

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.MapControllers();
app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    builder.EntitySet<Compound>("Compounds");
    builder.EntitySet<Cycle>("Cycles");
    builder.EntitySet<DoseEvent>("DoseEvents");

    // Navigation exposure (optional)
    builder.EntityType<Cycle>()
        .HasMany(c => c.DoseEvents)
        .Contained();

    builder.EntityType<Compound>()
        .HasMany(c => c.DoseEvents)
        .Contained();

    return builder.GetEdmModel();
}
