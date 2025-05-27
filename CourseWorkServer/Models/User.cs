namespace CourseWorkServer.Models
{
    public class User
    {
        public int id { get; set; }
        private string login, password, isCashier;
        private string? cart, history, place;
        private int purchaseStatus;
        public string Login
        {
            get { return login; }
            set { login = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string? Cart
        {
            get { return cart; }
            set { cart = value; }
        }
        public string? History
        {
            get { return history; }
            set { history = value; }
        }
        public int PurchaseStatus
        {
            get { return purchaseStatus; }
            set { purchaseStatus = value; }
        }
        public string IsCashier
        {
            get { return isCashier; }
            set { isCashier = value; }
        }
        public string? Place
        {
            get { return place; }
            set { place = value; }
        }

        public User() { }
        public User(string login, string password, string? cart = null, string? history = null, int purchaseStatus = 0, string isCashier = "false", string? place = null)
        {
            this.login = login;
            this.password = password;
            this.cart = cart;
            this.history = history;
            this.purchaseStatus = purchaseStatus;
            this.isCashier = isCashier;
            this.place = place;
        }
    }
}
