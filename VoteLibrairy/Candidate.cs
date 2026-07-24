namespace VoteLibrairy
{
    public record Candidate
    {
        public string Name { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public Candidate(string name, DateOnly dateOfBirth = default)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
        }
    }
}
