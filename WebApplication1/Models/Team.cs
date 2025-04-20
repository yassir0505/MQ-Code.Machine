namespace WebApplication1.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string LeaderUsername { get; set; } = string.Empty;

        public List<TeamMember> Members { get; set; } = new();
    }

    public class TeamMember
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string MemberUsername { get; set; } = string.Empty;

        public Team? Team { get; set; }
    }
}
