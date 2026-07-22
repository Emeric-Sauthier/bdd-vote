using VoteLibrairy;

namespace VoteTests.StepDefinitions
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        private readonly Vote _vote = new Vote("Test Vote", "This vote has been created for testing.");
        private List<string> _candidates = new();
        private Dictionary<VoteRound, Dictionary<string, uint>> _results = new();
        private Dictionary<VoteRound, string?> _roundWinner = new();
        private Dictionary<VoteRound, string?> _expectedWinners = new();

        #region Candidates

        [Given("candidates are")]
        public void GivenTheCandidates(Table table)
        {
            foreach (DataTableRow row in table.Rows)
            {
                string value = row[0];
                _candidates.Add(value);
            }
        }

        [When("add the candidates to the vote")]
        public void WhenAddTheCandidates()
        {
            _vote.AddCandidates(_candidates);
        }

        [Then("candidates should be")]
        public void ThenCandidatesShouldBe(Table table)
        {
            CollectionAssert.AreEquivalent(_candidates, _vote.Candidates[_vote.Round]);
        }

        #endregion

        #region Vote states & results

        [Given("first round results are")]
        public void GivenFirstRoundResultsAre(Table table)
        {
            _results[VoteRound.First] = new();

            foreach(DataTableRow row in table.Rows)
            {
                string candidateName = row[0];
                uint voteNumber = uint.Parse(row[1]);
                _results[VoteRound.First].Add(candidateName, voteNumber);
            }
        }

        [Given("second round results are")]
        public void GivenSecondRoundResultsAre(Table table)
        {
            _results[VoteRound.Second] = new();

            foreach (DataTableRow row in table.Rows)
            {
                string candidateName = row[0];
                uint voteNumber = uint.Parse(row[1]);
                _results[VoteRound.Second].Add(candidateName, voteNumber);
            }
        }

        [When("open vote")]
        public void WhenOpenVote()
        {
            _vote.StartVote();
        }

        [When("add round results to vote")]
        public void WhenAddRoundResults()
        {
            foreach (string key in _results[_vote.Round].Keys)
            {
                _vote.AddVotes(key, _results[_vote.Round][key]);
            }
        }

        [When("process current round")]
        public void WhenProcessCurrentRound()
        {
            _vote.StartVote();

            foreach (string key in _results[_vote.Round].Keys)
            {
                _vote.AddVotes(key, _results[_vote.Round][key]);
            }

            _vote.CloseVote();

            _roundWinner[_vote.Round] = _vote.GetVoteWinner();
        }

        [Then("first round results should be")]
        public void ThenFirstRoundResultsShouldBe(Table table)
        {
            List<ResultRecord> expectedResults = new List<ResultRecord>();
            foreach (DataTableRow row in table.Rows)
            {
                expectedResults.Add(new ResultRecord(row[0], uint.Parse(row[1]), decimal.Parse(row[2])));
            }

            CollectionAssert.AreEquivalent(expectedResults, _vote.Results[VoteRound.First]);
        }

        [Then("second round results should be")]
        public void ThenSecondRoundResultsShouldBe(Table table)
        {
            List<ResultRecord> expectedResults = new List<ResultRecord>();
            foreach (DataTableRow row in table.Rows)
            {
                expectedResults.Add(new ResultRecord(row[0], uint.Parse(row[1]), decimal.Parse(row[2])));
            }

            CollectionAssert.AreEquivalent(expectedResults, _vote.Results[VoteRound.Second]);
        }

        [Then("first round winner should be (.*)")]
        public void ThenFirstVoteWinnerShouldBe(string value)
        {
            string? expectedWinner = value;
            if (value == "null")
            {
                expectedWinner = null;
            }

            Assert.AreEqual(expectedWinner, _roundWinner[VoteRound.First]);
        }

        [Then("second round winner should be (.*)")]
        public void ThenSecondVoteWinnerShouldBe(string value)
        {
            string? expectedWinner = value;
            if (value == "null")
            {
                expectedWinner = null;
            }

            Assert.AreEqual(expectedWinner, _roundWinner[VoteRound.Second]);
        }

        #endregion
    }
}
