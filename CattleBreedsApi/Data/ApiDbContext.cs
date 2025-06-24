using CattleBreedsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleBreedsApi.Data;

public partial class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public virtual DbSet<UploadFile> UploadFiles { get; set; } = null!;
    public virtual DbSet<CattlePredictionJob> CattlePredictionJobs { get; set; } = null!;
    public virtual DbSet<CattlePrediction> CattlePredictions { get; set; } = null!;
}