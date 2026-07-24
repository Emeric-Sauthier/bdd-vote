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

        public Dictionary<VoteRound, List<Candidate>> Candidates { get; private set; }

        // Round of the vote
        public VoteRound Round { get; private set; }

        // Total number of votes
        public Dictionary<VoteRound, uint> TotalVotes { get; private set; }

        public Dictionary<VoteRound, uint> BlankVotes { get; private set; }

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
            BlankVotes = new();
            Round = VoteRound.First;
        }

        public string? GetRoundWinner()
        {
            if (State != VoteState.Closed)
            {
                throw new InvalidOperationException($"Unable to get the round winner, vote is '{State.ToString()}'.");
            }

            Dictionary<string, Candidate> candidatesByName = Candidates[Round].ToDictionary(c => c.Name);

            IOrderedEnumerable<ResultRecord> orderedResults = Results[Round]
                .OrderByDescending(r => r.VoteNumber)
                .ThenBy(r => candidatesByName[r.CandidateName].DateOfBirth);

            ResultRecord bestResult = orderedResults.First();
            ResultRecord secondBestResult = orderedResults.Skip(1).First();
            bool equality = bestResult.VotePercentage == secondBestResult.VotePercentage;

            if (bestResult.VotePercentage > 50 || (Round == VoteRound.Second && !equality))
            {
                return bestResult.CandidateName;
            }
            else if (Round == VoteRound.First)
            {
                Candidates[VoteRound.Second] = orderedResults
                    .Take(2)
                    .Select(r => candidatesByName[r.CandidateName])
                    .ToList();
                State = VoteState.InComing;
                Round = VoteRound.Second;
                return null;
            }

            return null;
        }

        public void AddCandidate(Candidate candidate)
        {
            if (State != VoteState.InComing)
            {
                throw new InvalidOperationException($"Unable to add candidate to vote, the vote is '{State.ToString()}'.");
            }

            if (!Candidates.ContainsKey(Round))
            {
                Candidates[Round] = new();
            }

            if (Candidates[Round].Any(c => c.Name == candidate.Name))
            {
                throw new Exception($"Unable to add '{candidate.Name}', duplicated.");
            }

            Candidates[Round].Add(candidate);
        }

        public void AddCandidates(List<Candidate> candidates)
        {
            foreach (Candidate candidate in candidates)
            {
                AddCandidate(candidate);
            }
        }

        public void AddVotes(string candidate, uint voteNumber)
        {
            if (State != VoteState.Opened)
            {
                throw new Exception($"Unable to add vote result, the vote is '{State.ToString()}'.");
            }
            else if (!Results.ContainsKey(Round))
            {
                throw new Exception($"Unable to add result for {Round.ToString()}. No results found for the round.");
            }
            
            var c = Results[Round].FirstOrDefault(c => c.CandidateName == candidate, null);
            
            if (c is null)
            {
                throw new Exception($"Unable to add result for '{candidate}'. Not found.");
            }

            c.VoteNumber += voteNumber;
            TotalVotes[Round] += voteNumber;
        }

        public void AddBlankVotes(uint voteNumber)
        {
            if (State != VoteState.Opened)
            {
                throw new Exception($"Unable to add blank vote, the vote is '{State.ToString()}'.");
            }

            BlankVotes[Round] += voteNumber;
        }

        public void StartVote()
        {
            if (State != VoteState.InComing)
            {
                throw new Exception($"Unable to start vote, the vote is '{State.ToString()}'.");
            }
            if (!Candidates.ContainsKey(Round) || Candidates[Round].Count == 0)
            {
                throw new Exception("Unable to start vote, no candidates assigned.");
            }

            State = VoteState.Opened;
            TotalVotes[Round] = 0;
            BlankVotes[Round] = 0;
            
            // Add the default value for results (voteNumber & votePercentage to 0)
            Results[Round] = Candidates[Round].Select(c => new ResultRecord(c.Name)).ToList();
        }

        public void CloseVote()
        {
            if (State != VoteState.Opened)
            {
                throw new Exception($"Unable to close the vote, the vote is '{State}'.");
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
