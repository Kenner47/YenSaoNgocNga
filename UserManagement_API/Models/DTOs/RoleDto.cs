namespace UserManagement_API.Models.DTOs
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class CreateRoleDto
    {
        public string RoleName { get; set; }
    }

    public class UpdateRoleDto
    {
        public string RoleName { get; set; }
    }
}