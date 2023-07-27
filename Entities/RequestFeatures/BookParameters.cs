namespace Entities.RequestFeatures
{
    public class BookParameters : RequestParameters 
    {
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; } = 1000 /*uint.MaxValue*/;
        public bool ValidPriceRange => MaxPrice > MinPrice;

        public String? SearchTerm { get; set; }

        public BookParameters()   // Sıralama işlemi için ctor oluşturduk.
        {
            OrderBy = "id";
        }
    }
}
