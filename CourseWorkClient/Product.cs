using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkClient
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

        public Product() { }
        public Product(string name, string image, string? description, string? category, string? cost)
        {
            this.name = name;
            this.image = image;
            this.description = description;
            this.category = category;
            this.cost = cost;
        }
    }
}
