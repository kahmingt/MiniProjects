using System.ComponentModel.DataAnnotations;

namespace WebApi.Model
{
    public class ProductCreateModel
    {
        public int CategoryID { get; set; }

        public string ProductName { get; set; } = "";

        public int SupplierID { get; set; }

        public short UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }

    }

    public class ProductRetrieveModel
    {
        public string? CategoryName { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; } = "";

        public string? SupplierName { get; set; }

        public short UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }

    }

    public class ProductUpdateModel
    {
        public int CategoryID { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; } = "";

        public int SupplierID { get; set; }

        public short UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }
    }

    public class ProductListModel
    {
        public string CategoryName { get; set; } = "";

        public int ProductID { get; set; }

        public string ProductName { get; set; } = "";

        public string SupplierName { get; set; } = "";

        public short UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }
    }
}