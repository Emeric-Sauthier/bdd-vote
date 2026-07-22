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

Scenario: Cannot affect the same candidate two times
	Given candidates are
	| Candidates |
	| Candidate1 |
	| Candidate1 |
	When add the candidates to the vote
	Then should throw an error with message Unable to add 'Candidate1', duplicated.