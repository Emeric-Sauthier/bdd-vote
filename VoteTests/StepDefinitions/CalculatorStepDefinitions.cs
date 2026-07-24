using System.Globalization;
using System.Linq.Expressions;
using VoteLibrairy;

namespace VoteTests.StepDefinitions
{
    [Binding]
    public sealed class VoteStepDefinitions
    {
        private readonly Vote _vote = new Vote("Test Vote", "This vote has been created for testing.");
        private List<Candidate> _candidates = new();
        private Dictionary<VoteRound, Dictionary<string, uint>> _results = new();
        private Dictionary<VoteRound, uint> _blankVotes = new();
        private Dictionary<VoteRound, string?> _roundWinner = new();
        private Exception? _exception;

        [Given("candidates are")]
        public void GivenTheCandidates(Table table)
        {
            bool hasDob = table.Header.Contains("Date of birth");

            foreach (DataTableRow row in table.Rows)
            {
                string name = row[0];
                DateOnly dob = hasDob
                    ? DateOnly.ParseExact(row[1], "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : default;

                _candidates.Add(new Candidate(name, dob));
            }
        }

        [Given("first round results are")]
        public void GivenFirstRoundResultsAre(Table table)
        {
            _results[VoteRound.First] = new();

            foreach (DataTableRow row in table.Rows)
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

        [Given("first round blank votes are (.*)")]
        public void GivenFirstRoundBlankVotesAre(uint blankVotes)
        {
            _blankVotes[VoteRound.First] = blankVotes;
        }

        [When("add the candidates to the vote")]
        public void WhenAddTheCandidates()
        {
            try
            {
                _vote.AddCandidates(_candidates);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [When("open vote")]
        public void WhenOpenVote()
        {
            try
            {
                _vote.StartVote();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [When("close vote")]
        public void WhenCloseVote()
        {
            try
            {
                _vote.CloseVote();
            }
            catch (Exception ex)
            {

                _exception = ex;
            }
        }

        [When("set round results")]
        public void WhenSetRoundResults()
        {
            foreach (string key in _results[_vote.Round].Keys)
            {
                _vote.AddVotes(key, _results[_vote.Round][key]);
            }
        }

        [When("get round winner")]
        public void WhenGetRoundWinner()
        {
            try
            {
                _vote.GetRoundWinner();
            }
            catch (Exception ex)
            {
                _exception = ex;
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

            if (_blankVotes.TryGetValue(_vote.Round, out uint blanks))
            {
                _vote.AddBlankVotes(blanks);
            }

            _vote.CloseVote();

            _roundWinner[_vote.Round] = _vote.GetRoundWinner();
        }

        [Then("candidates should be")]
        public void ThenCandidatesShouldBe(Table table)
        {
            CollectionAssert.AreEquivalent(_candidates, _vote.Candidates[_vote.Round]);
        }

        [Then("second round candidates should be")]
        public void ThenSecondRoundCandidatesShouldBe(Table table)
        {
            List<string> expectedNames = new();
            foreach (DataTableRow row in table.Rows)
            {
                expectedNames.Add(row[0]);
            }

            List<Candidate> expected = _candidates.Where(c => expectedNames.Contains(c.Name)).ToList();

            CollectionAssert.AreEquivalent(expected, _vote.Candidates[VoteRound.Second]);
        }

        [Then("first round blank votes should be (.*)")]
        public void ThenFirstRoundBlankVotesShouldBe(uint blankVotes)
        {
            Assert.AreEqual(blankVotes, _vote.BlankVotes[VoteRound.First]);
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

        [Then("should throw an error with message (.*)")]
        public void ThenShouldThrowAnError(string message)
        {
            Assert.IsNotNull(_exception);
            Assert.AreEqual(message, _exception.Message);
        }
    }
}
