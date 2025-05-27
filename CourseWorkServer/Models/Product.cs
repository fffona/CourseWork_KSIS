namespace CourseWorkServer.Models
{
    public class Product
    {
        public int id { get; set; }
        private string name, image;
        private string? cost, description, category;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        public string? Description
        {
            get { return description; }
            set { description = value; }
        }
        public string? Category
        {
            get { return category; }
            set { category = value; }
        }
        public string? Cost
        {
            get { return cost; }
            set { cost = value; }
        }
    }
}
