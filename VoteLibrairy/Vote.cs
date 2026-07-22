using System.Data;

namespace VoteLibrairy
{
    public class Vote
    {
        // Name of the vote session
        public string Name { get; private set; }

        // Description of the vote session
        public string Description { get; private set; }

        // Candidates of the vote session (per round): key = Round, value = List of candidates

        public Dictionary<VoteRound, List<string>> Candidates { get; private set; }

        // Round of the vote
        public VoteRound Round { get; private set; }

        // Total number of votes
        public Dictionary<VoteRound, uint> TotalVotes { get; private set; }

        // Result of the vote session (per round): key = Round, value = List of results
        public Dictionary<VoteRound, List<ResultRecord>> Results { get; private set; }

        // State of the vote session
        public VoteState State { get; private set; }

        public Vote(string name, string description)
        {
            Name = name;
            Description = description;
            Candidates = new();
            Results = new();
            TotalVotes = new();
            Round = VoteRound.First;
        }

        public string? GetVoteWinner()
        {
            if (State != VoteState.Closed)
            {
                throw new InvalidOperationException();
            }

            IOrderedEnumerable<ResultRecord> orderedResults = Results[Round].OrderByDescending(r => r.VotePercentage);
            
            ResultRecord bestResult = orderedResults.First();
            ResultRecord secondBestResult = orderedResults.Skip(1).First();
            bool equality = bestResult.VotePercentage == secondBestResult.VotePercentage;

            if (bestResult.VotePercentage > 50 || (Round == VoteRound.Second && !equality))
            {
                return bestResult.CandidateName;
            }
            else if (Round == VoteRound.First)
            {
                Candidates[VoteRound.Second] = orderedResults.Take(2).Select(r => r.CandidateName).ToList();
                State = VoteState.InComing;
                Round = VoteRound.Second;
                return null;
            }

            return null;
        }

        public void AddCandidate(string candidate)
        {
            if (!Candidates.ContainsKey(Round))
            {
                Candidates[Round] = new();
            }

            if (Candidates[Round].Contains(candidate) || State != VoteState.InComing)
            {
                throw new Exception();
            }

            Candidates[Round].Add(candidate);
        }

        public void AddCandidates(List<string> candidates)
        {
            if (!Candidates.ContainsKey(Round))
            {
                Candidates[Round] = new();
            }
            
            Candidates[Round].AddRange(candidates);
        }

        public void AddVotes(string candidate, uint voteNumber)
        {
            if (State != VoteState.Opened)
            {
                throw new Exception();
            } else if (!Results.ContainsKey(Round))
            {
                throw new Exception();
            }
            
            var c = Results[Round].FirstOrDefault(c => c.CandidateName == candidate, null);
            
            if (c is null)
            {
                throw new Exception();
            }

            c.VoteNumber += voteNumber;
            TotalVotes[Round] += voteNumber;
        }

        public void StartVote()
        {
            if (State != VoteState.InComing)
            {
                throw new Exception();
            }

            State = VoteState.Opened;
            TotalVotes[Round] = 0;
            
            // Add the default value for results (voteNumber & votePercentage to 0)
            Results[Round] = Candidates[Round].Select(c => new ResultRecord(c)).ToList();
        }

        public void CloseVote()
        {
            if (State != VoteState.Opened)
            {
                throw new Exception();
            }

            State = VoteState.Closed;

            UpdateVotePercentages();
        }

        private void UpdateVotePercentages()
        {
            Results[Round] = Results[Round].Select(r => new ResultRecord(r.CandidateName, r.VoteNumber, 100 * r.VoteNumber / TotalVotes[Round])).ToList();
        }
    }
}
