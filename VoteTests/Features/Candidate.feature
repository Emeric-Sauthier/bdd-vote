Feature: Candidates

Scenario to test the affectation of candidates

@mytag
Scenario: Affect candidates to the vote
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |
	When add the candidates to the vote
	Then candidates should be
	| Candidates |
	| Candidate1 |
	| Candidate2 |
	| Candidate3 |