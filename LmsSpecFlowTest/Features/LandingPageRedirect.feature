Feature: Landing page


Scenario: User gets redirected to the correct landing page after logging in
	Given that the user is an admin	
	When the user logs in
	Then the user should be redirected to AdminOverview view