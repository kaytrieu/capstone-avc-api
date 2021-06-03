namespace AVC.Dtos.AccountDtos
{
    public class AccountReadAfterAuthenDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Avatar { get; set; }
    }
}
