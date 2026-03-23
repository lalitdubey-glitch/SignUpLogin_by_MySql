namespace SignUpLogin_by_MySql.Models
{
    public class SignUpModel
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string pass { get; set; }
        public string? mob { get; set; }
        public DateOnly? dob { get; set; }
        public IFormFile? img { get; set; }
        public string? gender { get; set; }
        public List<string>? hobby { get; set; }
        public string? profession { get; set; }
        public string? pincode { get; set; }
        public string? state { get; set; }
        public string? dist { get; set; }
        public string? vill { get; set; }
    }
}
