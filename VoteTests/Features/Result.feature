Feature: Result

Scenarios to test the results and states of vote

@mytag
Scenario: First round win
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	And first round results are
	| Candidates | Vote Number |
	| Candidate1 |          55 |
	| Candidate2 |          20 |
	| Candidate3 |          25 |
	When add the candidates to the vote
	And process current round
	Then first round results should be
	| Candidates | Vote Number | Vote Percentage |
	| Candidate1 |          55 |              55 |
	| Candidate2 |          20 |              20 |
	| Candidate3 |          25 |              25 |
	And first round winner should be Candidate1

Scenario: Second round win
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	And first round results are
	| Candidates | Vote Number |
	| Candidate1 |          50 |
	| Candidate2 |          20 |
	| Candidate3 |          30 |
	And second round results are
	| Candidates | Vote Number |
	| Candidate1 |          60 |
	| Candidate3 |          40 |
	When add the candidates to the vote
	And process current round
	And process current round
	Then first round results should be
	| Candidates | Vote Number | Vote Percentage |
	| Candidate1 |          50 |              50 |
	| Candidate2 |          20 |              20 |
	| Candidate3 |          30 |              30 |
	And first round winner should be null
	And second round results should be
	| Candidates | Vote Number | Vote Percentage |
	| Candidate1 |          60 |              60 |
	| Candidate3 |          40 |              40 |
	And second round winner should be Candidate1

Scenario: Second round without winner
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	And first round results are
	| Candidates | Vote Number |
	| Candidate1 |          50 |
	| Candidate2 |          20 |
	| Candidate3 |          30 |
	And second round results are
	| Candidates | Vote Number |
	| Candidate1 |          30 |
	| Candidate3 |          30 |
	When add the candidates to the vote
	And process current round
	And process current round
	Then first round results should be
	| Candidates | Vote Number | Vote Percentage |
	| Candidate1 |          50 |              50 |
	| Candidate2 |          20 |              20 |
	| Candidate3 |          30 |              30 |
	And first round winner should be null
	And second round results should be
	| Candidates | Vote Number | Vote Percentage |
	| Candidate1 |          30 |              50 |
	| Candidate3 |          30 |              50 |
	And second round winner should be null

Scenario: Cannot get winner (InComing)
	When get round winner
	Then should throw an error with message Unable to get the round winner, vote is 'InComing'.

Scenario: Cannot get winner (Opened)
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	When add the candidates to the vote
	And open vote
	And get round winner
	Then should throw an error with message Unable to get the round winner, vote is 'Opened'.

Scenario: Cannot start vote (No candidates)
	When open vote
	Then should throw an error with message Unable to start vote, no candidates assigned.

Scenario: Cannot start vote (Opened)
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	When add the candidates to the vote
	And open vote
	And open vote
	Then should throw an error with message Unable to start vote, the vote is 'Opened'.

Scenario: Cannot start vote (Closed)
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	And first round results are
	| Candidates | Vote Number |
	| Candidate1 |          50 |
	| Candidate2 |          20 |
	| Candidate3 |          30 |
	When add the candidates to the vote
	And open vote
	And set round results
	And close vote
	And open vote
	Then should throw an error with message Unable to start vote, the vote is 'Closed'.

Scenario: Cannot close vote (InComing)
	When close vote
	Then should throw an error with message Unable to close the vote, the vote is 'InComing'.

Scenario: Cannot close vote (Closed)
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	And first round results are
	| Candidates | Vote Number |
	| Candidate1 |          50 |
	| Candidate2 |          20 |
	| Candidate3 |          30 |
	When add the candidates to the vote
	And open vote
	And set round results
	And close vote
	And close vote
	Then should throw an error with message Unable to close the vote, the vote is 'Closed'.