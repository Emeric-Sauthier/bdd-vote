namespace VoteLibrairy
{
    public record ResultRecord
    {
        public string CandidateName { get; set; }

        public uint VoteNumber { get; set; } = 0;

        public decimal VotePercentage { get; set; } = decimal.Zero;

        public ResultRecord(string candidateName, uint voteNumber, decimal votePercentage)
        {
            CandidateName = candidateName;
            VoteNumber = voteNumber;
            VotePercentage = votePercentage;
        }

        public ResultRecord(string candidateName) : this(candidateName, 0, 0) { }
    }
}
