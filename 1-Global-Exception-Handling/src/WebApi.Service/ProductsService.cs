using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Data;
using WebApi.Model;

namespace WebApi.Service
{
    public interface IProductsService
    {
        Task CreateProductAsync(ProductCreateModel modelDTO);
        Task<Products> GetProductByIdAsync(int id);
        Task<ProductRetrieveModel> GetProductDetailsByIdAsync(int id);
        Task<List<ProductListModel>> GetProductListAsync();
        Task UpdateProductDetailsByIdAsync(ProductUpdateModel modelDTO);
        Task DeleteProductByIdAsync(Products product);
    }

    public class ProductsService : IProductsService
    {
        protected ApplicationDbContext _db { get; }

        public ProductsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateProductAsync(ProductCreateModel modelDTO)
        {
            Products modelDB = new Products()
            {
                CategoryId = modelDTO.CategoryID,
                ProductName = modelDTO.ProductName,
                SupplierId = modelDTO.SupplierID,
                UnitPrice = modelDTO.UnitPrice,
                UnitsInStock = modelDTO.UnitsInStock,
            };

            await _db.Set<Products>().AddAsync(modelDB);
            await CommitChangesAsync();
        }

        public async Task<ProductRetrieveModel> GetProductDetailsByIdAsync(int id)
        {
            var model = await _db.Set<Products>().Where(x => x.ProductId == id && !x.IsDeleted).AsNoTracking()
                                .Include(x => x.Categories)
                                .Include(x => x.Supplier)
                                .Select(x => new ProductRetrieveModel()
                                {
                                    CategoryName = x.Categories.CategoryName,
                                    ProductID = x.ProductId,
                                    ProductName = x.ProductName,
                                    SupplierName = x.Supplier.CompanyName,
                                    UnitsInStock = x.UnitsInStock,
                                    UnitPrice = x.UnitPrice,
                                })
                                .FirstOrDefaultAsync();
            return model!;
        }

        public async Task<Products> GetProductByIdAsync(int id)
        {
            var model = await _db.Set<Products>().Where(x => x.ProductId == id && !x.IsDeleted).AsNoTracking()
                                .Include(x => x.Categories)
                                .Include(x => x.Supplier)
                                .FirstOrDefaultAsync();
            return model!;
        }

        public async Task<List<ProductListModel>> GetProductListAsync()
        {
            return await _db.Set<Products>().Where(x => !x.IsDeleted)
                            .Include(x => x.Categories)
                            .Include(x => x.Supplier)
                            .AsNoTracking()
                            .Select(x => new ProductListModel()
                            {
                                CategoryName = x.Categories.CategoryName,
                                ProductID = x.ProductId,
                                ProductName = x.ProductName,
                                SupplierName = x.Supplier.CompanyName,
                                UnitsInStock = x.UnitsInStock,
                                UnitPrice = x.UnitPrice,
                            })
                            .ToListAsync();
        }

        public async Task UpdateProductDetailsByIdAsync(ProductUpdateModel modelDTO)
        {
            Products modelDB = new Products()
            {
                CategoryId = modelDTO.CategoryID,
                ProductId = modelDTO.ProductID,
                ProductName = modelDTO.ProductName,
                SupplierId = modelDTO.SupplierID,
                UnitsInStock = modelDTO.UnitsInStock,
                UnitPrice = modelDTO.UnitPrice,
            };

            _db.Entry(modelDB).Property(x => x.ProductId).IsModified = false;
            _db.Set<Products>().Update(modelDB);
            await CommitChangesAsync();
        }

        public async Task DeleteProductByIdAsync(Products product)
        {
            product.IsDeleted = true;
            _db.Entry(product).Property(x => x.ProductId).IsModified = false;
            _db.Set<Products>().Update(product);
            await CommitChangesAsync();
        }

        private async Task CommitChangesAsync()
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    transaction.Dispose();
                    throw new Exception(ex.Message);
                }
            });
        }
    }
}